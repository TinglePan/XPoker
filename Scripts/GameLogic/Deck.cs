using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using Godot;
using XCardGame.Scripts.Cards;

using XCardGame.Scripts.Common.Constants;

namespace XCardGame.Scripts.GameLogic;

public class Deck
{
    public ObservableCollection<BaseCard> CardList;

    public Deck(List<MarkerCard> cards=null)
    {
        CardList = new ObservableCollection<BaseCard>();
        if (cards != null)
        {
            foreach (var card in cards)
            {
                CardList.Add(card);
            }
        }
    }
}