using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Godot;
using XCardGame.Scripts.Buffs;
using XCardGame.Scripts.Cards;
using XCardGame.Scripts.Cards.InteractCards;
using XCardGame.Scripts.Common;
using XCardGame.Scripts.Common.Constants;
using XCardGame.Scripts.Defs;
using XCardGame.Scripts.Defs.Def;
using XCardGame.Scripts.Defs.Def.Card;
using XCardGame.Scripts.Effects;
using XCardGame.Scripts.HandEvaluate;
using XCardGame.Scripts.Ui;
// using Godot.Collections;

namespace XCardGame.Scripts.Game;

public partial class Battle: Node2D
{

    public class SetupArgs
    {
        public int DealCommunityCardCount;
        public int FaceDownCommunityCardCount;
        public int RequiredHoleCardCountMin;
        public int RequiredHoleCardCountMax;
        public List<BattleEntity.SetupArgs> EntitySetupArgs;
        
    }

    public enum State
    {
        BeforeDealCards,
        BeforeShowDown,
        BeforeResolve,
        AfterResolve,
    }
    
    public GameMgr GameMgr;
    public Dealer Dealer;
    public CardContainer CommunityCardContainer;
    public SplitCardContainer ResolveCardContainer;
    public CardContainer ItemCardContainer;
    public CardContainer RuleCardContainer;
    public PlayerBattleEntity Player;
    public BattleEntity Enemy;
    public BattleEntity[] Entities;

    public BaseButton BigButton;
    public PackedScene GameOverScene;
    public PackedScene GameWinScene;
    public PackedScene SelectRewardCardScene;
    
    public Action<Battle> OnBattleProceed;
    public Action<Battle, CardNode> OnDealCard;
    public Action<Battle, CardNode> OnDealtCard;
    public Action<Battle> AfterDealCards;
    public Action<Battle> OnRoundStart;
    public Action<Battle> OnRoundEnd;
    public Action<Battle> BeforeShowDown;
    public Action<Battle, Engage> BeforeEngage;
    public Action<Battle> OnPlayerDefeated;
    public Action<Battle> OnEnemyDefeated;
    public Action<Battle, BattleEntity> OnNewEnemy;
    public Action<Battle> OnBattleFinished;

    // public ObservableCollection<BaseCard> CommunityCards;
    // public ObservableCollection<BaseCard> PrimaryCards;
    // public ObservableCollection<BaseCard> KickerCards;
    // public ObservableCollection<BaseCard> RuleCards;
    
    public CompletedHandEvaluator HandEvaluator;
    
    public int DealCommunityCardCount;
    public int FaceDownCommunityCardCount;
    public int RequiredHoleCardCountMin;
    public int RequiredHoleCardCountMax;
    
    public int RoundCount;
    public Dictionary<BattleEntity, CompletedHand> RoundHands;
    public Engage RoundEngage;

    public ObservableCollection<Enums.HandTier> HandTierOrderDescend;
    
    public List<BaseEffect> Effects;
    public State CurrentState;

    protected List<EnemyRandBoxDef> AllEnemyRandBoxDefs;
    
    public override void _Ready()
    {
        base._Ready();
        GameMgr = GetNode<GameMgr>("/root/GameMgr");
        Dealer = GetNode<Dealer>("Dealer");
        CommunityCardContainer = GetNode<CardContainer>("CommunityCards");
        ResolveCardContainer = GetNode<SplitCardContainer>("ResolveCards");
        ItemCardContainer = GetNode<CardContainer>("ItemCards");
        RuleCardContainer = GetNode<CardContainer>("RuleCards");
        
        Player = GetNode<PlayerBattleEntity>("Player");
        Enemy = GetNode<BattleEntity>("Enemy");
        GameOverScene = ResourceCache.Instance.Load<PackedScene>("res://Scenes/GameOver.tscn");
        GameWinScene = ResourceCache.Instance.Load<PackedScene>("res://Scenes/GameWin.tscn");
        SelectRewardCardScene = ResourceCache.Instance.Load<PackedScene>("res://Scenes/SelectRewardCard.tscn");
        Entities = new [] { Player, Enemy };
        
        RoundHands = new Dictionary<BattleEntity, CompletedHand>();
        // CommunityCards = new ObservableCollection<BaseCard>();
        // PrimaryCards = new ObservableCollection<BaseCard>();
        // KickerCards = new ObservableCollection<BaseCard>();
        // RuleCards = new ObservableCollection<BaseCard>();
        HandTierOrderDescend = new ObservableCollection<Enums.HandTier>();
        Effects = new List<BaseEffect>();
    }

    public virtual void Setup(object o)
    {
        var args = (SetupArgs)o;
        DealCommunityCardCount = args.DealCommunityCardCount;
        FaceDownCommunityCardCount = args.FaceDownCommunityCardCount;
        RequiredHoleCardCountMin = args.RequiredHoleCardCountMin;
        RequiredHoleCardCountMax = args.RequiredHoleCardCountMax;
        HandEvaluator = new CompletedHandEvaluator(Configuration.CompletedHandCardCount, 
            RequiredHoleCardCountMin, RequiredHoleCardCountMax);
        
        var entitiesSetupArgs = args.EntitySetupArgs;
        for (int i = 0; i < Entities.Length; i++)
        {
            var entity = Entities[i];
            entity.Setup(entitiesSetupArgs[i]);
            entity.OnDefeated += OnEntityDefeated;
        }
        
        var containerSetupArgs = new CardContainer.SetupArgs
        {
            ContentNodeSize = Configuration.CardSize,
            Separation = Configuration.CardContainerSeparation,
            PivotDirection = Enums.Direction2D8Ways.Neutral,
            DefaultCardFaceDirection = Enums.CardFace.Up,
            Margins = Configuration.DefaultContentContainerMargins,
        };
        
        var resolveCardContainerSetupArgs = new SplitCardContainer.SetupArgs
        {
            CardContainersSetupArgs = new List<CardContainer.SetupArgs>
            {
                containerSetupArgs,
                containerSetupArgs,
            },
            Separation = Configuration.SplitCardContainerSeparation,
            PivotDirection = Enums.Direction2D8Ways.Neutral,
        };
        
        ResolveCardContainer.Setup(resolveCardContainerSetupArgs);
        
        containerSetupArgs.GetCardFaceDirectionFunc = GetCommunityCardFaceDirectionFunc;
        containerSetupArgs.ShouldCollectDealtItemAndRuleCards = true;
        CommunityCardContainer.Setup(containerSetupArgs);
        containerSetupArgs.GetCardFaceDirectionFunc = null;
        containerSetupArgs.ShouldCollectDealtItemAndRuleCards = false;
        
        containerSetupArgs.AllowInteract = true;
        containerSetupArgs.ExpectedInteractCardDefType = typeof(ItemCardDef);
        
        ItemCardContainer.Setup(containerSetupArgs);
        ItemCardContainer.OnAddContentNode += OnItemCardContainerAddNode;

        containerSetupArgs.ExpectedInteractCardDefType = typeof(RuleCardDef);
        RuleCardContainer.Setup(containerSetupArgs);


        Dealer.Setup(new Dealer.SetupArgs
        {
            SourceDecks = Entities.Select(e => e.Deck).ToList(),
        });

        foreach (var handTierValue in ((Enums.HandTier[])Enum.GetValues(typeof(Enums.HandTier))).Reverse())
        {
            HandTierOrderDescend.Add(handTierValue);
        }

        AllEnemyRandBoxDefs = EnemyRandBoxDefs.All();
        
        Reset();
    }

    public async void Start()
    {
        Reset();
        Dealer.Shuffle();
        await GameMgr.AwaitAndDisableInput(Dealer.DealInnateCards());
        await GameMgr.AwaitAndDisableInput(NewRound());
        CurrentState = State.BeforeDealCards;
    }

    public async void Proceed()
    {
        switch (CurrentState)
        {
            case State.BeforeDealCards:
                await GameMgr.AwaitAndDisableInput(DealCards());
                CurrentState = State.BeforeShowDown;
                break;
            case State.BeforeShowDown:
                await GameMgr.AwaitAndDisableInput(ShowDown());
                CurrentState = State.BeforeResolve;
                break;
            case State.BeforeResolve:
                await GameMgr.AwaitAndDisableInput(RoundEngage.Resolve());
                CurrentState = State.AfterResolve;
                break;
            case State.AfterResolve:
                OnRoundEnd?.Invoke(this);
                var isPlayerDefeated = Player.IsDefeated();
                var isEnemyDefeated = Enemy.IsDefeated();
                await GameMgr.AwaitAndDisableInput(NewRound());
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
                        await GameMgr.AwaitAndDisableInput(Dealer.AnimateShuffle());
                        var selectRewardCard = GameMgr.OverlayScene(SelectRewardCardScene) as SelectRewardCard;
                        selectRewardCard.OnQuit += AfterSelectRewardCard;
                        selectRewardCard.Setup(new SelectRewardCard.SetupArgs
                        {
                            RewardCardCount = Configuration.DefaultRewardCardCount,
                            RewardCardDefType = typeof(ItemCardDef),
                            InitReRollPrice = Configuration.DefaultReRollPrice,
                            ReRollPriceIncrease = Configuration.DefaultReRollPriceIncrease,
                            SkipReward = Configuration.DefaultSkipReward
                        });
                    }
                }
                CurrentState = State.BeforeDealCards;
                break;
        }
        OnBattleProceed?.Invoke(this);
    }
    
    public async Task NewRound()
    {
        async Task DiscardCards(List<CardNode> cardNodes)
        {
            var tasks = new List<Task>();
            foreach (var cardNode in cardNodes)
            {
                tasks.Add(Dealer.AnimateDiscard(cardNode));
                await Utils.Wait(this, Configuration.AnimateCardTransformInterval);
                GD.Print($"time: {Time.GetTicksMsec()}");
            }
            await Task.WhenAll(tasks);
        }
        foreach (var entity in Entities)
        {
            await entity.RoundReset();
        }
        await DiscardCards(CommunityCardContainer.CardNodes);
        foreach (var cardContainer in ResolveCardContainer.CardContainers)
        {
            await DiscardCards(cardContainer.CardNodes);
        }
        RoundCount++;
        OnRoundStart?.Invoke(this);
        RoundHands.Clear();
        RoundEngage = null;
    }
    
    public void Reset()
    {
        async Task DiscardCards(List<CardNode> cardNodes)
        {
            var tasks = new List<Task>();
            foreach (var cardNode in cardNodes)
            {
                tasks.Add(Dealer.AnimateDiscard(cardNode));
                await Utils.Wait(this, Configuration.AnimateCardTransformInterval);
            }
            await Task.WhenAll(tasks);
        }
        var tasks = new List<Task>();
        RoundCount = 0;
        CurrentState = State.BeforeDealCards;
        foreach (var entity in Entities)
        {
            entity.Reset();
        }
        Dealer.Reset();
        tasks.Add(DiscardCards(ItemCardContainer.CardNodes));
        tasks.Add(DiscardCards(RuleCardContainer.CardNodes));
        Task.WhenAll(tasks);
    }

    public async Task DealCards()
    {
        var tasks = new List<Task>();
        foreach (var entity in Entities)
        {
            for (int i = 0; i < entity.DealCardCount; i++)
            {
                tasks.Add(Dealer.DealCardIntoContainer(entity.HoleCardContainer));
                await Utils.Wait(this, Configuration.AnimateCardTransformInterval);
            }
        }
        for (int i = 0; i < DealCommunityCardCount; i++)
        {
            tasks.Add(Dealer.DealCardIntoContainer(CommunityCardContainer));
            await Utils.Wait(this, Configuration.AnimateCardTransformInterval);
        }
        await Task.WhenAll(tasks);
        AfterDealCards?.Invoke(this);
    }
    
    public async Task ShowDown()
    {
        BeforeShowDown?.Invoke(this);
        
        // var startTime = Time.GetTicksUsec();

        // var validCommunityCards = GetValidCards(CommunityCards);
        foreach (var entity in Entities)
        {
            // var validHoleCards = GetValidCards(entity.HoleCards);
            var bestHand = HandEvaluator.EvaluateBestHand(CommunityCardContainer.Cards, entity.HoleCardContainer.Cards, HandTierOrderDescend.ToList());
            entity.RoundHandTier.Value = bestHand.Tier;
            RoundHands.Add(entity, bestHand);
        }

        var tasks = new List<Task>();
        foreach (var entity in Entities)
        {
            foreach (var cardNode in entity.HoleCardContainer.CardNodes)
            {
                if (cardNode.FaceDirection.Value == Enums.CardFace.Down)
                {
                    tasks.Add(cardNode.AnimateFlip(Enums.CardFace.Up));
                }
            }
        }
        foreach (var cardNode in CommunityCardContainer.CardNodes)
        {
            if (cardNode.FaceDirection.Value == Enums.CardFace.Down)
            {
                tasks.Add(cardNode.AnimateFlip(Enums.CardFace.Up));
            }
        }
        var playerHand = RoundHands[Player];
        var enemyHand = RoundHands[Enemy];
        RoundEngage = new Engage(GameMgr, playerHand, enemyHand);
        tasks.Add(GameMgr.BattleLog.LogAndWait(
            Utils._($"Showdown: {Player}({playerHand.Tier}) vs {Enemy}({enemyHand.Tier}). {Player} is {Player.RoundRole.Value}. {Enemy} is {Enemy.RoundRole.Value}"),
            Configuration.LogInterval));
        BeforeEngage?.Invoke(this, RoundEngage);
        await Task.WhenAll(tasks);
        // RoundEngage.PrepareRoundSkills();
        // GameMgr.InputMgr.SwitchToInputHandler(new PrepareRoundSkillInputHandler(GameMgr));
        // var endTime = Time.GetTicksUsec();
        // GD.Print($"Hand evaluation time: {endTime - startTime} us");
        // GD.Print($"{Players[0]} Best Hand: {playerBestHand.Rank}, {string.Join(",", playerBestHand.PrimaryCards)}, Kickers: {string.Join(",", playerBestHand.Kickers)}");
        // GD.Print($"{Players[1]} Best Hand: {opponentBestHand.Rank}, {string.Join(",", opponentBestHand.PrimaryCards)}, Kickers: {string.Join(",", opponentBestHand.Kickers)}");
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
        if (target.BuffContainer.Buffs.Contains(buff))
        {
            buff.Repeat(this, target, source, sourceCard);
        }
        else
        {
            buff.InflictOn(target, source, sourceCard);
        }
    }

    protected Enums.CardFace GetCommunityCardFaceDirectionFunc(int i)
    {
        if (CurrentState >= State.BeforeResolve)
        {
            return Enums.CardFace.Up;
        }
        else
        {
            return i < DealCommunityCardCount - FaceDownCommunityCardCount ? Enums.CardFace.Up : Enums.CardFace.Down;
        }
    }

    // protected void OnEntityAbilityCardsChanged(object sender, NotifyCollectionChangedEventArgs args)
    // {
    //     switch (args.Action)
    //     {
    //         case NotifyCollectionChangedAction.Add:
    //             if (args.NewItems != null)
    //                 foreach (var t in args.NewItems)
    //                 {
    //                     if (t is BaseCard card)
    //                     {
    //                         FieldCards.Add(card);
    //                     }
    //                 }
    //             break;
    //         case NotifyCollectionChangedAction.Remove:
    //             if (args.OldItems != null)
    //                 foreach (var t in args.OldItems)
    //                 {
    //                     if (t is BaseCard card)
    //                     {
    //                         FieldCards.Remove(card);
    //                     }
    //                 }
    //             break;
    //         case NotifyCollectionChangedAction.Reset:
    //             var removedCards = new List<BaseCard>();
    //             foreach (var fieldCard in FieldCards)
    //             {
    //                 var found = false;
    //                 foreach (var entity in Entities)
    //                 {
    //                     if (entity.AbilityCards.Contains(fieldCard))
    //                     {
    //                         found = true;
    //                         break;
    //                     }
    //                 }
    //                 if (!found) removedCards.Add(fieldCard);
    //             }
    //
    //             foreach (var removedCard in removedCards)
    //             {
    //                 FieldCards.Remove(removedCard);
    //             }
    //
    //             break;
    //     }
    // }

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
        var boxes = AllEnemyRandBoxDefs
            .Where(def => def.ProgressRange.X <= GameMgr.ProgressCounter.Value
                          && GameMgr.ProgressCounter.Value < def.ProgressRange.Y).ToList();
        var i = Utils.RandOnWeight(boxes.Select(x => x.RandBox.RandBoxWeight).ToList(), GameMgr.Rand);
        var boxDef = boxes[i];
        var box = boxDef.RandBox;
        var entityDef = box.Rand(GameMgr.Rand);
        Enemy.Setup(BattleEntity.InitArgs(entityDef));
        OnNewEnemy?.Invoke(this, Enemy);
    }

    protected void AfterSelectRewardCard()
    {
        NewChallenger();
        Start();
    }

    protected async void OnItemCardContainerAddNode(BaseContentNode node)
    {
        if (ItemCardContainer.ContentNodes.Count > Player.ItemPocketSize.Value)
        {
            await GameMgr.AwaitAndDisableInput(Dealer.AnimateDiscard((CardNode)ItemCardContainer.ContentNodes[0]));
        }
    }
}