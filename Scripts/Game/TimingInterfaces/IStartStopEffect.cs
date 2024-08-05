namespace XCardGame.TimingInterfaces;

public interface IStartStopEffect
{
    public bool IsEffectActive { get; }
    public void OnStartEffect();
    public void OnStopEffect();
}