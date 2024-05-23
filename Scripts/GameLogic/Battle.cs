using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Godot;
using XCardGame.Scripts.Buffs;
using XCardGame.Scripts.Cards;
using XCardGame.Scripts.Common.Constants;
using XCardGame.Scripts.Common.DataBinding;
using XCardGame.Scripts.Effects;
using XCardGame.Scripts.HandEvaluate;
using XCardGame.Scripts.Nodes;

namespace XCardGame.Scripts.GameLogic;

public class Battle: Control, ISetup, IManagedUi
{
    [Export] public DealingDeck DealingDeck;
    [Export] public CardContainer CommunityCardContainer;
    [Export] public CardContainer FieldCardContainer;
    [Export] public PlayerBattleEntity Player;
    [Export] public List<BattleEntity> Entities;
    [Export] public Label CurrentEnergyLabel;
    [Export] public Label MaxEnergyLabel;
    [Export] public BaseButton ProceedButton;
    
    [Export]
    public string Identifier { get; set; }
    public GameMgr GameMgr { get; private set; }
    public UiMgr UiMgr { get; private set; }
    
    public bool HasSetup { get; set; }
    
    public Action<Battle> AfterDealCards;
    public Action<Battle> OnRoundStart;
    public Action<Battle> OnRoundEnd;
    public Action<Battle> BeforeShowDown;
    public Action<Battle> BeforeEngage;
    public Action<Battle, Attack> BeforeApplyDamage;
    public Action<Battle> OnBattleFinished;
    
    public CompletedHandEvaluator HandEvaluator;
    
    public int DealCommunityCardCount;
    public int FaceDownCommunityCardCount;
    
    public int RoundCount;
    public Dictionary<BattleEntity, CompletedHand> RoundHandStrengths;
    public Dictionary<BattleEntity, CompletedHand> RoundHandStrengthsWithoutFaceDownCards;
    
    public List<BaseEffect> Effects;
    
    public override void _Ready()
    {
        HasSetup = false;
        RoundHandStrengths = new Dictionary<BattleEntity, CompletedHand>();
        RoundHandStrengthsWithoutFaceDownCards = new Dictionary<BattleEntity, CompletedHand>();
        HandEvaluator = new CompletedHandEvaluator(Configuration.CompletedHandCardCount, Configuration.DefaultRequiredHoleCardCountMin, Configuration.DefaultRequiredHoleCardCountMax);
    }
    
    public override void _ExitTree()
    {
        base._ExitTree();
        if (HasSetup && Player != null)
        {
            Player.Energy.DetailedValueChanged -= OnPlayerEnergyChanged;
            Player.MaxEnergy.DetailedValueChanged -= OnPlayerMaxEnergyChanged;
        }
    }

    public virtual void Setup(Dictionary<string, object> args)
    {
        GameMgr = (GameMgr)args["gameMgr"];
        DealCommunityCardCount = (int)args["dealCommunityCardCount"];
        FaceDownCommunityCardCount = (int)args["faceDownCommunityCardCount"];
        
        DealingDeck.Setup(new Dictionary<string, object>()
        {
            { "sourceDecks" , Entities.Select(e => e.Deck).ToList() },
            { "excludedCards" , null }
        });
        
        CommunityCardContainer.Setup(new Dictionary<string, object>()
        {
            { "cards", new ObservableCollection<BaseCard>() },
            { "getCardFaceDirectionFunc", (Func<int, Enums.CardFace>)GetCommunityCardFaceDirectionFunc }
        });
        
        FieldCardContainer.Setup(new Dictionary<string, object>()
        {
            { "cards", new ObservableCollection<BaseCard>() }
        });
        
        // Player.Setup((Dictionary<string, object>)args["playerSetupArgs"]);
        // var iEnemy = 0;
        // foreach (var entity in Entities)
        // {
        //     if (entity is PlayerBattleEntity) continue;
        //     entity.Setup(((List<Dictionary<string, object>>)args["enemySetupArgs"])[iEnemy]);
        //     iEnemy++;
        // }
        
        foreach (var entity in Entities)
        {
            entity.OnDefeated += OnEntityDefeated;
        }
        
        Player.Energy.DetailedValueChanged += OnPlayerEnergyChanged;
        Player.Energy.FireValueChangeEventsOnInit();
        Player.MaxEnergy.DetailedValueChanged += OnPlayerMaxEnergyChanged;
        Player.MaxEnergy.FireValueChangeEventsOnInit();
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
        DealingDeck.Shuffle();
        NewRound();
    }
    
    public void NewRound()
    {
        RoundCount++;
        OnRoundStart?.Invoke(this);
        foreach (var entity in Entities)
        {
            entity.RoundReset();
        }
        CommunityCardContainer.ClearContents();
        RoundHandStrengths.Clear();
        RoundHandStrengthsWithoutFaceDownCards.Clear();
        DealCards();
    }
    
    public void Reset()
    {
        RoundCount = 0;
        foreach (var entity in Entities)
        {
            entity.Reset();
        }
        CommunityCardContainer.ClearContents();
        FieldCardContainer.ClearContents();
        DealingDeck.Reset();
    }

    public void DealCards()
    {
        foreach (var entity in Entities)
        {
            for (int i = 0; i < entity.DealCardCount; i++)
            {
                DealingDeck.DealCardAppend(entity.HoleCardContainer);
            }
        }
        for (int i = 0; i < DealCommunityCardCount; i++)
        {
            DealingDeck.DealCardAppend(CommunityCardContainer);
        }
        AfterDealCards?.Invoke(this);
    }
    
    public void ShowDown()
    {
        BeforeShowDown?.Invoke(this);
        
        // var startTime = Time.GetTicksUsec();

        foreach (var entity in Entities)
        {
            var (bestHand, bestHandWithoutFaceDownCards) = 
                HandEvaluator.EvaluateBestHandsWithAndWithoutFaceDownCards(CommunityCardContainer.Contents.ToList(),
                    entity.HoleCardContainer.Contents.ToList());
            RoundHandStrengthsWithoutFaceDownCards.Add(entity, bestHandWithoutFaceDownCards);
            RoundHandStrengths.Add(entity, bestHand);
        }

        foreach (var entity in Entities)
        {
            foreach (var card in entity.HoleCardContainer.Contents)
            {
                card.Node.TweenFlip(Enums.CardFace.Up, Configuration.FlipTweenTime);
            }
        }
        foreach (var card in CommunityCardContainer.Contents)
        {
            card.Node.TweenFlip(Enums.CardFace.Up, Configuration.FlipTweenTime);
        }

        BeforeEngage?.Invoke(this);
        
        // TODO: Engage needs rework
        for (int i = 0; i < Entities.Count; i++)
        {
            var entity = Entities[i];
            for (int j = i + 1; j < Entities.Count; j++)
            {
                var otherEntity = Entities[j];
                if (entity.FactionId == otherEntity.FactionId)
                {
                    continue;
                }
                var handStrength = RoundHandStrengths[entity];
                var handStrengthWithoutFaceDownCard = RoundHandStrengthsWithoutFaceDownCards[entity];
                var otherHandStrength = RoundHandStrengths[otherEntity];
                var otherHandStrengthWithoutFaceDownCard = RoundHandStrengthsWithoutFaceDownCards[otherEntity];
                
                if (HandEvaluator.Compare(handStrength, otherHandStrength) >= 0)
                {
                    Attack attack = new Attack(GameMgr, entity, otherEntity, handStrength, 
                        handStrengthWithoutFaceDownCard, otherHandStrength,
                        otherHandStrengthWithoutFaceDownCard);
                    BeforeApplyDamage?.Invoke(this, attack);
                    attack.Apply();
                }
                if (HandEvaluator.Compare(handStrength, otherHandStrength) <= 0)
                {
                    Attack attack = new Attack(GameMgr, otherEntity, entity, otherHandStrength,
                        otherHandStrengthWithoutFaceDownCard, handStrength, 
                        handStrengthWithoutFaceDownCard);
                    BeforeApplyDamage?.Invoke(this, attack);
                    attack.Apply();
                }
            }
        }
        OnRoundEnd?.Invoke(this);
        // var endTime = Time.GetTicksUsec();
        // GD.Print($"Hand evaluation time: {endTime - startTime} us");
        // GD.Print($"{Players[0]} Best Hand: {playerBestHand.Rank}, {string.Join(",", playerBestHand.PrimaryCards)}, Kickers: {string.Join(",", playerBestHand.Kickers)}");
        // GD.Print($"{Players[1]} Best Hand: {opponentBestHand.Rank}, {string.Join(",", opponentBestHand.PrimaryCards)}, Kickers: {string.Join(",", opponentBestHand.Kickers)}");
    }

    public void InflictBuffOn(BaseBuff buff, BattleEntity target)
    {
        target.BuffContainer.Contents.Add(buff);
    }

    public void OnEntityDefeated(BattleEntity e)
    {
        GD.Print($"{e} defeated");
        if (e == Player)
        {
            GD.Print($"You lose");
            OnBattleFinished?.Invoke(this);
        }
        else
        {
            Entities.Remove(e);
            if (Entities.Count == 1)
            {
                GD.Print($"{Entities[0]} wins");
                OnBattleFinished?.Invoke(this);
            }
        }
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
    
    protected void OnPlayerEnergyChanged(object sender, ValueChangedEventDetailedArgs<int> args)
    {
        CurrentEnergyLabel.Text = args.NewValue.ToString();
    }
	
    protected void OnPlayerMaxEnergyChanged(object sender, ValueChangedEventDetailedArgs<int> args)
    {
        MaxEnergyLabel.Text = args.NewValue.ToString();
    }

    protected Enums.CardFace GetCommunityCardFaceDirectionFunc(int i)
    {
        return i < DealCommunityCardCount - FaceDownCommunityCardCount ? Enums.CardFace.Up : Enums.CardFace.Down;
    }

}