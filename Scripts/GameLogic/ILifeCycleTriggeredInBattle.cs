namespace XCardGame.Scripts.GameLogic;

public interface ILifeCycleTriggeredInBattle
{
    public void OnSpawn(Battle battle);
    public void OnExhausted(Battle battle);
}