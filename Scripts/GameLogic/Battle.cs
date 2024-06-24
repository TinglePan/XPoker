using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using Godot;
using XCardGame.Scripts.Buffs;
using XCardGame.Scripts.Cards;
using XCardGame.Scripts.Cards.AbilityCards;
using XCardGame.Scripts.Common;
using XCardGame.Scripts.Common.Constants;
using XCardGame.Scripts.Common.DataBinding;
using XCardGame.Scripts.Defs;
using XCardGame.Scripts.Effects;
using XCardGame.Scripts.HandEvaluate;
using XCardGame.Scripts.InputHandling;
using XCardGame.Scripts.Nodes;

namespace XCardGame.Scripts.GameLogic;

public partial class Battle: Node2D, ISetup
{

    public enum State
    {
        BeforeDealCards,
        BeforeShowDown,
        BeforeResolve,
        AfterResolve,
    }
    
    public GameMgr GameMgr;
    public Dealer Dealer;
    public SkillDisplay SkillDisplay;
    public CardContainer CommunityCardContainer;
    public CardContainer FieldCardContainer;
    public PlayerBattleEntity Player;
    public BattleEntity Enemy;
    public BattleEntity[] Entities;

    public BaseButton ProceedButton;
    public PackedScene GameOverScene;
    public PackedScene GameWinScene;
    public PackedScene SelectRewardCardScene;
    
    public bool HasSetup { get; set; }
    
    public Action<Battle> OnBattleProceed;
    public Action<Battle> AfterDealCards;
    public Action<Battle> OnRoundStart;
    public Action<Battle> OnRoundEnd;
    public Action<Battle> BeforeShowDown;
    public Action<Battle, Engage> BeforeEngage;
    public Action<Battle> OnPlayerDefeated;
    public Action<Battle> OnEnemyDefeated;
    public Action<Battle, BattleEntity> OnNewEnemy;
    public Action<Battle> OnBattleFinished;

    public ObservableCollection<BaseCard> CommunityCards;
    public ObservableCollection<BaseCard> FieldCards;
    
    public CompletedHandEvaluator HandEvaluator;
    
    public int DealCommunityCardCount;
    public int FaceDownCommunityCardCount;
    public int RequiredHoleCardCountMin;
    public int RequiredHoleCardCountMax;
    
    public int RoundCount;
    public Dictionary<BattleEntity, CompletedHand> RoundHands;
    public Engage RoundEngage;

    public ObservableCollection<Enums.HandTier> HandTierOrderAscend;
    
    public List<BaseEffect> Effects;
    public State CurrentState;

    
    public override void _Ready()
    {
        base._Ready();
        GameMgr = GetNode<GameMgr>("/root/GameMgr");
        Dealer = GetNode<Dealer>("Dealer");
        SkillDisplay = GetNode<SkillDisplay>("SkillDisplay");
        CommunityCardContainer = GetNode<CardContainer>("CommunityCards");
        FieldCardContainer = GetNode<CardContainer>("FieldCards");
        Player = GetNode<PlayerBattleEntity>("Player");
        Enemy = GetNode<BattleEntity>("Enemy");
        GameOverScene = ResourceCache.Instance.Load<PackedScene>("res://Scenes/GameOver.tscn");
        GameWinScene = ResourceCache.Instance.Load<PackedScene>("res://Scenes/GameWin.tscn");
        SelectRewardCardScene = ResourceCache.Instance.Load<PackedScene>("res://Scenes/SelectRewardCard.tscn");
        Entities = new [] { Player, Enemy };
        HasSetup = false;
        RoundHands = new Dictionary<BattleEntity, CompletedHand>();
        CommunityCards = new ObservableCollection<BaseCard>();
        FieldCards = new ObservableCollection<BaseCard>();
        HandTierOrderAscend = new ObservableCollection<Enums.HandTier>();
        Effects = new List<BaseEffect>();
    }

    public virtual void Setup(Dictionary<string, object> args)
    {
        DealCommunityCardCount = (int)args["dealCommunityCardCount"];
        FaceDownCommunityCardCount = (int)args["faceDownCommunityCardCount"];
        RequiredHoleCardCountMin = (int)args["requiredHoleCardCountMin"];
        RequiredHoleCardCountMax = (int)args["requiredHoleCardCountMax"];
        HandEvaluator = new CompletedHandEvaluator(Configuration.CompletedHandCardCount, 
            RequiredHoleCardCountMin, RequiredHoleCardCountMax);

        CommunityCardContainer.Setup(new Dictionary<string, object>()
        {
            { "allowInteract", false },
            { "cards", CommunityCards },
            { "contentNodeSize", Configuration.CardSize },
            { "separation", Configuration.CardContainerSeparation },
            { "pivotDirection", Enums.Direction2D8Ways.Neutral },
            { "nodesPerRow", 0 },
            { "hasBorder", true },
            { "expectedContentNodeCount", Configuration.CommunityCardCount },
            { "hasName", true },
            { "containerName", "Community cards"},
            { "getCardFaceDirectionFunc", (Func<int, Enums.CardFace>)GetCommunityCardFaceDirectionFunc },
            { "margins", Configuration.DefaultContentContainerMargins },
            { "withCardEffect", true }
        });
        
        FieldCardContainer.Setup(new Dictionary<string, object>()
        {
            { "allowInteract", true },
            { "cards", FieldCards },
            { "contentNodeSize", Configuration.CardSize },
            { "separation", Configuration.CardContainerSeparation },
            { "pivotDirection", Enums.Direction2D8Ways.Neutral },
            { "nodesPerRow", 0 },
            { "hasBorder", true },
            { "expectedContentNodeCount", 0 },
            { "hasName", true },
            { "containerName", "Field cards"},
            { "defaultCardFaceDirection", Enums.CardFace.Up },
            { "margins", Configuration.DefaultContentContainerMargins },
            { "withCardEffect", true }
        });
        
        SkillDisplay.Setup(new Dictionary<string, object>()
        {
            { "playerSkillCardContainer", Player.SkillCardContainer },
            { "enemySkillCardContainer", Enemy.SkillCardContainer },
        });
        
        var entitiesSetupArgs = (List<Dictionary<string, object>>)args["entities"];
        for (int i = 0; i < Entities.Length; i++)
        {
            var entity = Entities[i];
            entity.Setup(entitiesSetupArgs[i]);
            entity.OnDefeated += OnEntityDefeated;
            entity.AbilityCards.CollectionChanged += OnEntityAbilityCardsChanged;
            foreach (var abilityCard in entity.AbilityCards)
            {
                FieldCards.Add(abilityCard);
            }
        }
        
        Dealer.Setup(new Dictionary<string, object>()
        {
            { "sourceDecks" , Entities.Select(e => e.Deck).ToList() },
            { "excludedCards" , null }
        });

        foreach (var handTierValue in Enum.GetValues(typeof(Enums.HandTier)))
        {
            HandTierOrderAscend.Add((Enums.HandTier)handTierValue);
        }
        
        
        // Player.Setup((Dictionary<string, object>)args["playerSetupArgs"]);
        // var iEnemy = 0;
        // foreach (var entity in Entities)
        // {
        //     if (entity is PlayerBattleEntity) continue;
        //     entity.Setup(((List<Dictionary<string, object>>)args["enemySetupArgs"])[iEnemy]);
        //     iEnemy++;
        // }
        
        Reset();

        HasSetup = true;
    }

    public void EnsureSetup()
    {
        if (!HasSetup)
        {
            GD.PrintErr($"{this} not setup yet");
        }
    }

    public void Start()
    {
        Reset();
        Dealer.Shuffle();
        NewRound();
    }

    public async void Proceed()
    {
        switch (CurrentState)
        {
            case State.BeforeDealCards:
                DealCards();
                break;
            case State.BeforeShowDown:
                ShowDown();
                break;
            case State.BeforeResolve:
                ResolveSkills();
                break;
            case State.AfterResolve:
                var isPlayerDefeated = Player.IsDefeated();
                var isEnemyDefeated = Enemy.IsDefeated();
                await NewRound();
                if (isPlayerDefeated)
                {
                    GameOver();
                    return;
                }
                if (isEnemyDefeated)
                {
                    if (GameMgr.ProgressCounter.Value >= Configuration.ProgressCountRequiredToWin)
                    {
                        GameWin();
                    }
                    else
                    {
                        Dealer.AnimateShuffle();
                        var selectRewardCard = GameMgr.OverlayScene(SelectRewardCardScene) as SelectRewardCard;
                        selectRewardCard.OnQuit += AfterSelectRewardCard;
                        selectRewardCard.Setup(new Dictionary<string, object>()
                        {
                            { "rewardCardCount", Configuration.DefaultRewardCardCount },
                            { "rewardCardDefType", typeof(AbilityCardDef) },
                            { "reRollPrice", Configuration.DefaultReRollPrice },
                            { "reRollPriceIncrease", Configuration.DefaultReRollPriceIncrease },
                            { "skipReward", Configuration.DefaultSkipReward }
                        });
                    }
                }
                break;
        }
        OnBattleProceed?.Invoke(this);
    }
    
    public async Task NewRound()
    {
        RoundCount++;
        OnRoundStart?.Invoke(this);
        foreach (var entity in Entities)
        {
            await entity.RoundReset(Configuration.AnimateCardTransformInterval);
        }
        foreach (var cardNode in CommunityCardContainer.ContentNodes.ToList())
        {
            await Dealer.AnimateDiscard(cardNode, Configuration.AnimateCardTransformInterval);
        }
        // CommunityCardContainer.ContentNodes.Clear();
        RoundHands.Clear();
        RoundEngage = null;
        CurrentState = State.BeforeDealCards;
    }
    
    public void Reset()
    {
        RoundCount = 0;
        CurrentState = State.BeforeDealCards;
        foreach (var entity in Entities)
        {
            entity.Reset();
        }
        CommunityCardContainer.ContentNodes.Clear();
        Dealer.Reset();
    }

    public void DealCards()
    {
        var cardDealt = 0;
        foreach (var entity in Entities)
        {
            for (int i = 0; i < entity.DealCardCount; i++)
            {
                Dealer.DealCardIntoContainer(entity.HoleCardContainer, delay: cardDealt * Configuration.AnimateCardTransformInterval);
                cardDealt++;
            }
        }
        for (int i = 0; i < DealCommunityCardCount; i++)
        {
            Dealer.DealCardIntoContainer(CommunityCardContainer, delay: cardDealt * Configuration.AnimateCardTransformInterval);
            cardDealt++;
        }
        AfterDealCards?.Invoke(this);
        CurrentState = State.BeforeShowDown;
    }
    
    public void ShowDown()
    {
        BeforeShowDown?.Invoke(this);
        
        // var startTime = Time.GetTicksUsec();

        foreach (var entity in Entities)
        {
            var bestHand = 
                HandEvaluator.EvaluateBestHand(CommunityCardContainer.Contents.ToList(),
                    entity.HoleCardContainer.Contents.ToList());
            RoundHands.Add(entity, bestHand);
        }

        foreach (var entity in Entities)
        {
            foreach (var cardNode in entity.HoleCardContainer.ContentNodes)
            {
                if (cardNode.FaceDirection.Value == Enums.CardFace.Down)
                {
                    cardNode.AnimateFlip(Enums.CardFace.Up);
                }
            }
        }
        foreach (var cardNode in CommunityCardContainer.ContentNodes)
        {
            if (cardNode.FaceDirection.Value == Enums.CardFace.Down)
            {
                cardNode.AnimateFlip(Enums.CardFace.Up);
            }
        }
        

        var playerHand = RoundHands[Player];
        var enemyHand = RoundHands[Enemy];
        RoundEngage = new Engage(GameMgr, playerHand, enemyHand);
        BeforeEngage?.Invoke(this, RoundEngage);
        RoundEngage.PrepareRoundSkills();
        CurrentState = State.BeforeResolve;
        GameMgr.InputMgr.SwitchToInputHandler(new PrepareRoundSkillInputHandler(GameMgr));
        // var endTime = Time.GetTicksUsec();
        // GD.Print($"Hand evaluation time: {endTime - startTime} us");
        // GD.Print($"{Players[0]} Best Hand: {playerBestHand.Rank}, {string.Join(",", playerBestHand.PrimaryCards)}, Kickers: {string.Join(",", playerBestHand.Kickers)}");
        // GD.Print($"{Players[1]} Best Hand: {opponentBestHand.Rank}, {string.Join(",", opponentBestHand.PrimaryCards)}, Kickers: {string.Join(",", opponentBestHand.Kickers)}");
    }

    public void ResolveSkills()
    {
        RoundEngage.Resolve();
        CurrentState = State.AfterResolve;
        OnRoundEnd?.Invoke(this);
    }

    public BattleEntity GetOpponentOf(BattleEntity entity)
    {
        foreach (var maybeOpponent in Entities)
        {
            if (maybeOpponent != entity)
            {
                return maybeOpponent;
            }
        }
        return null;
    }

    public void OnEntityDefeated(BattleEntity e)
    {
        GD.Print($"{e} defeated");
        if (e == Player)
        {
            GD.Print($"You lose");
            OnPlayerDefeated?.Invoke(this);
        }
        else
        {
            GD.Print($"You win");
            Player.Credit.Value += Mathf.Abs(Player.Hp.Value - Player.MaxHp.Value / 2);
            GameMgr.ProgressCounter.Value++;
            OnEnemyDefeated?.Invoke(this);
        }
        OnBattleFinished?.Invoke(this);
    }
    
    public void StartEffect(BaseEffect effect)
    {
        if (!Effects.Contains(effect))
        {
            Effects.Add(effect);
            effect.OnStart(this);
        }
    }

    public void StopEffect(BaseEffect effect)
    {
        if (Effects.Contains(effect))
        {
            effect.OnStop(this);
            Effects.Remove(effect);
        }
    }

    public void InflictBuffOn(BaseBuff buff, BattleEntity target, BattleEntity source, BaseCard sourceCard = null)
    {
        if (target.Buffs.Contains(buff))
        {
            buff.Repeat(this, target, source, sourceCard);
        }
        else
        {
            buff.InflictOn(target, source, sourceCard);
        }
    }

    public int GetHandTierValue(Enums.HandTier handTier)
    {
        return HandTierOrderAscend.IndexOf(handTier);
    }

    protected Enums.CardFace GetCommunityCardFaceDirectionFunc(int i)
    {
        return i < DealCommunityCardCount - FaceDownCommunityCardCount ? Enums.CardFace.Up : Enums.CardFace.Down;
    }

    protected void OnEntityAbilityCardsChanged(object sender, NotifyCollectionChangedEventArgs args)
    {
        switch (args.Action)
        {
            case NotifyCollectionChangedAction.Add:
                if (args.NewItems != null)
                    foreach (var t in args.NewItems)
                    {
                        if (t is BaseCard card)
                        {
                            FieldCards.Add(card);
                        }
                    }
                break;
            case NotifyCollectionChangedAction.Remove:
                if (args.OldItems != null)
                    foreach (var t in args.OldItems)
                    {
                        if (t is BaseCard card)
                        {
                            FieldCards.Remove(card);
                        }
                    }
                break;
            case NotifyCollectionChangedAction.Reset:
                var removedCards = new List<BaseCard>();
                foreach (var fieldCard in FieldCards)
                {
                    var found = false;
                    foreach (var entity in Entities)
                    {
                        if (entity.AbilityCards.Contains(fieldCard))
                        {
                            found = true;
                            break;
                        }
                    }
                    if (!found) removedCards.Add(fieldCard);
                }

                foreach (var removedCard in removedCards)
                {
                    FieldCards.Remove(removedCard);
                }

                break;
        }
    }

    protected void GameOver()
    {
        GameMgr.InputMgr.QuitCurrentInputHandler();
        GameMgr.ChangeScene(GameOverScene);
    }

    protected void GameWin()
    {
        GameMgr.InputMgr.QuitCurrentInputHandler();
        GameMgr.ChangeScene(GameWinScene);
    }

    protected void NewChallenger()
    {
        Enemy.Setup(BattleEntity.InitArgs(BattleEntityDefs.DefaultEnemyBattleEntityDef));
        OnNewEnemy?.Invoke(this, Enemy);
    }

    protected void AfterSelectRewardCard()
    {
        NewChallenger();
        Start();
    }
}