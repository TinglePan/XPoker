using XCardGame.TimingInterfaces;

namespace XCardGame;

// NOTE: This effect lasts for the entire round. If an effect is going to stop when its creator card is stopped, we will have to call StopEffect in the OnStop method of the creator card.
public class BaseFieldEffect: BaseEffect
{
    public bool IsEffectActive => Battle.FieldEffects.Contains(this);
    
    public BaseFieldEffect(string name, string descriptionTemplate, BaseCard originateCard) : base(name, descriptionTemplate, originateCard)
    {
    }

    public void OnStartEffect()
    {
        if (!IsEffectActive)
        {
            Battle.FieldEffects.Add(this);
        }
    }

    public void OnStopEffect()
    {
        if (IsEffectActive)
        {
            Battle.FieldEffects.Remove(this);
        }
    }
}