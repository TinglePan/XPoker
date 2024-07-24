namespace XCardGame;

public interface IPowerScaledEffect
{
    public int RawValue { get; }
    public float PowerScale { get; }
    public int CalculateValue(int power);
}