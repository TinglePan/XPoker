using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using Godot;
using XCardGame.Scripts.Cards;

using XCardGame.Scripts.Common.Constants;

namespace XCardGame.Scripts.GameLogic;

public class Deck
{
    public ObservableCollection<BaseCard> CardList;

    public Deck(List<PokerCard> cards=null)
    {
        CardList = cards == null ? new ObservableCollection<BaseCard>() : new ObservableCollection<BaseCard>(cards);
        // HashSet<Enums.CardRank> excludedRanks = new HashSet<Enums.CardRank>()
        // {
        //     Enums.CardRank.None,
        //     Enums.CardRank.BlackJoker,
        //     Enums.CardRank.RedJoker
        // };
        // foreach (var suit in Enum.GetValues(typeof(Enums.CardSuit)))
        // {
        //     if ((Enums.CardSuit)suit == Enums.CardSuit.None)
        //     {
        //         continue;
        //     }
        //     foreach (var rank in Enum.GetValues(typeof(Enums.CardRank)))
        //     {
        //         if (excludedRanks.Contains((Enums.CardRank)rank))
        //         {
        //             continue;
        //         }
        //
        //         BasePokerCard pokerCard = new BasePokerCard(_gameMgr, (Enums.CardSuit)suit, (Enums.CardRank)rank, Enums.CardFace.Down);
        //         CardList.Add(pokerCard);
        //     }
        // }
    }
}