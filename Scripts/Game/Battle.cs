using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using Godot;
// using Godot.Collections;
using XCardGame.Scripts.Buffs;
using XCardGame.Scripts.Cards;
using XCardGame.Scripts.Cards.AbilityCards;
using XCardGame.Scripts.Common;
using XCardGame.Scripts.Common.Constants;
using XCardGame.Scripts.Common.DataBinding;
using XCardGame.Scripts.Defs;
using XCardGame.Scripts.Defs.Def.Card;
using XCardGame.Scripts.Effects;
using XCardGame.Scripts.HandEvaluate;
using XCardGame.Scripts.InputHandling;
using XCardGame.Scripts.Nodes;
using XCardGame.Scripts.Ui;

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
    public CardContainer CommunityCardContainer;
    public CardContainer EquipmentCardContainer;
    public CardContainer ItemCardContainer;
    public CardContainer RuleCardContainer;
    public CardContainer ResolveCardContainer;
    public CardContainer KickerCardContainer;
    public PlayerBattleEntity Player;
    public BattleEntity Enemy;
    public BattleEntity[] Entities;
    public RoleMarker PlayerRoleMarker;
    public RoleMarker EnemyRoleMarker;
    public System.Collections.Generic.Dictionary<BattleEntity, RoleMarker> RoleMarkers;

    public BaseButton BigButton;
    public PackedScene GameOverScene;
    public PackedScene GameWinScene;
    public PackedScene SelectRewardCardScene;
    
    public bool HasSetup { get; set; }
    
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

    public ObservableCollection<BaseCard> CommunityCards;
    public ObservableCollection<BaseCard> RuleCards;
    public ObservableCollection<BaseCard> ResolveCards;
    public ObservableCollection<BaseCard> KickerCards;
    
    public CompletedHandEvaluator HandEvaluator;
    
    public int DealCommunityCardCount;
    public int FaceDownCommunityCardCount;
    public int RequiredHoleCardCountMin;
    public int RequiredHoleCardCountMax;
    
    public int RoundCount;
    public System.Collections.Generic.Dictionary<BattleEntity, CompletedHand> RoundHands;
    public Engage RoundEngage;

    public ObservableCollection<Enums.HandTier> HandTierOrderAscend;
    
    public List<BaseEffect> Effects;
    public State CurrentState;

    
    public override void _Ready()
    {
        base._Ready();
        GameMgr = GetNode<GameMgr>("/root/GameMgr");
        Dealer = GetNode<Dealer>("Dealer");
        CommunityCardContainer = GetNode<CardContainer>("CommunityCards");
        EquipmentCardContainer = GetNode<CardContainer>("EquipmentCards");
        ItemCardContainer = GetNode<CardContainer>("ItemCards");
        RuleCardContainer = GetNode<CardContainer>("RuleCards");
        ResolveCardContainer = GetNode<CardContainer>("ResolveCards");
        KickerCardContainer = GetNode<CardContainer>("KickerCards");
        
        Player = GetNode<PlayerBattleEntity>("Player");
        Enemy = GetNode<BattleEntity>("Enemy");
        GameOverScene = ResourceCache.Instance.Load<PackedScene>("res://Scenes/GameOver.tscn");
        GameWinScene = ResourceCache.Instance.Load<PackedScene>("res://Scenes/GameWin.tscn");
        SelectRewardCardScene = ResourceCache.Instance.Load<PackedScene>("res://Scenes/SelectRewardCard.tscn");
        Entities = new [] { Player, Enemy };
        HasSetup = false;
        PlayerRoleMarker = GetNode<RoleMarker>("PlayerRoleMarker");
        EnemyRoleMarker = GetNode<RoleMarker>("EnemyRoleMarker");
        RoleMarkers = new Dictionary<BattleEntity, RoleMarker>
        {
            { Player, PlayerRoleMarker },
            { Enemy, EnemyRoleMarker }
        };
        
        RoundHands = new Dictionary<BattleEntity, CompletedHand>();
        CommunityCards = new ObservableCollection<BaseCard>();
        RuleCards = new ObservableCollection<BaseCard>();
        ResolveCards = new ObservableCollection<BaseCard>();
        KickerCards = new ObservableCollection<BaseCard>();
        HandTierOrderAscend = new ObservableCollection<Enums.HandTier>();
        Effects = new List<BaseEffect>();
    }

    public virtual void Setup(System.Collections.Generic.Dictionary<string, object> args)
    {
        DealCommunityCardCount = (int)args["dealCommunityCardCount"];
        FaceDownCommunityCardCount = (int)args["faceDownCommunityCardCount"];
        RequiredHoleCardCountMin = (int)args["requiredHoleCardCountMin"];
        RequiredHoleCardCountMax = (int)args["requiredHoleCardCountMax"];
        HandEvaluator = new CompletedHandEvaluator(Configuration.CompletedHandCardCount, 
            RequiredHoleCardCountMin, RequiredHoleCardCountMax);

        var containerSetupArgs = new System.Collections.Generic.Dictionary<string, object>()
        {
            { "allowInteract", false },
            { "contentNodeSize", Configuration.CardSize },
            { "separation", Configuration.CardContainerSeparation },
            { "pivotDirection", Enums.Direction2D8Ways.Neutral },
            { "nodesPerRow", 0 },
            { "hasBorder", true },
            { "expectedContentNodeCount", 1 },
            { "hasName", true },
            { "containerName", "Community cards" },
            { "margins", Configuration.DefaultContentContainerMargins },
            { "withCardEffect", true }
        };

        containerSetupArgs["cards"] = CommunityCards;
        containerSetupArgs["containerName"] = "Community cards";
        containerSetupArgs["expectedContentNodeCount"] = Configuration.CommunityCardCount;
        containerSetupArgs["getCardFaceDirectionFunc"] = (Func<int, Enums.CardFace>)GetCommunityCardFaceDirectionFunc;
        CommunityCardContainer.Setup(containerSetupArgs);
        
        containerSetupArgs["defaultCardFaceDirection"] = Enums.CardFace.Up;
        
        containerSetupArgs["cards"] = ResolveCards;
        containerSetupArgs["containerName"] = "Resolve region";
        containerSetupArgs["expectedContentNodeCount"] = Configuration.CompletedHandCardCount;
        RuleCardContainer.Setup(containerSetupArgs);

        containerSetupArgs["cards"] = KickerCards;
        containerSetupArgs["containerName"] = "Kickers";
        containerSetupArgs["expectedContentNodeCount"] = 0;
        RuleCardContainer.Setup(containerSetupArgs);
        
        containerSetupArgs["allowInteract"] = true;
        
        containerSetupArgs["cards"] = Player.Equipments;
        containerSetupArgs["expectedInteractCardType"] = Enums.InteractCardType.Equipment;
        containerSetupArgs["containerName"] = "Equipments";
        containerSetupArgs["expectedContentNodeCount"] = 0;
        EquipmentCardContainer.Setup(containerSetupArgs);
        
        containerSetupArgs["cards"] = Player.Items;
        containerSetupArgs["expectedInteractCardType"] = Enums.InteractCardType.Item;
        containerSetupArgs["containerName"] = "Items";
        containerSetupArgs["expectedContentNodeCount"] = Player.ItemPocketSize;
        ItemCardContainer.Setup(containerSetupArgs);

        containerSetupArgs["cards"] = RuleCards;
        containerSetupArgs["expectedInteractCardType"] = Enums.InteractCardType.Rule;
        containerSetupArgs["containerName"] = "Rules";
        containerSetupArgs["expectedContentNodeCount"] = 0;
        RuleCardContainer.Setup(containerSetupArgs);
        
        var entitiesSetupArgs = (List<System.Collections.Generic.Dictionary<string, object>>)args["entities"];
        for (int i = 0; i < Entities.Length; i++)
        {
            var entity = Entities[i];
            entity.Setup(entitiesSetupArgs[i]);
            entity.OnDefeated += OnEntityDefeated;
        }
        
        Dealer.Setup(new System.Collections.Generic.Dictionary<string, object>()
        {
            { "sourceDecks" , Entities.Select(e => e.Deck).ToList() },
            { "excludedCards" , null }
        });

        foreach (var handTierValue in Enum.GetValues(typeof(Enums.HandTier)))
        {
            HandTierOrderAscend.Add((Enums.HandTier)handTierValue);
        }
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

    public async void Start()
    {
        Reset();
        Dealer.Shuffle();
        await Dealer.DealEquipmentCards();
        await NewRound();
        CurrentState = State.BeforeDealCards;
    }

    public async void Proceed()
    {
        switch (CurrentState)
        {
            case State.BeforeDealCards:
                DealCards();
                CurrentState = State.BeforeShowDown;
                break;
            case State.BeforeShowDown:
                ShowDown();
                CurrentState = State.BeforeResolve;
                break;
            case State.BeforeResolve:
                RoundEngage.Resolve();
                CurrentState = State.AfterResolve;
                break;
            case State.AfterResolve:
                OnRoundEnd?.Invoke(this);
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
                        await Dealer.AnimateShuffle();
                        var selectRewardCard = GameMgr.OverlayScene(SelectRewardCardScene) as SelectRewardCard;
                        selectRewardCard.OnQuit += AfterSelectRewardCard;
                        selectRewardCard.Setup(new System.Collections.Generic.Dictionary<string, object>()
                        {
                            { "rewardCardCount", Configuration.DefaultRewardCardCount },
                            { "rewardCardDefType", typeof(InteractCardDef) },
                            { "reRollPrice", Configuration.DefaultReRollPrice },
                            { "reRollPriceIncrease", Configuration.DefaultReRollPriceIncrease },
                            { "skipReward", Configuration.DefaultSkipReward }
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
                var card = cardNode.Content.Value;
                if (card is BaseInteractCard interactCard)
                {
                    var sourceContainer = (CardContainer)cardNode.Container.Value;
                    switch (interactCard.InteractCardDef.Type)
                    {
                        case Enums.InteractCardType.Equipment:
                            tasks.Add(sourceContainer.MoveCardNodeToContainer(cardNode, EquipmentCardContainer));
                            break;
                        case Enums.InteractCardType.Item:
                            if (ItemCardContainer.ContentNodes.Count >= Player.ItemPocketSize.Value)
                            {
                                tasks.Add(Dealer.AnimateDiscard(ItemCardContainer.ContentNodes[0]));
                            }
                            tasks.Add(sourceContainer.MoveCardNodeToContainer(cardNode, ItemCardContainer));
                            break;
                        case Enums.InteractCardType.Rule:
                            tasks.Add(sourceContainer.MoveCardNodeToContainer(cardNode, RuleCardContainer));
                            break;
                    }
                }
                else
                {
                    tasks.Add(Dealer.AnimateDiscard(cardNode));
                }
                await Utils.Wait(this, Configuration.AnimateCardTransformInterval);
            }
            await Task.WhenAll(tasks);
        }
        var tasks = new List<Task>();
        foreach (var entity in Entities)
        {
            tasks.Add(DiscardCards(entity.HoleCardContainer.ContentNodes.ToList()));
        }
        tasks.Add(DiscardCards(CommunityCardContainer.ContentNodes.ToList()));
        tasks.Add(DiscardCards(ResolveCardContainer.ContentNodes.ToList()));
        tasks.Add(DiscardCards(KickerCardContainer.ContentNodes.ToList()));
        RoundCount++;
        OnRoundStart?.Invoke(this);
        RoundHands.Clear();
        RoundEngage = null;
        await Task.WhenAll(tasks);
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

        tasks.Add(DiscardCards(EquipmentCardContainer.ContentNodes.ToList()));
        tasks.Add(DiscardCards(ItemCardContainer.ContentNodes.ToList()));
        tasks.Add(DiscardCards(RuleCardContainer.ContentNodes.ToList()));
        Dealer.Reset();
    }

    public async void DealCards()
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
    
    public void ShowDown()
    {
        BeforeShowDown?.Invoke(this);
        
        // var startTime = Time.GetTicksUsec();

        var validCommunityCards = GetValidCards(CommunityCards);
        foreach (var entity in Entities)
        {
            var validHoleCards = GetValidCards(entity.HoleCards);
            var bestHand = HandEvaluator.EvaluateBestHand(validCommunityCards, validHoleCards);
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
    
    protected List<BaseCard> GetValidCards(IEnumerable<BaseCard> cards)
    {
        var res = new List<BaseCard>();
        
        return res;
    }

    protected Enums.CardFace GetCommunityCardFaceDirectionFunc(int i)
    {
        return i < DealCommunityCardCount - FaceDownCommunityCardCount ? Enums.CardFace.Up : Enums.CardFace.Down;
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
        Enemy.Setup(BattleEntity.InitArgs(BattleEntityDefs.DefaultEnemyBattleEntityDef));
        OnNewEnemy?.Invoke(this, Enemy);
    }

    protected void AfterSelectRewardCard()
    {
        NewChallenger();
        Start();
    }
}