using XCardGame.Scripts.Cards;
using XCardGame.Scripts.GameLogic;
using XCardGame.Scripts.Nodes;

namespace XCardGame.Scripts.Effects;

// NOTE: This effect lasts for the entire round. If an effect is going to stop when its creator card is stopped, we will have to call StopEffect in the OnStop method of the creator card.
public class BaseFieldEffect: BaseEffect
{
    public BaseFieldEffect(string name, string descriptionTemplate, BaseCard originateCard) : base(name, descriptionTemplate, originateCard)
    {
    }
}