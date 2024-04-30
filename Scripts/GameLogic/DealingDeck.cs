using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using XCardGame.Scripts.Cards.PokerCards;
using XCardGame.Scripts.Common.Constants;

namespace XCardGame.Scripts.GameLogic;

public class DealingDeck
{
    public List<BasePokerCard> CardList;
    public List<BasePokerCard> DealingCards;
    public List<BasePokerCard> DealtCards;
    
    public DealingDeck()
    {
        CardList = new List<BasePokerCard>();
        DealingCards = new List<BasePokerCard>();
        DealtCards = new List<BasePokerCard>();
    }
    
    public DealingDeck(List<Deck> srcDecks, List<BasePokerCard> excludedCards = null)
    {
        CardList = new List<BasePokerCard>();
        DealingCards = new List<BasePokerCard>();
        DealtCards = new List<BasePokerCard>();
        foreach (var deck in srcDecks)
        {
            MixIn(deck, excludedCards);
        }
        Shuffle();
    }
    
    public void MixIn(Deck deck, List<BasePokerCard> excludedCards = null)
    {
        foreach (var card in deck.CardList.OfType<BasePokerCard>())
        {
            if (excludedCards != null && excludedCards.Contains(card))
            {
                continue;
            }
            CardList.Add(card);
        }
    }

    public void Shuffle()
    {
        DealingCards.Clear();
        DealtCards.Clear();
        foreach (var card in CardList)
        {
            DealingCards.Add(card);
        }
        Random rng = new Random();
        int n = DealingCards.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            (DealingCards[k], DealingCards[n]) = (DealingCards[n], DealingCards[k]);
        }
    }

    public BasePokerCard Deal()
    {
        if (DealingCards.Count == 0)
        {
            Shuffle();
        }
        var card = DealingCards[0];
        DealingCards.RemoveAt(0);
        DealtCards.Add(card);
        return card;
    }
    
    public BasePokerCard Deal(BattleEntity e)
    {
        for (int i = 0; i < DealingCards.Count; i++)
        {
            var card = DealingCards[i];
            if (card.Owner == e)
            {
                DealingCards.RemoveAt(i);
                DealtCards.Add(card);
                return card;
            }
        }
        return null;
    }
}