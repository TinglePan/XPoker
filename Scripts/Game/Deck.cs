using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace XCardGame;

public class Deck
{
    public ObservableCollection<BaseCard> CardList;

    public Deck(List<CardDef> cardDefs)
    {
        CardList = new ObservableCollection<BaseCard>();
        if (cardDefs != null)
        {
            foreach (var cardDef in cardDefs)
            {
                var card = CardFactory.CreateInstance(cardDef.ConcreteClassPath, cardDef);
                CardList.Add(card);
            }
        }
    }

    public void MixIn(List<CardDef> cardDefs)
    {
        if (cardDefs != null)
        {
            foreach (var cardDef in cardDefs)
            {
                var card = CardFactory.CreateInstance(cardDef.ConcreteClassPath, cardDef);
                CardList.Add(card);
            }
        }
    }

    public void MixIn(List<BaseCard> cards)
    {
        foreach (var card in cards)
        {
            CardList.Add(card);
        }
    }
}