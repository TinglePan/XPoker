using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using Godot;
using hamsterbyte.DeveloperConsole;
using XCardGame.Scripts.Cards;
using XCardGame.Scripts.Cards.AbilityCards;
using XCardGame.Scripts.Cards.SkillCards;
using XCardGame.Scripts.Common.Constants;
using XCardGame.Scripts.Common.DataBinding;
using XCardGame.Scripts.Defs;
using XCardGame.Scripts.Nodes;

namespace XCardGame.Scripts.GameLogic;

public partial class PlayerBattleEntity: BattleEntity
{
    public ObservableProperty<int> Cost;
    public ObservableProperty<int> MaxCost;
    public ObservableProperty<int> Credit;
    // public Dictionary<int, LevelUpInfo> LevelUpTable;

    public static Dictionary<string, object> InitArgs(PlayerBattleEntityDef def)
    {
        var res = BattleEntity.InitArgs(def);
        res["maxCost"] = def.InitCost;
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
        Cost = new ObservableProperty<int>(nameof(Cost), this, 0);
        MaxCost = new ObservableProperty<int>(nameof(MaxCost), this, (int)args["maxCost"]);
        Credit = new ObservableProperty<int>(nameof(Credit), this, 0);
    }

    public override async Task RoundReset(float delay = 0f)
    {
        await base.RoundReset(delay);
        Cost.Value = MaxCost.Value;
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