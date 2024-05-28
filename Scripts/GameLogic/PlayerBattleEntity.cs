using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using Godot;
using XCardGame.Scripts.Cards;
using XCardGame.Scripts.Cards.AbilityCards;
using XCardGame.Scripts.Cards.SkillCards;
using XCardGame.Scripts.Common.Constants;
using XCardGame.Scripts.Common.DataBinding;

namespace XCardGame.Scripts.GameLogic;

public partial class PlayerBattleEntity: BattleEntity
{
    public ObservableProperty<int> Cost;
    public ObservableProperty<int> MaxCost;
    public Dictionary<int, LevelUpInfo> LevelUpTable;
    
    public override void Setup(Dictionary<string, object> args)
    {
        base.Setup(args);
        var maxCost = (int)args["maxCost"];
        MaxCost = new ObservableProperty<int>(nameof(MaxCost), this, maxCost);
        Cost = new ObservableProperty<int>(nameof(Cost), this, maxCost);
        LevelUpTable = (Dictionary<int, LevelUpInfo>)args["levelUpTable"];
        Level.DetailedValueChanged += LevelChanged;
    }

    public override void RoundReset()
    {
        base.RoundReset();
        Cost.Value = MaxCost.Value;
    }

    protected void LevelChanged(object obj, ValueChangedEventDetailedArgs<int> args)
    {
        Debug.Assert(args.OldValue < args.NewValue);
        for (int i = args.OldValue; i < args.NewValue; i++)
        {
            if (LevelUpTable.TryGetValue(i, out var levelUpInfo))
            {
                // TODO: Choose one from cards in list?
                if (levelUpInfo.GrantCards is { Count: > 0 })
                {
                    foreach (var card in levelUpInfo.GrantCards)
                    {
                        if (card is MarkerCard markerCard)
                        {
                            Deck.CardList.Add(markerCard);
                        } else if (card is BaseSkillCard skillCard)
                        {
                            SkillCardContainer.Contents.Add(skillCard);
                        }
                        else if (card is BaseAbilityCard abilityCard)
                        {
                            AbilityCards.Add(abilityCard);
                        }
                        else
                        {
                            GD.PrintErr($"Invalid card type {card} in level up table");
                        }
                    }
                }
                MaxCost.Value += levelUpInfo.Cost;
            }
        } 
    }

}