namespace XCardGame.Scripts.GameLogic;

public interface ILifeCycleTriggeredInBattle
{
    public void OnStart(Battle battle);
    
    public void OnStop(Battle battle);
    
    public void OnDisposal(Battle battle);
}