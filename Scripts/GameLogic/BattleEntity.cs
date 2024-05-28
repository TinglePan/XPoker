using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Godot;
using XCardGame.Scripts.Buffs;
using XCardGame.Scripts.Cards;
using XCardGame.Scripts.Cards.AbilityCards;
using XCardGame.Scripts.Cards.SkillCards;
using XCardGame.Scripts.Common;
using XCardGame.Scripts.Common.Constants;
using XCardGame.Scripts.Common.DataBinding;
using XCardGame.Scripts.HandEvaluate;
using XCardGame.Scripts.Nodes;

namespace XCardGame.Scripts.GameLogic;

public partial class BattleEntity: Node, ISetup
{
    [Export] public CardContainer HoleCardContainer;
    [Export] public CardContainer SkillCardContainer;
    [Export] public BuffContainer BuffContainer;
    
    [Export] public Label NameLabel;
    [Export] public TextureRect Portrait;
    [Export] public Label HitPointLabel;
    [Export] public Label MaxHitPointLabel;
    
    public GameMgr GameMgr;
    public Battle Battle;
    
    public bool HasSetup { get; set; }
    
    public Action<BattleEntity> OnDefeated;
    
    public string DisplayName;
    public string PortraitPath;
    public Deck Deck;
    public int DealCardCount;
    public int ShowDownHoleCardCountMin;
    public int ShowDownHoleCardCountMax;
    public int FactionId;
    public Dictionary<Enums.HandTier, int> HandPowers;
    public ObservableProperty<int> HitPoint;
    public ObservableProperty<int> MaxHitPoint;
    public ObservableProperty<int> Level;
    public bool IsHoleCardDealtVisible;
    public ObservableCollection<BaseAbilityCard> AbilityCards;

    public override void _Ready()
    {
        base._Ready();
        HasSetup = false;
    }

    public virtual void Setup(Dictionary<string, object> args)
    {
        GameMgr = (GameMgr)args["gameMgr"];
        Battle = (Battle)args["battle"];
        
        DisplayName = (string)args["name"];
        PortraitPath = (string)args["portraitPath"];
        Deck = (Deck)args["deck"];
        foreach (var card in Deck.CardList)
        {
            card.Owner = this;
        }

        DealCardCount = (int)args["dealCardCount"];
        ShowDownHoleCardCountMin = (int)args["showDownHoleCardCountMin"];
        ShowDownHoleCardCountMax = (int)args["showDownHoleCardCountMax"];
        FactionId = (int)args["factionId"];
        HandPowers = (Dictionary<Enums.HandTier, int>)args["handPowers"];
        var maxHitPoint = (int)args["maxHitPoint"];
        HitPoint = new ObservableProperty<int>(nameof(HitPoint), this, maxHitPoint);
        MaxHitPoint = new ObservableProperty<int>(nameof(MaxHitPoint), this, maxHitPoint);
        Level = new ObservableProperty<int>(nameof(Level), this, (int)args["level"]);
        IsHoleCardDealtVisible = (bool)args["isHoleCardDealtVisible"];
        AbilityCards = (ObservableCollection<BaseAbilityCard>)args["abilityCards"];
        
        HoleCardContainer.Setup(new Dictionary<string, object>()
        {
            { "cards", new ObservableCollection<BaseCard>() },
            { "defaultCardFaceDirection", IsHoleCardDealtVisible ? Enums.CardFace.Up : Enums.CardFace.Down } 
        });
        SkillCardContainer.Setup(new Dictionary<string, object>()
        {
            { "cards", (ObservableCollection<BaseSkillCard>)args["skillCards"] }
        });
        BuffContainer.Setup(new Dictionary<string, object>()
        {
            { "buffs", new ObservableCollection<BaseBuff>() },
        });
        NameLabel.Text = DisplayName;
        Portrait.Texture = ResourceCache.Instance.Load<Texture2D>(PortraitPath);
        HitPoint.DetailedValueChanged += OnHitPointChanged;
        HitPoint.FireValueChangeEventsOnInit();
        MaxHitPoint.DetailedValueChanged += OnMaxHitPointChanged;
        MaxHitPoint.FireValueChangeEventsOnInit();
        
        HasSetup = true;
    }

    public void EnsureSetup()
    {
        if (!HasSetup)
        {
            GD.PrintErr($"{this} not setup yet");
        }
    }

    public virtual void Reset()
    {
        HitPoint.Value = MaxHitPoint.Value;
        HoleCardContainer.ClearContents();
        BuffContainer.ClearContents();
    }
    
    public virtual void RoundReset()
    {
        HoleCardContainer.ClearContents();
    }

    public override string ToString()
    {
        return DisplayName;
    }
    
    protected void OnHitPointChanged(object sender, ValueChangedEventDetailedArgs<int> args)
    {
        // TODO: Animate number change here 
        HitPointLabel.Text = args.NewValue.ToString();
        if (args.NewValue <= 0)
        {
            OnDefeated?.Invoke(this);
        }
    }
	
    protected void OnMaxHitPointChanged(object sender, ValueChangedEventDetailedArgs<int> args)
    {
        MaxHitPointLabel.Text = args.NewValue.ToString();
        if (HitPoint.Value > args.NewValue)
        {
            HitPoint.Value = args.NewValue;
        }
    }
}