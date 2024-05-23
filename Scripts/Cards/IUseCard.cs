namespace XCardGame.Scripts.Cards;

public interface IUseCard
{
    public bool IsRecharging { get; }
    public int Cost { get; }
    
    public int ActualCost();
    public void ChooseTargets();
    public void Use();
}