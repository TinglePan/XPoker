using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Godot;
using XCardGame.Common;
using XCardGame.TimingInterfaces;
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

    public PackedScene GameOverScene;
    public PackedScene GameWinScene;
    public PackedScene SelectRewardCardScene;
    
    public Action<Battle, CardNode> OnDealCard;
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
    public int FirstFlipCommunityCardCount;
    public bool AllowLastFlip;
    public int LastFlipCommunityCardCount;
    public int RequiredHoleCardCountMin;
    public int RequiredHoleCardCountMax;
    public bool KeepHeatMultiplierOnNewRound;
    
    
    public int RoundCount;
    public Dictionary<BattleEntity, CompletedHand> RoundHands;
    public Engage RoundEngage;
    public int CurrentFaceUpCommunityCardCount => CommunityCardContainer.ContentNodes.Count(x => ((CardNode)x).FaceDirection.Value == Enums.CardFace.Up);

    public ObservableCollection<Enums.HandTier> HandTierOrderDescend;
    
    public List<BaseEffect> Effects;
    public ObservableProperty<State> CurrentState;
    public float HeatMultiplier;

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
        CurrentState = new ObservableProperty<State>(nameof(CurrentState), this, State.BeforeDealCards);
        Effects = new List<BaseEffect>();
        ButtonRoot = GetNode<Node2D>("Buttons");
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
        
        var engageCardContainerSetupArgs = new SplitCardContainer.SetupArgs
        {
            CardContainersSetupArgs = new List<CardContainer.SetupArgs>
            {
                containerSetupArgs,
                containerSetupArgs,
            },
            Separation = Configuration.SplitCardContainerSeparation,
            PivotDirection = Enums.Direction2D8Ways.Neutral,
        };
        
        EngageCardContainer.Setup(engageCardContainerSetupArgs);
        
        containerSetupArgs.DefaultCardFaceDirection = Enums.CardFace.Down;
        containerSetupArgs.ShouldCollectDealtItemAndRuleCards = true;
        CommunityCardContainer.Setup(containerSetupArgs);
        containerSetupArgs.DefaultCardFaceDirection = Enums.CardFace.Up;
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

    public async void Start()
    {
        Reset();
        await GameMgr.AwaitAndDisableInput(Dealer.DealInnateCards());
        await GameMgr.AwaitAndDisableInput(NewRound());
        Dealer.Shuffle();
        CurrentState.Value = State.BeforeDealCards;
        HeatMultiplier = 1.0f;
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
        foreach (var entity in Entities)
        {
            await entity.RoundReset();
        }
        
        await RemoveCards(CommunityCardContainer.CardNodes);
        foreach (var cardContainer in EngageCardContainer.CardContainers)
        {
            await RemoveCards(cardContainer.CardNodes);
        }
        RoundCount++;
        OnRoundStart?.Invoke(this);
        RoundHands.Clear();
        RoundEngage = null;
        HeatMultiplier = KeepHeatMultiplierOnNewRound ? HeatMultiplier : 1.0f;
    }
    
    public void Reset()
    {
        RoundCount = 0;
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
        HeatMultiplier += Configuration.AllFaceDownHeatMultiplierAdd;
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
        AfterDealCards?.Invoke(this);
    }

    public bool CanFlipCards()
    {
        if (CommunityCardContainer.ContentNodes.Count - CurrentFaceUpCommunityCardCount > LastFlipCommunityCardCount)
        {
            return true;
        }
        if (AllowLastFlip) return true;
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
            HeatMultiplier -= Configuration.AllFaceDownHeatMultiplierAdd;
            flipCount = FirstFlipCommunityCardCount;
        } else if (faceDownCommunityCardCount <= LastFlipCommunityCardCount)
        {
            HeatMultiplier = Configuration.AllFlipHeatMultiplier;
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
            }
        }
        await Task.WhenAll(tasks);
    }

    public async Task Fold()
    {
        var tasks = new List<Task>();
        HeatMultiplier = Configuration.FoldHeatMultiplier;
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
        BeforeShowDown?.Invoke(this);
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
        BeforeEngage?.Invoke(this, RoundEngage);
        await Task.WhenAll(tasks);
        // RoundEngage.PrepareRoundSkills();
        // GameMgr.InputMgr.SwitchToInputHandler(new PrepareRoundSkillInputHandler(GameMgr));
        // var endTime = Time.GetTicksUsec();
        // GD.Print($"Hand evaluation time: {endTime - startTime} us");
        // GD.Print($"{Players[0]} Best Hand: {playerBestHand.Rank}, {string.Join(",", playerBestHand.PrimaryCards)}, Kickers: {string.Join(",", playerBestHand.Kickers)}");
        // GD.Print($"{Players[1]} Best Hand: {opponentBestHand.Rank}, {string.Join(",", opponentBestHand.PrimaryCards)}, Kickers: {string.Join(",", opponentBestHand.Kickers)}");
    }

    public async Task RoundEnd()
    {
        OnRoundEnd?.Invoke(this);
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
            await GameMgr.AwaitAndDisableInput(((CardNode)ItemCardContainer.ContentNodes[0]).AnimateLeaveField());
        }
    }
}