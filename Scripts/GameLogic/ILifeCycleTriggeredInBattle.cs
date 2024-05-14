namespace XCardGame.Scripts.GameLogic;

public interface ILifeCycleTriggeredInBattle
{
    public void OnAppear(Battle battle);
    
    public void OnDisappear(Battle battle);
    
    public void OnDisposal(Battle battle);
}