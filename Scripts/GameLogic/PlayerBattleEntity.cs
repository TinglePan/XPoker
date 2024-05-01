using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using XCardGame.Scripts.Cards;
using XCardGame.Scripts.Cards.AbilityCards;
using XCardGame.Scripts.Cards.PokerCards;
using XCardGame.Scripts.Common.Constants;
using XCardGame.Scripts.Common.DataBinding;

namespace XCardGame.Scripts.GameLogic;

public partial class PlayerBattleEntity: BattleEntity
{
    public ObservableCollection<BaseCard> AbilityCards;
    public int Fatigue;
    public ObservableProperty<int> Cost;
    public ObservableProperty<int> MaxCost;

    public override void Setup(Dictionary<string, object> args)
    {
        base.Setup(args);
        var maxCost = (int)args["maxCost"];
        MaxCost = new ObservableProperty<int>(nameof(MaxCost), this, maxCost);
        Cost = new ObservableProperty<int>(nameof(Cost), this, maxCost);
        AbilityCards = new ObservableCollection<BaseCard>();
    }

    public override void RoundReset()
    {
        base.Reset();
        Fatigue = 0;
        Cost.Value = MaxCost.Value;
    }
}