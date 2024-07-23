using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Godot;
using XCardGame.Scripts.Buffs;
using XCardGame.Scripts.Cards;
using XCardGame.Scripts.Cards.InteractCards.ItemCards;
using XCardGame.Scripts.Common;
using XCardGame.Scripts.Common.Constants;
using XCardGame.Scripts.Common.DataBinding;
using XCardGame.Scripts.Defs.Def.BattleEntity;
using XCardGame.Scripts.Ui;

namespace XCardGame.Scripts.Game;

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
    public Node2D DefenceIcon;
    public Label DefenceLabel;
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
            IsHoleCardDealtVisible = def is PlayerBattleEntityDef
        };
    }

    public override void _Ready()
    {
        base._Ready();
        GameMgr = GetNode<GameMgr>("/root/GameMgr");
        HoleCardContainer = GetNode<CardContainer>("HoleCards");
        BuffContainer = GetNode<BuffContainer>("Buffs");
        CharacterIcon = GetNode<IconWithTextFallback>("Sprite");
        DefenceLabel = GetNode<Label>("Defence/Value");
        DefenceIcon = GetNode<Node2D>("Defence");
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
            card.OwnerEntity = this;
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
                tasks.Add(Battle.Dealer.AnimateDiscard((CardNode)node));
                await Utils.Wait(this, Configuration.AnimateCardTransformInterval);
            }
            await Task.WhenAll(tasks);
        }
        RoundRole.Value = Enums.EngageRole.None;
        Guard.Value = 0;
        await DiscardCards(HoleCardContainer.ContentNodes.ToList());
    }

    public int GetPower(Enums.HandTier handTier, Enums.EngageRole role)
    {
        var power = HandPowers[handTier];
        foreach (var buff in BuffContainer.Contents)
        {
            switch (buff)
            {
                case ChargeBuff chargeBuff:
                    power += chargeBuff.Stack.Value;
                    chargeBuff.Consume();
                    break;
                case BeefUpBuff beefUpBuff:
                    if (role == Enums.EngageRole.Attacker)
                    {
                        power += beefUpBuff.Stack.Value;
                    }
                    break;
                case CrippleDeBuff crippleDeBuff:
                    if (role == Enums.EngageRole.Attacker)
                    {
                        power -= crippleDeBuff.Stack.Value;
                    }
                    break;
                case ResistBuff resistBuff:
                    if (role == Enums.EngageRole.Defender)
                    {
                        power += resistBuff.Stack.Value;
                    }
                    break;
                case FragileDeBuff fragileDeBuff:
                    if (role == Enums.EngageRole.Defender)
                    {
                        power -= fragileDeBuff.Stack.Value;
                    }
                    break;
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
        foreach (var buff in BuffContainer.Contents)
        {
            if (buff is WeakenDeBuff weakenDeBuff)
            {
                res.Add(1 - (float)Configuration.WeakenMultiplier / 100);
                weakenDeBuff.Consume();
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
        foreach (var buff in BuffContainer.Contents)
        {
            if (buff is ResistBuff resistBuff)
            {
                res -= resistBuff.Stack.Value;
            }
        }
        return res;
    }

    public List<float> GetDefenderDamageMultipliers()
    {
        List<float> res = new();
        foreach (var buff in BuffContainer.Contents)
        {
            if (buff is VulnerableDeBuff vulnerableDeBuff)
            {
                res.Add(1 + (float)Configuration.VulnerableMultiplier / 100);
                vulnerableDeBuff.Consume();
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
        foreach (var buff in BuffContainer.Contents)
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
        Guard.Value = Mathf.Clamp(Guard.Value + amount, 0, Configuration.MaxDefence);
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

    protected void GuardChanged(object sender, ValueChangedEventDetailedArgs<int> args)
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