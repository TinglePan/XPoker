namespace XCardGame;

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

    public virtual void OnStartEffect(Battle battle)
    {
        
    }

    public virtual void OnStopEffect(Battle battle)
    {
        
    }

    public virtual void Resolve(Battle battle, Engage engage, BattleEntity entity)
    {
        
    }
}