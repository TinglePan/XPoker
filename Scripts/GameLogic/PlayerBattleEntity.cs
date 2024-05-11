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
    public ObservableCollection<BaseCard> AbilityCards;
    public Dictionary<Enums.CardSuit, ObservableProperty<int>> Overload;
    public ObservableProperty<int> Cost;
    public ObservableProperty<int> MaxCost;

    public Dictionary<int, LevelUpInfo> LevelUpTable;

    public override void Setup(Dictionary<string, object> args)
    {
        base.Setup(args);
        var maxCost = (int)args["maxCost"];
        MaxCost = new ObservableProperty<int>(nameof(MaxCost), this, maxCost);
        Cost = new ObservableProperty<int>(nameof(Cost), this, maxCost);
        AbilityCards = args.TryGetValue("abilityCards", out var arg) ? arg as ObservableCollection<BaseCard> : new ObservableCollection<BaseCard>();
        Overload = new Dictionary<Enums.CardSuit, ObservableProperty<int>>()
        {
            { Enums.CardSuit.Clubs, new ObservableProperty<int>("Overload.Clubs", this, 0) },
            { Enums.CardSuit.Diamonds, new ObservableProperty<int>("Overload.Diamonds", this, 0) },
            { Enums.CardSuit.Hearts, new ObservableProperty<int>("Overload.Hearts", this, 0) },
            { Enums.CardSuit.Spades, new ObservableProperty<int>("Overload.Spades", this, 0) }
        };
        LevelUpTable = args["levelUpTable"] as Dictionary<int, LevelUpInfo>;
        Level.DetailedValueChanged += LevelChanged;
    }

    public override void RoundReset()
    {
        base.Reset();
        foreach (var suit in Overload.Keys)
        {
            Overload[suit].Value = 0;
        }
        Cost.Value = MaxCost.Value;
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
                        AbilityCards.Add(abilityCard);
                    }
                }
                if (levelUpInfo.PokerCards is { Count: > 0 })
                {
                    foreach (var pokerCard in levelUpInfo.PokerCards)
                    {
                        Deck.CardList.Add(pokerCard);
                    }
                }
                MaxCost.Value += levelUpInfo.Cost;
                DealCardCount += levelUpInfo.DealCardCount;
                ShowDownHoleCardCountMax += levelUpInfo.ShowDownHoleCardCountMax;
            }
        } 
    }
}