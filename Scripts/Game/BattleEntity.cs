using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot;
using XCardGame.Common;
using XCardGame.Ui;

namespace XCardGame;

public partial class BattleEntity: Node
{
    public class SetupArgs
    {
        public BattleEntityDef Def;
        public Deck Deck;
        public int DealCardCount;
        public int Attack;
        public int Defence;
        public Dictionary<Enums.HandTier, int> HandPowers;
        public int MaxHp;
        public bool IsHoleCardDealtVisible;
    }
    
    public GameMgr GameMgr;
    public Battle Battle;
    public CardContainer HoleCardContainer;
    public BuffContainer BuffContainer;
    public IconWithTextFallback CharacterIcon;
    public Node2D GuardIcon;
    public Label GuardLabel;
    public ProgressBar HpBar;
    public Label HpLabel;
    public Label RoundHandLabel;
    
    public Action<BattleEntity, int> OnHpChanged;
    public Action<BattleEntity> OnDefeated;

    public BattleEntityDef Def;
    public Deck Deck;
    public int DealCardCount;
    public int Attack;
    public int Defence;
    public Dictionary<Enums.HandTier, int> HandPowers;
    public ObservableProperty<int> Guard;
    public ObservableProperty<int> Hp;
    public ObservableProperty<int> MaxHp;
    // public ObservableProperty<int> Level;
    public bool IsHoleCardDealtVisible;
    // public ObservableCollection<BaseCard> HoleCards;
    // public ObservableCollection<BaseBuff> Buffs;
    public ObservableProperty<Enums.HandTier> RoundHandTier;
    public ObservableProperty<Enums.EngageRole> RoundRole;

    public static SetupArgs InitArgs(BattleEntityDef def)
    {
        return new SetupArgs()
        {
            Def = def,
            Deck = new Deck(def.InitDeckDef),
            DealCardCount = Configuration.DefaultHoleCardCount,
            Attack = def.InitAttack,
            Defence = def.InitDefence,
            HandPowers = def.InitHandPowers,
            MaxHp = def.InitHp,
            IsHoleCardDealtVisible = def.IsPlayer
        };
    }

    public override void _Ready()
    {
        base._Ready();
        GameMgr = GetNode<GameMgr>("/root/GameMgr");
        HoleCardContainer = GetNode<CardContainer>("HoleCards");
        BuffContainer = GetNode<BuffContainer>("Buffs");
        CharacterIcon = GetNode<IconWithTextFallback>("Sprite");
        GuardLabel = GetNode<Label>("Defence/Value");
        GuardIcon = GetNode<Node2D>("Defence");
        HpBar = GetNode<ProgressBar>("HpBar/Bar");
        HpBar.MinValue = 0;
        HpBar.Step = 1;
        HpLabel = GetNode<Label>("HpBar/Hp");
        RoundHandLabel = GetNode<Label>("RoundHand/Label");
        
        Hp = new ObservableProperty<int>(nameof(Hp), this, 0);
        MaxHp = new ObservableProperty<int>(nameof(MaxHp), this, 0);
        Hp.ValueChanged += HpChanged;
        MaxHp.ValueChanged += HpChanged;
        // Level = new ObservableProperty<int>(nameof(Level), this, 0);
        Guard = new ObservableProperty<int>(nameof(Guard), this, 0);
        Guard.DetailedValueChanged += GuardChanged;
        Guard.FireValueChangeEventsOnInit();
        RoundRole = new ObservableProperty<Enums.EngageRole>(nameof(RoundRole), this, Enums.EngageRole.None);
        RoundHandTier = new ObservableProperty<Enums.HandTier>(nameof(RoundHandTier), this, Enums.HandTier.HighCard);
        RoundRole.ValueChanged += UpdateRoundHandLabel;
        RoundHandTier.ValueChanged += UpdateRoundHandLabel;
        RoundRole.FireValueChangeEventsOnInit();
        // HoleCards = new ObservableCollection<BaseCard>();
        // Buffs = new ObservableCollection<BaseBuff>();
    }

    public virtual void Setup(object o)
    {
        var args = (SetupArgs)o;
        Battle = GameMgr.CurrentBattle;
        Def = args.Def;
        Deck = args.Deck;
        foreach (var card in Deck.CardList)
        {
            card.Owner = this;
        }

        CharacterIcon.Setup(new IconWithTextFallback.SetupArgs()
        {
            DisplayName = Def.Name,
            IconPath = new ObservableProperty<string>("", this, Def.SpritePath)
        });

        DealCardCount = args.DealCardCount;
        HandPowers = args.HandPowers;
        Attack = args.Attack;
        Defence = args.Defence;
        
        MaxHp.Value = args.MaxHp;
        Hp.Value = MaxHp.Value;
        IsHoleCardDealtVisible = args.IsHoleCardDealtVisible;
        HoleCardContainer.Setup(new CardContainer.SetupArgs
        {
            ContentNodeSize = Configuration.CardSize,
            Separation = Configuration.CardContainerSeparation,
            PivotDirection = Enums.Direction2D8Ways.Neutral,
            DefaultCardFaceDirection = IsHoleCardDealtVisible ? Enums.CardFace.Up : Enums.CardFace.Down,
            Margins = Configuration.DefaultContentContainerMargins,
            HasBorder = true,
            MinContentNodeCountForBorder = DealCardCount,
            HasName = true,
            ShouldCollectDealtItemAndRuleCards = true,
            ContainerName = "Hole cards:"
        });
        BuffContainer.Setup(new BaseContentContainer.SetupArgs
        {
            ContentNodeSize = Configuration.CardSize,
            Separation = Configuration.CardContainerSeparation,
            PivotDirection = Enums.Direction2D8Ways.Neutral,
            Margins = Configuration.DefaultContentContainerMargins,
            NodesPerRow = Configuration.BuffCountPerRow,
        });
    }

    public virtual void Reset()
    {
        Hp.Value = MaxHp.Value;
        HoleCardContainer.ContentNodes.Clear();
        BuffContainer.ContentNodes.Clear();
    }
    
    public virtual async Task RoundReset()
    {
        async Task DiscardCards(List<BaseContentNode> nodes)
        {
            var tasks = new List<Task>();
            foreach (var node in nodes)
            {
                tasks.Add(((CardNode)node).AnimateLeaveField());
                await Utils.Wait(this, Configuration.AnimateCardTransformInterval);
            }
            await Task.WhenAll(tasks);
        }
        RoundRole.Value = Enums.EngageRole.None;
        Guard.Value = 0;
        await DiscardCards(HoleCardContainer.ContentNodes.ToList());
    }

    public void AddBuff(BaseBuff buff)
    {
        foreach (var content in BuffContainer.Contents)
        {
            var existingBuff = (BaseBuff)content;
            if (existingBuff.Name == buff.Name)
            {
                buff.RepeatOn(existingBuff);
                return;
            }
        }

        buff.Effect();
        BuffContainer.Contents.Add(buff);
    }
    
    public BaseBuff GetBuff<T>() where T: BaseBuff
    {
        foreach (var content in BuffContainer.Contents)
        {
            if (content is T buff)
            {
                return buff;
            }
        }
        return null;
    }

    public void RemoveBuff(BaseBuff buff)
    {
        BuffContainer.Contents.Remove(buff);
    }

    public int GetAttackModifier()
    {
        var res = 0;
        foreach (var buff in BuffContainer.Contents)
        {
            switch (buff)
            {
                case ChargeBuff chargeBuff:
                    res += chargeBuff.Stack.Value;
                    chargeBuff.Consume();
                    break;
                case TempAtkBuff beefUpBuff:
                    res += beefUpBuff.Stack.Value;
                    break;
                case TempAtkDeBuff crippleDeBuff:
                    res -= crippleDeBuff.Stack.Value;
                    break;
            }
        }
        return res;
    }

    public List<int> GetAttackMultipliers()
    {
        List<int> res = new();
        
        foreach (var buff in BuffContainer.Contents)
        {
            switch (buff)
            {
                case WeakenDeBuff weakenDeBuff:
                    res.Add(-Configuration.WeakenMultiplier);
                    weakenDeBuff.Consume();
                    break;
                case TauntedBuff tauntedBuff:
                    res.Add(Configuration.TauntedMultiplierPerStack * tauntedBuff.Stack.Value);
                    break;
                case CourageBuff courageBuff:
                    res.Add(Configuration.CourageMultiplierPerStack * courageBuff.Stack.Value);
                    break;
            }
        }
        return res;
    }

    public int GetReceiveDamageModifier()
    {
        var res = 0;
        foreach (var buff in BuffContainer.Contents)
        {
            if (buff is ResistBuff resistBuff)
            {
                res -= resistBuff.Stack.Value;
            }
        }
        return res;
    }
    
    public List<int> GetReceiveDamageMultipliers()
    {
        var res = new List<int>();
        
        foreach (var buff in BuffContainer.Contents)
        {
            switch (buff)
            {
                case VulnerableDeBuff vulnerableDeBuff:
                    res.Add(Configuration.VulnerableMultiplier);
                    vulnerableDeBuff.Consume();
                    break;
                case CautiousBuff cautiousBuff:
                    res.Add(-Configuration.CautiousMultiplierPerStack * cautiousBuff.Stack.Value);
                    break;
            }
        }
        return res;
    }

    
    public int GetDefenceModifier()
    {
        var res = 0;
        foreach (var buff in BuffContainer.Contents)
        {
            switch (buff)
            {
                case ChargeBuff chargeBuff:
                    res += chargeBuff.Stack.Value;
                    chargeBuff.Consume();
                    break;
                case TempAtkBuff beefUpBuff:
                    res += beefUpBuff.Stack.Value;
                    break;
                case TempAtkDeBuff crippleDeBuff:
                    res -= crippleDeBuff.Stack.Value;
                    break;
            }
        }
        return res;
    }
    
    public List<int> GetDefenceMultipliers()
    {
        // List<int> res = new();
        // return res;
        return null;
    }
    

    public override string ToString()
    {
        return Def.Name;
    }

    public void ChangeGuard(int amount)
    {
        Guard.Value = Mathf.Clamp(Guard.Value + amount, 0, Configuration.MaxGuard);
    }

    public int TakeDamage(int damage, bool bypassDefence = false)
    {
        if (bypassDefence)
        {
            return ChangeHp(-damage);
        }
        else
        {
            var reduceDefence = Mathf.Min(damage, Guard.Value);
            ChangeGuard(-reduceDefence);
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

    protected void GuardChanged(object sender, ValueChangedEventDetailedArgs<int> args)
    {
        if (args.NewValue <= 0)
        {
            GuardIcon.Hide();
        }
        else
        {
            GuardIcon.Show();
            GuardLabel.Text = args.NewValue.ToString();
        }
    }
    
    protected void UpdateRoundHandLabel(object sender, ValueChangedEventArgs args)
    {
        RoundHandLabel.Text = GetRoundHandText();
    }

    protected string GetRoundHandText()
    {
        if (RoundRole.Value == Enums.EngageRole.None)
        {
            return Utils._("Round Hand:");
        }
        return Utils._($"Round hand: {Utils.PrettyPrintHandTier(RoundHandTier.Value)}({RoundRole.Value.ToString()})");
    }
}