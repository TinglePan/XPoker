using XCardGame.Scripts.Effects;

namespace XCardGame.Scripts.Cards;

public interface ITapCard
{
    public int TappedCost { get; }
    public int UnTappedCost { get; }
    public BaseEffect Effect { get; }
    public void StartEffect();
    public void ToggleTap();
}