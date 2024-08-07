using XCardGame.TimingInterfaces;

namespace XCardGame;

public class BaseSingleTurnFieldEffect: BaseFieldEffect, IRoundEnd
{
    public BaseSingleTurnFieldEffect(string name, string descriptionTemplate, BaseCard originateCard) :
        base(name, descriptionTemplate, originateCard)
    {
    }

    public void OnRoundEnd()
    {
        if (IsEffectActive)
        {
            Battle.FieldEffects.Remove(this);
        }
    }
}