using System.Collections.Generic;
using System.Collections.ObjectModel;
using XCardGame.Scripts.Cards;
using XCardGame.Scripts.Defs.Def;
using XCardGame.Scripts.Defs.Def.Card;

namespace XCardGame.Scripts.Game;

public class Deck
{
    public ObservableCollection<BaseCard> CardList;

    public Deck(List<BaseCardDef> cardDefs)
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

    public void MixIn(List<BaseCardDef> cardDefs)
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