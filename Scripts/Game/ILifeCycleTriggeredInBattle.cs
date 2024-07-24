namespace XCardGame;

public interface ILifeCycleTriggeredInBattle
{
    public void OnStartEffect(Battle battle);
    public void OnStopEffect(Battle battle);
}