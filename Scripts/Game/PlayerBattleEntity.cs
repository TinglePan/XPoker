using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using XCardGame.Scripts.Cards;
using XCardGame.Scripts.Common.DataBinding;
using XCardGame.Scripts.Defs.Def;
using XCardGame.Scripts.Nodes;

namespace XCardGame.Scripts.GameLogic;

public partial class PlayerBattleEntity: BattleEntity
{
    public ObservableProperty<int> Energy;
    public ObservableProperty<int> MaxEnergy;
    public ObservableProperty<int> Credit;

    public ObservableCollection<BaseCard> Equipments;
    public ObservableProperty<int> ItemPocketSize;
    public ObservableCollection<BaseCard> Items;
    // public Dictionary<int, LevelUpInfo> LevelUpTable;

    public static Dictionary<string, object> InitArgs(PlayerBattleEntityDef def)
    {
        var res = BattleEntity.InitArgs(def);
        res["itemPocketSize"] = def.InitItemPocketSize;
        return res;
    }
    
    // public override void _Ready()
    // {
    //     base._Ready();
    //     Level.DetailedValueChanged += LevelChanged;
    // }

    public override void Setup(Dictionary<string, object> args)
    {
        // LevelUpTable = (Dictionary<int, LevelUpInfo>)args["levelUpTable"];
        base.Setup(args);
        Energy = new ObservableProperty<int>(nameof(Energy), this, (int)args["energy"]);
        MaxEnergy = new ObservableProperty<int>(nameof(MaxEnergy), this, (int)args["maxEnergy"]);
        Credit = new ObservableProperty<int>(nameof(Credit), this, 0);
        Equipments = new ObservableCollection<BaseCard>();
        if (args.TryGetValue("equipments", out var equipments))
        {
            foreach (var equipmentCard in (List<BaseCard>)equipments)
            {
                Equipments.Add(equipmentCard);
            }
        }
        ItemPocketSize = new ObservableProperty<int>(nameof(ItemPocketSize), this, (int)args["itemPocketSize"]);
        Items = new ObservableCollection<BaseCard>();
        if (args.TryGetValue("items", out var items))
        {
            foreach (var itemCard in (List<BaseCard>)items)
            {
                Items.Add(itemCard);
            }            
        }
    }

    public override void RoundReset()
    {
        Energy.Value = MaxEnergy.Value;
    }

    public bool CanBuy(CardNode cardNode)
    {
        return Credit.Value >= cardNode.Content.Value.Def.BasePrice;
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