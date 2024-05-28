namespace XCardGame.Scripts.Cards;

public interface IUseCard
{
    public bool IsRecharging { get; }
    public void ChooseTargets();
    public void Use();
}