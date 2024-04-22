using System;
using System.Collections.Generic;
using XCardGame.Scripts.Cards.PokerCards;
using XCardGame.Scripts.Common.Constants;

namespace XCardGame.Scripts.GameLogic;

public class Deck
{
    public class DealingDeck
    {
        public List<BasePokerCard> CardList;
        public int CurrentTopIndex;
        public DealingDeck(Deck deck)
        {
            CardList = new List<BasePokerCard>();
            foreach (BasePokerCard card in deck.CardList)
            {
                CardList.Add(card);
            }
            CurrentTopIndex = 0;
        }
        
        public void Shuffle()
        {
            Random rng = new Random();
            int n = CardList.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                (CardList[k], CardList[n]) = (CardList[n], CardList[k]);
            }

        }
        
        public void Reset()
        {
            CurrentTopIndex = 0;
            Shuffle();
        }
        
        public BasePokerCard Deal(bool facedDown = true)
        {
            var card = CardList[CurrentTopIndex++];
            card.Face.Value = facedDown ? Enums.CardFace.Down : Enums.CardFace.Up;
            return card;
        }
    
        public BasePokerCard Peek()
        {
            return CardList[CurrentTopIndex];
        }

    }
    
    public List<BasePokerCard> CardList;
    protected GameMgr GameMgr;

    public Deck(GameMgr gameMgr)
    {
        GameMgr = gameMgr;
        CardList = new List<BasePokerCard>();
        HashSet<Enums.CardRank> excludedRanks = new HashSet<Enums.CardRank>()
        {
            Enums.CardRank.None,
            Enums.CardRank.BlackJoker,
            Enums.CardRank.RedJoker
        };
        foreach (var suit in Enum.GetValues(typeof(Enums.CardSuit)))
        {
            if ((Enums.CardSuit)suit == Enums.CardSuit.None)
            {
                continue;
            }
            foreach (var rank in Enum.GetValues(typeof(Enums.CardRank)))
            {
                if (excludedRanks.Contains((Enums.CardRank)rank))
                {
                    continue;
                }

                BasePokerCard pokerCard = new BasePokerCard(GameMgr, (Enums.CardSuit)suit, (Enums.CardRank)rank, Enums.CardFace.Down);
                CardList.Add(pokerCard);
            }
        }
    }
    
    public DealingDeck Deal()
    {
        var dealingDeck = new DealingDeck(this);
        dealingDeck.Shuffle();
        return dealingDeck;
    }
}