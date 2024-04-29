using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using XCardGame.Scripts.Cards.AbilityCards;
using XCardGame.Scripts.Cards.PokerCards;
using XCardGame.Scripts.Common.Constants;

namespace XCardGame.Scripts.GameLogic;

public class PlayerBattleEntity: BattleEntity
{
    public ObservableCollection<BaseAbilityCard> AbilityCards;

    public override void Setup(Dictionary<string, object> args)
    {
        base.Setup(args);
        AbilityCards = new ObservableCollection<BaseAbilityCard>();
    }
}