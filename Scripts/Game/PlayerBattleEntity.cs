using System.Threading.Tasks;
using Godot;
using XCardGame.Common;
using XCardGame.Ui;

namespace XCardGame;

public partial class PlayerBattleEntity: BattleEntity
{

    public new class SetupArgs : BattleEntity.SetupArgs
    {
        public int Energy;
        public int MaxEnergy;
        public int Credit;
        public int ItemPocketSize;
        public int ItemRecharge;
    }
    
    public Label EnergyLabel;
    
    public ObservableProperty<int> Energy;
    public ObservableProperty<int> MaxEnergy;
    public ObservableProperty<int> Credit;

    public ObservableProperty<int> ItemPocketSize;
    public ObservableProperty<int> ItemRecharge;
    // public Dictionary<int, LevelUpInfo> LevelUpTable;

    public static SetupArgs InitArgs(PlayerBattleEntityDef def)
    {
        return new SetupArgs
        {
            Def = def,
            Deck = new Deck(def.InitDeckDef),
            DealCardCount = Configuration.DefaultHoleCardCount,
            Attack = def.InitAttack,
            Defence = def.InitDefence,
            HandPowers = def.InitHandPowers,
            MaxHp = def.InitHp,
            IsHoleCardDealtVisible = true,
            Energy = def.InitEnergy,
            MaxEnergy = def.InitEnergy,
            Credit = def.InitCredit,
            ItemPocketSize = def.InitItemPocketSize,
            ItemRecharge = def.InitItemRecharge,
        };
    }
    
    public override void _Ready()
    {
        base._Ready();
        EnergyLabel = GetNode<Label>("Energy/Label");
    }

    public override void Setup(object o)
    {
        // LevelUpTable = (Dictionary<int, LevelUpInfo>)args["levelUpTable"];
        base.Setup(o);
        var args = (SetupArgs)o;
        Energy = new ObservableProperty<int>(nameof(Energy), this, args.Energy);
        MaxEnergy = new ObservableProperty<int>(nameof(MaxEnergy), this, args.MaxEnergy);
        Energy.ValueChanged += UpdateEnergyLabel;
        MaxEnergy.ValueChanged += UpdateEnergyLabel;
        Energy.FireValueChangeEventsOnInit();
        Credit = new ObservableProperty<int>(nameof(Credit), this, 0);
        // TODO: Credit display binding
        Credit.FireValueChangeEventsOnInit();
        ItemPocketSize = new ObservableProperty<int>(nameof(ItemPocketSize), this, args.ItemPocketSize);
        // TODO: Item pocket size display binding
        ItemPocketSize.FireValueChangeEventsOnInit();
        ItemRecharge = new ObservableProperty<int>(nameof(ItemRecharge), this, args.ItemRecharge);
        // TODO: Item recharge count display binding
        ItemRecharge.FireValueChangeEventsOnInit();
    }

    public override async Task RoundReset()
    {
        await base.RoundReset();
        Energy.Value = MaxEnergy.Value;
    }

    public bool CanBuy(CardNode cardNode)
    {
        return Credit.Value >= cardNode.Card.Def.BasePrice;
    }
    
    protected void UpdateEnergyLabel(object sender, ValueChangedEventArgs args)
    {
        EnergyLabel.Text = GetEnergyText();
    }

    protected string GetEnergyText()
    {
        return Utils._($"En: {Energy.Value} / {MaxEnergy.Value}");
    }

    // protected void LevelChanged(object obj, ValueChangedEventDetailedArgs<int> args)
    // {
    //     Debug.Assert(args.OldValue < args.NewValue);
    //     for (int i = args.OldValue; i < args.NewValue; i++)
    //     {
    //         if (LevelUpTable != null && LevelUpTable.TryGetValue(i, out var levelUpInfo))
    //         {
    //             // TODO: Choose one from cards in list?
    //             if (levelUpInfo.GrantCards is { Count: > 0 })
    //             {
    //                 foreach (var card in levelUpInfo.GrantCards)
    //                 {
    //                     if (card is PokerCard markerCard)
    //                     {
    //                         Deck.CardList.Add(markerCard);
    //                     } else if (card is BaseSkillCard skillCard)
    //                     {
    //                         SkillCards.Add(skillCard);
    //                     }
    //                     else if (card is BaseAbilityCard abilityCard)
    //                     {
    //                         AbilityCards.Add(abilityCard);
    //                     }
    //                     else
    //                     {
    //                         GD.PrintErr($"Invalid card type {card} in level up table");
    //                     }
    //                 }
    //             }
    //             MaxCost.Value += levelUpInfo.Cost;
    //         }
    //     } 
    // }

}