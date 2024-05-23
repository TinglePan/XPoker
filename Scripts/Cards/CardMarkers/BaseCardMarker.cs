using XCardGame.Scripts.GameLogic;

namespace XCardGame.Scripts.Cards.CardMarkers;

public class BaseCardMarker: ILifeCycleTriggeredInBattle
{
    public string Description;
    public string TexturePath;
    public MarkerCard Card;
    
    protected Battle Battle;

    public BaseCardMarker(string description, string texturePath, MarkerCard card)
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
    
    public virtual void OnDisposal(Battle battle)
    {
    }
}