using XCardGame.Scripts.Cards.PokerCards;
using XCardGame.Scripts.Common.Constants;
using XCardGame.Scripts.Common.DataBinding;
using XCardGame.Scripts.GameLogic;

namespace XCardGame.Scripts.Cards;

public class BasePokerCardMarker: ILifeCycleTriggeredInBattle
{
    public PokerCard Card;
    
    protected Battle Battle;

    public BasePokerCardMarker(PokerCard card)
    {
        Card = card;
        Battle = card.Battle;
    }

    public void OnAppearInField(Battle battle)
    {
        throw new System.NotImplementedException();
    }

    public void OnDisposalFromField(Battle battle)
    {
        throw new System.NotImplementedException();
    }
}