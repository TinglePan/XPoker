namespace XCardGame.Scripts.GameLogic;

public interface ILifeCycleTriggeredInBattle
{
    public void OnAppearInField(Battle battle);
    public void OnDisposalFromField(Battle battle);
}