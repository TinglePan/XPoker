using XCardGame.Scripts.Common.Constants;
using XCardGame.Scripts.GameLogic;

namespace XCardGame.Scripts.Cards.CardMarkers;

public class BaseCardMarker: ILifeCycleTriggeredInBattle
{
    public string Description;
    public string TexturePath;
    public PokerCard Card;
    
    protected Battle Battle;

    public BaseCardMarker(string description, string texturePath, PokerCard card)
    {
        Description = description;
        TexturePath = texturePath;
        Card = card;
        Battle = card.Battle;
    }

    public virtual void OnStart(Battle battle)
    {
        
    }

    public virtual void OnStop(Battle battle)
    {
        
    }

    public virtual void Resolve(Battle battle, Engage engage, Enums.EngageRole role)
    {
        
    }
}