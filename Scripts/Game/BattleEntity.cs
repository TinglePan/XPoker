using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Godot;
using XCardGame.Scripts.Buffs;
using XCardGame.Scripts.Cards;
using XCardGame.Scripts.Cards.InteractCards.EquipmentCards;
using XCardGame.Scripts.Common.Constants;
using XCardGame.Scripts.Common.DataBinding;
using XCardGame.Scripts.Defs.Def.BattleEntity;
using XCardGame.Scripts.Ui;

namespace XCardGame.Scripts.Game;

public partial class BattleEntity: Node, ISetup
{
    public GameMgr GameMgr;
    public Battle Battle;
    public CardContainer HoleCardContainer;
    public BuffContainer BuffContainer;
    public Sprite2D Sprite;
    public Node2D DefenceIcon;
    public Label DefenceLabel;
    public ProgressBar HpBar;
    public Label HpLabel;
    
    public Action<BattleEntity, int> OnHpChanged;
    public Action<BattleEntity> OnDefeated;
    
    public bool HasSetup { get; set; }

    public BattleEntityDef Def;
    public Deck Deck;
    public int DealCardCount;
    public int Speed;
    public int BaseHandPower;
    public Dictionary<Enums.HandTier, int> HandPowers;
    public ObservableProperty<int> Defence;
    public ObservableProperty<int> Hp;
    public ObservableProperty<int> MaxHp;
    // public ObservableProperty<int> Level;
    public bool IsHoleCardDealtVisible;
    public ObservableCollection<BaseCard> HoleCards;
    public ObservableCollection<BaseBuff> Buffs;

    public static Dictionary<string, object> InitArgs(BattleEntityDef def)
    {
        var res = new Dictionary<string, object>();
        res["def"] = def;
        res["deck"] = new Deck(def.InitDeckDef);
        res["dealCardCount"] = Configuration.DefaultHoleCardCount;
        res["handPowers"] = def.InitHandPowers;
        res["maxHp"] = def.InitHp;
        res["baseHandPower"] = def.InitBaseHandPower;
        res["isHoleCardDealtVisible"] = def is PlayerBattleEntityDef;
        return res;
    }

    public override void _Ready()
    {
        base._Ready();
        GameMgr = GetNode<GameMgr>("/root/GameMgr");
        HoleCardContainer = GetNode<CardContainer>("HoleCards");
        BuffContainer = GetNode<BuffContainer>("Buffs");
        Sprite = GetNode<Sprite2D>("Sprite");
        DefenceLabel = GetNode<Label>("Defence/Value");
        DefenceIcon = GetNode<Node2D>("Defence");
        HpBar = GetNode<ProgressBar>("HpBar/Bar");
        HpBar.MinValue = 0;
        HpBar.Step = 1;
        HpLabel = GetNode<Label>("HpBar/Hp");
        
        MaxHp = new ObservableProperty<int>(nameof(MaxHp), this, 0);
        Hp = new ObservableProperty<int>(nameof(Hp), this, 0);
        // Level = new ObservableProperty<int>(nameof(Level), this, 0);
        Defence = new ObservableProperty<int>(nameof(Defence), this, 0);
        Defence.DetailedValueChanged += DefenceChanged;
        Defence.FireValueChangeEventsOnInit();
        Hp.ValueChanged += HpChanged;
        MaxHp.ValueChanged += HpChanged;
        HoleCards = new ObservableCollection<BaseCard>();
        Buffs = new ObservableCollection<BaseBuff>();
        HasSetup = false;
    }

    public virtual void Setup(Dictionary<string, object> args)
    {
        Battle = GameMgr.CurrentBattle;
        Def = (BattleEntityDef)args["def"];
        Deck = (Deck)args["deck"];
        foreach (var card in Deck.CardList)
        {
            card.OwnerEntity = this;
        }
        DealCardCount = (int)args["dealCardCount"];
        HandPowers = (Dictionary<Enums.HandTier, int>)args["handPowers"];
        BaseHandPower = (int)args["baseHandPower"];
        MaxHp.Value =(int)args["maxHp"];
        Hp.Value = MaxHp.Value;
        IsHoleCardDealtVisible = (bool)args["isHoleCardDealtVisible"];
        HoleCardContainer.Setup(new Dictionary<string, object>()
        {
            { "allowInteract", false },
            { "cards", HoleCards },
            { "contentNodeSize", Configuration.CardSize },
            { "separation", Configuration.CardContainerSeparation },
            { "pivotDirection", Enums.Direction2D8Ways.Neutral },
            { "nodesPerRow", 0 },
            { "hasBorder", false },
            { "expectedContentNodeCount", Configuration.DefaultHoleCardCount },
            { "hasName", true },
            { "containerName", "Hole cards" },
            { "defaultCardFaceDirection", IsHoleCardDealtVisible ? Enums.CardFace.Up : Enums.CardFace.Down },
            { "margins", Configuration.DefaultContentContainerMargins },
            { "withCardEffect", true }
        });
        BuffContainer.Setup(new Dictionary<string, object>()
        {
            { "allowInteract", false },
            { "buffs", Buffs },
            { "contentNodeSize", Configuration.CardSize },
            { "separation", Configuration.CardContainerSeparation },
            { "pivotDirection", Enums.Direction2D8Ways.Neutral },
            { "nodesPerRow", Configuration.BuffCountPerRow },
            { "hasBorder", false },
            { "hasName", false },
            { "margins", Configuration.DefaultContentContainerMargins }
        });
        
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
    
    public virtual void RoundReset()
    {
        // HoleCardContainer.ContentNodes.Clear();
    }

    public int GetPower(Enums.HandTier handTier, bool useCharge = true)
    {
        var power = BaseHandPower + HandPowers[handTier];
        foreach (var buff in Buffs)
        {
            if (buff is ChargeBuff chargePowerBuff)
            {
                if (useCharge)
                {
                    power += chargePowerBuff.Stack.Value;
                    chargePowerBuff.Consume();
                }
            } else if (buff is FeebleDeBuff feebleDeBuff)
            {
                if (useCharge)
                {
                    power -= feebleDeBuff.Stack.Value;
                    feebleDeBuff.Consume();
                }
            } else if (buff is EmpowerBuff empowerBuff)
            {
                power += empowerBuff.Stack.Value;
            } else if (buff is CrippleDeBuff crippleDeBuff)
            {
                power -= crippleDeBuff.Stack.Value;
            }
        }
        return power;
    }
    
    public int GetAttackerDamageModifier()
    {
        var res = 0;
        // foreach (var buff in attacker.Buffs)
        // {
        // }
        return res;
    }

    public List<float> GetAttackerDamageMultipliers()
    {
        List<float> res = new();
        foreach (var buff in Buffs)
        {
            if (buff is WeakenDeBuff)
            {
                res.Add(1 - (float)Configuration.WeakenMultiplier / 100);
                buff.Consume();
            } else if (buff is BigShieldCard.BigShieldBuff)
            {
                res.Add(0);
            }
        }
        return res;
    }

    public int GetDefenderDamageModifier()
    {
        var res = 0;
        foreach (var buff in Buffs)
        {
            if (buff is ResistBuff)
            {
                res -= buff.Stack.Value;
            }
        }
        return res;
    }

    public List<float> GetDefenderDamageMultipliers()
    {
        List<float> res = new();
        foreach (var buff in Buffs)
        {
            if (buff is VulnerableDeBuff)
            {
                res.Add(1 + (float)Configuration.VulnerableMultiplier / 100);
                buff.Consume();
            }
            else if (buff is BigShieldCard.BigShieldBuff)
            {
                res.Add(0);
            }
        }
        return res;
    }
    
    public int GetDefenceModifier()
    {
        var res = 0;
        // foreach (var buff in self.Buffs)
        // {
        // }
        return res;
    }
    
    public List<float> GetDefenceMultipliers()
    {
        List<float> res = new();
        foreach (var buff in Buffs)
        {
            if (buff is FragileDeBuff)
            {
                res.Add(1 - (float)Configuration.FragileMultiplier / 100);
            }
        }
        return res;
    }

    public override string ToString()
    {
        return Def.Name;
    }

    public void ChangeDefence(int amount)
    {
        Defence.Value = Mathf.Clamp(Defence.Value + amount, 0, Configuration.MaxDefence);
    }

    public int TakeDamage(int damage, bool bypassDefence = false)
    {
        if (bypassDefence)
        {
            return ChangeHp(-damage);
        }
        else
        {
            var reduceDefence = Mathf.Min(damage, Defence.Value);
            ChangeDefence(-reduceDefence);
            damage -= reduceDefence;
            if (damage > 0)
            {
                return ChangeHp(-damage);
            }
            return 0;
        }
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

    public bool IsDefeated()
    {
        return Hp.Value <= 0;
    }
    
    protected void HpChanged(object sender, ValueChangedEventArgs args)
    {
        string GetHpText(int hp, int maxHp)
        {
            return $"{hp} / {maxHp}";
        }
        HpLabel.Text = GetHpText(Hp.Value, MaxHp.Value);
        HpBar.Value = Hp.Value;
        HpBar.MaxValue = MaxHp.Value;
    }

    protected void DefenceChanged(object sender, ValueChangedEventDetailedArgs<int> args)
    {
        if (args.NewValue <= 0)
        {
            DefenceIcon.Hide();
        }
        else
        {
            DefenceIcon.Show();
            DefenceLabel.Text = args.NewValue.ToString();
        }
    }
}