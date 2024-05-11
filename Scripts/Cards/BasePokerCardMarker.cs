using XCardGame.Scripts.Common.Constants;
using XCardGame.Scripts.Common.DataBinding;
using XCardGame.Scripts.GameLogic;

namespace XCardGame.Scripts.Cards;

public class BasePokerCardMarker: ILifeCycleTriggeredInBattle
{
    public string Description;
    public string TexturePath;
    public PokerCard Card;
    
    protected Battle Battle;

    public BasePokerCardMarker(string description, string texturePath, PokerCard card)
    {
        Description = description;
        TexturePath = texturePath;
        Card = card;
        Battle = card.Battle;
    }

    public virtual void OnAppearInField(Battle battle)
    {
    }

    public virtual void OnDisposalFromField(Battle battle)
    {
    }
}