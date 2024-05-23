using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using XCardGame.Scripts.Cards;
using XCardGame.Scripts.Cards.AbilityCards;

using XCardGame.Scripts.Common.Constants;
using XCardGame.Scripts.Common.DataBinding;

namespace XCardGame.Scripts.GameLogic;

public partial class PlayerBattleEntity: BattleEntity
{
    public ObservableProperty<int> Energy;
    public ObservableProperty<int> MaxEnergy;
    public Dictionary<int, LevelUpInfo> LevelUpTable;
    
    public override void Setup(Dictionary<string, object> args)
    {
        base.Setup(args);
        var maxEnergy = (int)args["maxEnergy"];
        MaxEnergy = new ObservableProperty<int>(nameof(MaxEnergy), this, maxEnergy);
        Energy = new ObservableProperty<int>(nameof(Energy), this, maxEnergy);
        LevelUpTable = (Dictionary<int, LevelUpInfo>)args["levelUpTable"];
        Level.DetailedValueChanged += LevelChanged;
    }

    public override void RoundReset()
    {
        base.RoundReset();
        Energy.Value = MaxEnergy.Value;
    }

    protected void LevelChanged(object obj, ValueChangedEventDetailedArgs<int> args)
    {
        Debug.Assert(args.OldValue < args.NewValue);
        for (int i = args.OldValue; i < args.NewValue; i++)
        {
            if (LevelUpTable.TryGetValue(i, out var levelUpInfo))
            {
                // TODO: Choose one from cards in list
                if (levelUpInfo.AbilityCards is { Count: > 0 })
                {
                    foreach (var abilityCard in levelUpInfo.AbilityCards)
                    {
                        AbilityCardContainer.Contents.Add(abilityCard);
                    }
                }
                if (levelUpInfo.PokerCards is { Count: > 0 })
                {
                    foreach (var pokerCard in levelUpInfo.PokerCards)
                    {
                        Deck.CardList.Add(pokerCard);
                    }
                }
                MaxEnergy.Value += levelUpInfo.Energy;
                DealCardCount += levelUpInfo.DealCardCount;
                ShowDownHoleCardCountMax += levelUpInfo.ShowDownHoleCardCountMax;
            }
        } 
    }

}