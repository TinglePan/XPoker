using System.Collections.ObjectModel;
using XCardGame.Scripts.Cards;
using XCardGame.Scripts.Defs.Def;

namespace XCardGame.Scripts.Game;

public class Deck
{
    public ObservableCollection<BaseCard> CardList;

    public Deck(DeckDef def)
    {
        CardList = new ObservableCollection<BaseCard>();
        if (def.CardDefs != null)
        {
            foreach (var cardDef in def.CardDefs)
            {
                var card = CardFactory.CreateInstance(cardDef.ConcreteClassPath, cardDef);
                CardList.Add(card);
            }
        }
    }
}