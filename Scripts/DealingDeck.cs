using System;
using XCardGame.Scripts.Cards;

namespace XCardGame.Scripts;

public class DealingDeck: Deck
{
    public int CurrentTopIndex;

    public DealingDeck(Deck deck)
    {
        foreach (BaseCard card in deck.CardList)
        {
            CardList.Add(card);
        }
        CurrentTopIndex = 0;
    }

    public BaseCard Next()
    {
        return CardList[CurrentTopIndex++];
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

    public void DealCardTo(Player player)
    {
        var card = Next();
        player.AddHoleCard(card);
    }
}