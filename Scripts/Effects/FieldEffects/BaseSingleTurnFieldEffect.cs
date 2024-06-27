using XCardGame.Scripts.Cards;
using XCardGame.Scripts.GameLogic;
using XCardGame.Scripts.Nodes;

namespace XCardGame.Scripts.Effects;

public class BaseSingleTurnFieldEffect: BaseFieldEffect
{
    public BaseSingleTurnFieldEffect(string name, string descriptionTemplate, BaseCard originateCard) :
        base(name, descriptionTemplate, originateCard)
    {
    }

    public override void OnStart(Battle battle)
    {
        base.OnStart(battle);
        battle.OnRoundEnd += OnRoundEnd;
    }

    public override void OnStop(Battle battle)
    {
        base.OnStop(battle);
        battle.OnRoundEnd -= OnRoundEnd;
    }

    protected void OnRoundEnd(Battle battle)
    {
        battle.StopEffect(this);
    }
}