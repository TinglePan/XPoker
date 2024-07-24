namespace XCardGame;

public class BaseSingleTurnFieldEffect: BaseFieldEffect
{
    public BaseSingleTurnFieldEffect(string name, string descriptionTemplate, BaseCard originateCard) :
        base(name, descriptionTemplate, originateCard)
    {
    }

    public override void OnStartEffect(Battle battle)
    {
        base.OnStartEffect(battle);
        battle.OnRoundEnd += OnRoundEnd;
    }

    public override void OnStopEffect(Battle battle)
    {
        base.OnStopEffect(battle);
        battle.OnRoundEnd -= OnRoundEnd;
    }

    protected void OnRoundEnd(Battle battle)
    {
        battle.StopEffect(this);
    }
}