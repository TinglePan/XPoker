namespace XCardGame.Scripts.Cards;

public interface IActivatableCard
{
    public bool CanActivate();

    public void Activate();

    public void AfterEffect();
    public void AfterCanceled();
}