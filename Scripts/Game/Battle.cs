using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Godot;
using XCardGame.CardProperties;
using XCardGame.Common;
using XCardGame.Ui;

// using Godot.Collections;

namespace XCardGame;

public partial class Battle: Node2D
{

    public class SetupArgs
    {
        public int DealCommunityCardCount;
        public int FirstFlipCommunityCardCount;
        public bool AllowLastFlip;
        public int LastFlipCommunityCardCount;
        public bool KeepHeatMultiplierOnNewRound;
        public int RequiredHoleCardCountMin;
        public int RequiredHoleCardCountMax;
        public List<BattleEntity.SetupArgs> EntitySetupArgs;
        
    }

    public enum State
    {
        BeforeDealCards,
        BeforeShowDown,
        BeforeEngage,
        AfterEngage,
    }
    
    public GameMgr GameMgr;
    public Dealer Dealer;
    public CardContainer CommunityCardContainer;
    public SplitCardContainer EngageCardContainer;
    public CardContainer ItemCardContainer;
    public CardContainer RuleCardContainer;
    public PlayerBattleEntity Player;
    public BattleEntity Enemy;
    public BattleEntity[] Entities;
    public Node2D ButtonRoot;
    public SplitCardContainer OpenedPiledCardContainer;
    public Label Info;

    public PackedScene GameOverScene;
    public PackedScene GameWinScene;
    public PackedScene SelectRewardCardScene;
    public PackedScene CardPrefab;
    public PackedScene PiledCardPrefab;
    
    public Action<CardNode> OnDealCard;
    public Action AfterDealCards;
    public Action OnRoundStart;
    public Action OnRoundEnd;
    public Action BeforeShowDown;
    public Action <Engage> BeforeEngage;
    public Action OnPlayerDefeated;
    public Action OnEnemyDefeated;
    public Action <BattleEntity> OnNewEnemy;
    public Action OnBattleFinished;

    // public ObservableCollection<BaseCard> CommunityCards;
    // public ObservableCollection<BaseCard> PrimaryCards;
    // public ObservableCollection<BaseCard> KickerCards;
    // public ObservableCollection<BaseCard> RuleCards;
    
    public CompletedHandEvaluator HandEvaluator;
    
    public int DealCommunityCardCount;
    public int FirstFlipCommunityCardCount;
    public bool AllowLastFlip;
    public int LastFlipCommunityCardCount;
    public int RequiredHoleCardCountMin;
    public int RequiredHoleCardCountMax;
    public bool KeepHeatMultiplierOnNewRound;
    
    
    public ObservableProperty<int> RoundCount;
    public Dictionary<BattleEntity, CompletedHand> RoundHands;
    public Engage RoundEngage;
    public int CurrentFaceUpCommunityCardCount;

    public ObservableCollection<Enums.HandTier> HandTierOrderDescend;
    
    public List<BaseFieldEffect> FieldEffects;
    public ObservableProperty<State> CurrentState;
    public ObservableProperty<int> HeatMultiplier;

    protected List<EnemyRandBoxDef> AllEnemyRandBoxDefs;
    
    public override void _Ready()
    {
        base._Ready();
        GameMgr = GetNode<GameMgr>("/root/GameMgr");
        Dealer = GetNode<Dealer>("Dealer");
        CommunityCardContainer = GetNode<CardContainer>("CommunityCards");
        EngageCardContainer = GetNode<SplitCardContainer>("EngageCards");
        ItemCardContainer = GetNode<CardContainer>("ItemCards");
        RuleCardContainer = GetNode<CardContainer>("RuleCards");
        OpenedPiledCardContainer = GetNode<SplitCardContainer>("OpenedPiledCard");
        Info = GetNode<Label>("Info/Label");
        
        Player = GetNode<PlayerBattleEntity>("Player");
        Enemy = GetNode<BattleEntity>("Enemy");
        GameOverScene = ResourceCache.Instance.Load<PackedScene>("res://Scenes/GameOver.tscn");
        GameWinScene = ResourceCache.Instance.Load<PackedScene>("res://Scenes/GameWin.tscn");
        SelectRewardCardScene = ResourceCache.Instance.Load<PackedScene>("res://Scenes/SelectRewardCard.tscn");
        CardPrefab = ResourceCache.Instance.Load<PackedScene>("res://Scenes/Card.tscn");
        PiledCardPrefab = ResourceCache.Instance.Load<PackedScene>("res://Scenes/PiledCard.tscn");
        
        Entities = new [] { Player, Enemy };
        
        RoundHands = new Dictionary<BattleEntity, CompletedHand>();
        // CommunityCards = new ObservableCollection<BaseCard>();
        // PrimaryCards = new ObservableCollection<BaseCard>();
        // KickerCards = new ObservableCollection<BaseCard>();
        // RuleCards = new ObservableCollection<BaseCard>();
        HandTierOrderDescend = new ObservableCollection<Enums.HandTier>();
        CurrentState = new ObservableProperty<State>(nameof(CurrentState), this, State.BeforeDealCards);
        HeatMultiplier = new ObservableProperty<int>(nameof(HeatMultiplier), this, 100);
        HeatMultiplier.DetailedValueChanged += OnInfoChanged;
        RoundCount = new ObservableProperty<int>(nameof(RoundCount), this, 0);
        RoundCount.DetailedValueChanged += OnInfoChanged;
        FieldEffects = new List<BaseFieldEffect>();
        ButtonRoot = GetNode<Node2D>("Buttons");
        GameMgr.ProgressCounter.DetailedValueChanged += OnInfoChanged;
        
        GameMgr.ProgressCounter.FireValueChangeEventsOnInit();
    }
    
    public override void _ExitTree()
    {
        base._ExitTree();
        if (GameMgr != null)
        {
            GameMgr.ProgressCounter.DetailedValueChanged -= OnInfoChanged;
        }
    }

    public virtual void Setup(object o)
    {
        var args = (SetupArgs)o;
        DealCommunityCardCount = args.DealCommunityCardCount;
        FirstFlipCommunityCardCount = args.FirstFlipCommunityCardCount;
        AllowLastFlip = args.AllowLastFlip;
        LastFlipCommunityCardCount = args.LastFlipCommunityCardCount;
        RequiredHoleCardCountMin = args.RequiredHoleCardCountMin;
        RequiredHoleCardCountMax = args.RequiredHoleCardCountMax;
        KeepHeatMultiplierOnNewRound = args.KeepHeatMultiplierOnNewRound;
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
        
        var splitCardContainerSetupArgs = new SplitCardContainer.SetupArgs
        {
            CardContainersSetupArgs = new List<CardContainer.SetupArgs>
            {
                containerSetupArgs,
                containerSetupArgs,
            },
            Separation = Configuration.SplitCardContainerSeparation,
            PivotDirection = Enums.Direction2D8Ways.Neutral,
        };
        
        EngageCardContainer.Setup(splitCardContainerSetupArgs);

        splitCardContainerSetupArgs.CardContainersSetupArgs[0].AllowUseItemCard = true;
        splitCardContainerSetupArgs.CardContainersSetupArgs[1].AllowUseItemCard = true;
        OpenedPiledCardContainer.Setup(splitCardContainerSetupArgs);
        
        containerSetupArgs.ShouldCollectDealtItemAndRuleCards = true;
        containerSetupArgs.GetCardFaceDirectionFunc = GetCommunityCardContainerCardFaceDirection;
        CommunityCardContainer.Setup(containerSetupArgs);
        containerSetupArgs.GetCardFaceDirectionFunc = null;
        containerSetupArgs.ShouldCollectDealtItemAndRuleCards = false;
        
        containerSetupArgs.AllowUseItemCard = true;
        ItemCardContainer.Setup(containerSetupArgs);
        ItemCardContainer.OnAddContentNode += OnItemCardContainerAddNode;
        containerSetupArgs.AllowUseItemCard = false;

        containerSetupArgs.AllowUseRuleCard = true;
        RuleCardContainer.Setup(containerSetupArgs);
        containerSetupArgs.AllowUseRuleCard = false;


        Dealer.Setup(new Dealer.SetupArgs
        {
            SourceDecks = Entities.Select(e => e.Deck).ToList(),
        });

        foreach (var handTierValue in ((Enums.HandTier[])Enum.GetValues(typeof(Enums.HandTier))).Reverse())
        {
            HandTierOrderDescend.Add(handTierValue);
        }

        AllEnemyRandBoxDefs = EnemyRandBoxDefs.All();
        CurrentState.FireValueChangeEventsOnInit();
        
        Reset();
    }

    public CardNode InstantiateCardNode(BaseCard card, Node parent)
    {
        if (card.GetProp<CardPropPiled>() != null)
        {
            return Utils.InstantiatePrefab(PiledCardPrefab, parent) as CardNode;
        }
        return Utils.InstantiatePrefab(CardPrefab, parent) as CardNode;
    }

    public async void Start()
    {
        
        void CallOnBattleStart(IBattleStart battleStart)
        {
            battleStart.OnBattleStart();
        }
        
        Reset();
        await GameMgr.AwaitAndDisableInput(Dealer.DealInnateCards());
        await GameMgr.AwaitAndDisableInput(NewRound());
        Dealer.Shuffle();
        CurrentState.Value = State.BeforeDealCards;
        HeatMultiplier.Value = 100;
        
        CheckTiming<IBattleStart>(CallOnBattleStart);
    }
    
    public async Task NewRound()
    {
        async Task RemoveCards(List<CardNode> cardNodes)
        {
            var tasks = new List<Task>();
            foreach (var cardNode in cardNodes)
            {
                tasks.Add(cardNode.AnimateLeaveField());
                await Utils.Wait(this, Configuration.AnimateCardTransformInterval);
                // GD.Print($"time: {Time.GetTicksMsec()}");
            }
            await Task.WhenAll(tasks);
        }
        void CallOnRoundStart(IRoundStart roundStart)
        {
            roundStart.OnRoundStart();
        }
        
        foreach (var entity in Entities)
        {
            await entity.RoundReset();
        }
        await RemoveCards(CommunityCardContainer.CardNodes);
        foreach (var cardContainer in EngageCardContainer.CardContainers)
        {
            await RemoveCards(cardContainer.CardNodes);
        }
        RoundCount.Value++;
        RoundHands.Clear();
        RoundEngage = null;
        CurrentFaceUpCommunityCardCount = 0;
        if (!KeepHeatMultiplierOnNewRound)
        {
            HeatMultiplier.Value = 100;
        }
        OnRoundStart?.Invoke();
        CheckTiming<IRoundStart>(CallOnRoundStart);
    }
    
    public void Reset()
    {
        RoundCount.Value = 0;
        CurrentState.Value = State.BeforeDealCards;
        foreach (var entity in Entities)
        {
            entity.Reset();
        }
        Dealer.Reset();
        CommunityCardContainer.ContentNodes.Clear();
        EngageCardContainer.Reset();
        ItemCardContainer.ContentNodes.Clear();
        RuleCardContainer.ContentNodes.Clear();
    }

    public async Task DealCards()
    {
        var tasks = new List<Task>();
        HeatMultiplier.Value += Configuration.AllFaceDownHeatMultiplierAdd;
        foreach (var entity in Entities)
        {
            for (int i = 0; i < entity.DealCardCount; i++)
            {
                tasks.Add(Dealer.DrawCardIntoContainer(entity.HoleCardContainer));
                await Utils.Wait(this, Configuration.AnimateCardTransformInterval);
            }
        }
        for (int i = 0; i < DealCommunityCardCount; i++)
        {
            tasks.Add(Dealer.DrawCardIntoContainer(CommunityCardContainer));
            await Utils.Wait(this, Configuration.AnimateCardTransformInterval);
        }
        await Task.WhenAll(tasks);
        AfterDealCards?.Invoke();
    }

    public bool IsFieldContainer(CardContainer container)
    {
        if (container == CommunityCardContainer || container == ItemCardContainer || container == RuleCardContainer) return true;
        if (container == Player.HoleCardContainer || container == Enemy.HoleCardContainer) return true;
        if (EngageCardContainer.CardContainers.Contains(container)) return true;
        if (OpenedPiledCardContainer.CardContainers.Contains(container)) return true;
        return false;
    }

    public bool CanFlipCards()
    {
        if (CommunityCardContainer.ContentNodes.Count - CurrentFaceUpCommunityCardCount > LastFlipCommunityCardCount)
        {
            return true;
        } 
        if (CommunityCardContainer.ContentNodes.Count - CurrentFaceUpCommunityCardCount == LastFlipCommunityCardCount && AllowLastFlip)
        {
            return true;
        }
        return false;
    }

    public async Task FlipCards()
    {
        var tasks = new List<Task>();
        var faceUpCommunityCardCount = CurrentFaceUpCommunityCardCount;
        var faceDownCommunityCardCount = CommunityCardContainer.ContentNodes.Count - faceUpCommunityCardCount;
        int flipCount;
        if (faceUpCommunityCardCount == 0)
        {
            HeatMultiplier.Value -= Configuration.AllFaceDownHeatMultiplierAdd;
            flipCount = FirstFlipCommunityCardCount;
        } else if (faceDownCommunityCardCount <= LastFlipCommunityCardCount)
        {
            HeatMultiplier.Value = Configuration.AllFlipHeatMultiplier;
            flipCount = Mathf.Min(faceDownCommunityCardCount, LastFlipCommunityCardCount);
        }
        else
        {
            flipCount = 1;
        }
        foreach (var cardNode in CommunityCardContainer.CardNodes)
        {
            if (flipCount <= 0) break;
            if (cardNode.FaceDirection.Value == Enums.CardFace.Down)
            {
                tasks.Add(cardNode.AnimateFlip(Enums.CardFace.Up));
                flipCount--;
                CurrentFaceUpCommunityCardCount++;
            }
        }
        await Task.WhenAll(tasks);
    }

    public async Task Fold()
    {
        var tasks = new List<Task>();
        HeatMultiplier.Value = Configuration.FoldHeatMultiplier;
        foreach (var cardNode in Player.HoleCardContainer.CardNodes)
        {
            if (cardNode.FaceDirection.Value == Enums.CardFace.Up)
            {
                tasks.Add(cardNode.AnimateFlip(Enums.CardFace.Down));
            }
        }
        foreach (var cardNode in Enemy.HoleCardContainer.CardNodes)
        {
            if (cardNode.FaceDirection.Value == Enums.CardFace.Down)
            {
                tasks.Add(cardNode.AnimateFlip(Enums.CardFace.Up));
            }
        }
        await Task.WhenAll(tasks);
    }

    public async Task ShowDown()
    {
        BeforeShowDown?.Invoke();
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
        await Task.WhenAll(tasks);
    }
    
    public async Task Engage()
    {
        // var startTime = Time.GetTicksUsec();
        // var validCommunityCards = GetValidCards(CommunityCards);
        foreach (var entity in Entities)
        {
            // var validHoleCards = GetValidCards(entity.HoleCards);
            var bestHand = HandEvaluator.EvaluateBestHand(CommunityCardContainer.FaceUpCards(), entity.HoleCardContainer.FaceUpCards(), HandTierOrderDescend.ToList());
            entity.RoundHandTier.Value = bestHand.Tier;
            RoundHands.Add(entity, bestHand);
        }

        var tasks = new List<Task>();
        var playerHand = RoundHands[Player];
        var enemyHand = RoundHands[Enemy];
        RoundEngage = new Engage(GameMgr, playerHand, enemyHand);
        tasks.Add(GameMgr.BattleLog.LogAndWait(
            Utils._($"Engage: {Player}({playerHand.Tier}) vs {Enemy}({enemyHand.Tier}). {Player} is {Player.RoundRole.Value}. {Enemy} is {Enemy.RoundRole.Value}"),
            Configuration.LogInterval));
        BeforeEngage?.Invoke(RoundEngage);
        tasks.Add(RoundEngage.Resolve());
        await Task.WhenAll(tasks);
        // var endTime = Time.GetTicksUsec();
        // GD.Print($"Hand evaluation time: {endTime - startTime} us");
        // GD.Print($"{Players[0]} Best Hand: {playerBestHand.Rank}, {string.Join(",", playerBestHand.PrimaryCards)}, Kickers: {string.Join(",", playerBestHand.Kickers)}");
        // GD.Print($"{Players[1]} Best Hand: {opponentBestHand.Rank}, {string.Join(",", opponentBestHand.PrimaryCards)}, Kickers: {string.Join(",", opponentBestHand.Kickers)}");
    }

    public async Task RoundEnd()
    {
        
        void CallOnRoundEnd(IRoundEnd roundEnd)
        {
            roundEnd.OnRoundEnd();
        }
        
        OnRoundEnd?.Invoke();
        CheckTiming<IRoundEnd>(CallOnRoundEnd);
        
        var isPlayerDefeated = Player.IsDefeated();
        var isEnemyDefeated = Enemy.IsDefeated();
        if (isEnemyDefeated)
        {
            if (GameMgr.ProgressCounter.Value >= Configuration.ProgressCountRequiredToWin)
            {
                GameWin();
            }
            else
            {
                // await GameMgr.AwaitAndDisableInput(Dealer.AnimateShuffle());
                var selectRewardCard = GameMgr.OverlayScene(SelectRewardCardScene) as SelectRewardCard;
                selectRewardCard.OnQuit += AfterSelectRewardCard;
                selectRewardCard.Setup(new SelectRewardCard.SetupArgs
                {
                    RewardCardCount = Configuration.DefaultRewardCardCount,
                    InitReRollPrice = Configuration.DefaultReRollPrice,
                    ReRollPriceIncrease = Configuration.DefaultReRollPriceIncrease,
                    SkipReward = Configuration.DefaultSkipReward
                });
            }
            return;
        }
        if (isPlayerDefeated)
        {
            GameOver();
            return;
        }
        await GameMgr.AwaitAndDisableInput(NewRound());
        CurrentState.Value = State.BeforeDealCards;
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
            OnPlayerDefeated?.Invoke();
        }
        else
        {
            GD.Print($"You win");
            Player.Credit.Value += Mathf.Abs(Player.Hp.Value - Player.MaxHp.Value / 2);
            GameMgr.ProgressCounter.Value++;
            OnEnemyDefeated?.Invoke();
        }
        OnBattleFinished?.Invoke();
    }

    public void InflictBuffOn(BaseBuff buff, BattleEntity target, BattleEntity source, BaseCard sourceCard = null)
    {
        buff.Setup(new BaseBuff.SetupArgs
        {
            GameMgr = GameMgr,
            Battle = this,
            Entity = target,
            InflictedBy = source,
            InflictedByCard = sourceCard,
        });
        target.AddBuff(buff);
    }

    public bool CheckResolveCard(BaseCard card)
    {
        foreach (var effect in FieldEffects)
        {
            if (effect is KeepOutCard.KeepOutEffect keepOutEffect)
            {
                if (keepOutEffect.KeepOutSuits.Contains(card.Suit.Value) || keepOutEffect.KeepOutRanks.Contains(card.Rank.Value))
                {
                    return false;
                }
            }
        }
        return true;
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
        var boxes = AllEnemyRandBoxDefs
            .Where(def => def.ProgressRange.X <= GameMgr.ProgressCounter.Value
                          && GameMgr.ProgressCounter.Value <= def.ProgressRange.Y).ToList();
        Debug.Assert(boxes.Count > 0, "No enemy box available");
        var i = Utils.RandOnWeight(boxes.Select(x => x.RandBox.RandBoxWeight).ToList(), GameMgr.Rand);
        var boxDef = boxes[i];
        var box = boxDef.RandBox;
        var entityDef = box.Rand(GameMgr.Rand);
        Enemy.Setup(BattleEntity.InitArgs(entityDef));
        OnNewEnemy?.Invoke(Enemy);
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
            await GameMgr.AwaitAndDisableInput(((CardNode)ItemCardContainer.ContentNodes[0]).AnimateLeaveField());
        }
    }

    protected void OnInfoChanged(object sender, ValueChangedEventDetailedArgs<int> args)
    {
        UpdateInfo();
    }

    protected void UpdateInfo()
    {
        
        Info.Text = $"Progress {GameMgr.ProgressCounter.Value}/{Configuration.ProgressCountRequiredToWin}\n" +
                    $"Round: {RoundCount.Value}\n" +
                    $"Heat {HeatMultiplier.Value}%";
    }

    protected Enums.CardFace GetCommunityCardContainerCardFaceDirection(int i)
    {
        if (CurrentState.Value >= State.BeforeEngage)
        {
            return Enums.CardFace.Up;
        }
        if (i < CurrentFaceUpCommunityCardCount)
        {
            return Enums.CardFace.Up;
        }
        return Enums.CardFace.Down;
    }

    protected void CheckTiming<T>(Action<T> action)
    {
        foreach (var effect in FieldEffects.ToList())
        {
            if (effect is T timing)
            {
                action(timing);
            }
        }

        foreach (var content in ItemCardContainer.Contents.ToList())
        {
            if (content is T timing)
            {
                action(timing);
            }
        }
        
        foreach (var content in RuleCardContainer.Contents.ToList())
        {
            if (content is T timing)
            {
                action(timing);
            }
        }
    }
}