using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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
    [Export] public Label HpLabel;
    [Export] public Label MaxHpLabel;
    
    public Action<BattleEntity, int> OnHpChanged;
    public Action<BattleEntity> OnDefeated;
    
    public GameMgr GameMgr;
    public Battle Battle;
    
    public bool HasSetup { get; set; }
    
    
    public string DisplayName;
    public string PortraitPath;
    public Deck Deck;
    public int DealCardCount;
    public int FactionId;
    public Dictionary<Enums.HandTier, int> HandPowers;
    public int BaseHandPower;
    public ObservableProperty<int> Hp;
    public ObservableProperty<int> MaxHp;
    public ObservableProperty<int> Level;
    public bool IsHoleCardDealtVisible;
    public ObservableCollection<BaseCard> SkillCards;
    public ObservableCollection<BaseCard> HoleCards;
    public ObservableCollection<BaseBuff> Buffs;
    public ObservableCollection<BaseCard> AbilityCards;

    public override void _Ready()
    {
        base._Ready();
        GameMgr = GetNode<GameMgr>("/root/GameMgr");
        HasSetup = false;
    }

    public virtual void Setup(Dictionary<string, object> args)
    {
        Battle = GameMgr.CurrentBattle;
        DisplayName = (string)args["name"];
        PortraitPath = (string)args["portraitPath"];
        Deck = (Deck)args["deck"];
        foreach (var card in Deck.CardList)
        {
            card.Owner = this;
        }

        DealCardCount = (int)args["dealCardCount"];
        FactionId = (int)args["factionId"];
        HandPowers = (Dictionary<Enums.HandTier, int>)args["handPowers"];
        BaseHandPower = (int)args["baseHandPower"];
        MaxHp = new ObservableProperty<int>(nameof(MaxHp), this, (int)args["maxHp"]);
        Hp = new ObservableProperty<int>(nameof(Hp), this, MaxHp.Value);
        Level = new ObservableProperty<int>(nameof(Level), this, (int)args["level"]);
        IsHoleCardDealtVisible = (bool)args["isHoleCardDealtVisible"];
        AbilityCards = (ObservableCollection<BaseCard>)args["abilityCards"];

        HoleCards = new ObservableCollection<BaseCard>();
        HoleCardContainer.Setup(new Dictionary<string, object>()
        {
            { "allowInteract", false },
            { "cards", HoleCards },
            { "contentNodeSize", Configuration.CardSize },
            { "separation", Configuration.CardContainerSeparation },
            { "defaultCardFaceDirection", IsHoleCardDealtVisible ? Enums.CardFace.Up : Enums.CardFace.Down } 
        });
        SkillCards = (ObservableCollection<BaseCard>)args["skillCards"];
        SkillCardContainer.Setup(new Dictionary<string, object>()
        {
            { "allowInteract", false },
            { "cards", SkillCards },
            { "contentNodeSize", Configuration.CardSize },
            { "separation", Configuration.CardContainerSeparation },
            { "defaultCardFaceDirection", Enums.CardFace.Up } 
        });
        Buffs = new ObservableCollection<BaseBuff>();
        BuffContainer.Setup(new Dictionary<string, object>()
        {
            { "allowInteract", false },
            { "buffs", Buffs },
            { "contentNodeSize", Configuration.CardSize },
            { "separation", Configuration.CardContainerSeparation },
            { "defaultCardFaceDirection", Enums.CardFace.Up } 
        });
        NameLabel.Text = DisplayName;
        Portrait.Texture = ResourceCache.Instance.Load<Texture2D>(PortraitPath);
        Hp.DetailedValueChanged += HpChanged;
        Hp.FireValueChangeEventsOnInit();
        MaxHp.DetailedValueChanged += MaxHpChanged;
        MaxHp.FireValueChangeEventsOnInit();
        
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
        Hp.Value = MaxHp.Value;
        HoleCardContainer.ContentNodes.Clear();
        BuffContainer.ContentNodes.Clear();
    }
    
    public virtual void RoundReset(float animateCardDelay = 0f)
    {
        var index = 0;
        foreach (var contentNode in HoleCardContainer.ContentNodes.ToList())
        {
            Battle.Dealer.AnimateDiscard(contentNode, Configuration.AnimateCardTransformInterval * index + animateCardDelay);
            index++;
        }
        // HoleCardContainer.ContentNodes.Clear();
    }

    public override string ToString()
    {
        return DisplayName;
    }

    public int ChangeHp(int delta)
    {
        var actualDelta = Mathf.Clamp(delta, -Hp.Value, MaxHp.Value - Hp.Value);
        Hp.Value += actualDelta;
        OnHpChanged?.Invoke(this, actualDelta);
        if (Hp.Value <= 0)
        {
            OnDefeated?.Invoke(this);
        }
        return actualDelta;
    }

    public void ChangeMaxHp(int maxHp)
    {
        if (Hp.Value > maxHp)
        {
            ChangeHp(Hp.Value - maxHp);
        }
    }
    
    protected void HpChanged(object sender, ValueChangedEventDetailedArgs<int> args)
    {
        // TODO: Animate number change here 
        HpLabel.Text = args.NewValue.ToString();
    }
	
    protected void MaxHpChanged(object sender, ValueChangedEventDetailedArgs<int> args)
    {
        // TODO: Animate number change here 
        MaxHpLabel.Text = args.NewValue.ToString();
    }
}