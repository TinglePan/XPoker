using XCardGame.Scripts.Effects;

namespace XCardGame.Scripts.Cards;

public interface ITapCard
{
    public int TapCost { get; }
    public int UnTapCost { get; }
    public BaseEffect Effect { get; }
    public void StartEffect();
    public void ToggleTap();
}