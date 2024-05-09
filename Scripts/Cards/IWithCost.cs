namespace XCardGame.Scripts.Cards;

public interface IWithCost
{
    public int Cost { get; }
    public bool IsQuick { get; }
    public int ActualCost { get; }
}