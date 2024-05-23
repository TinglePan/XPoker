using XCardGame.Scripts.Cards;
using XCardGame.Scripts.GameLogic;

namespace XCardGame.Scripts.Effects;

// NOTE: This effect lasts for the entire round. If an effect is going to stop when its creator card is stopped, we will have to call StopEffect in the OnStop method of the creator card.
public class BaseSingleTurnEffect: BaseEffect
{
    public BaseSingleTurnEffect(string name, string description, string iconPath, BaseCard createdByCard) : base(name, description, iconPath, createdByCard)
    {
        createdByCard.Battle.OnRoundEnd += OnRoundEnd;
    }
    
    protected void OnRoundEnd(Battle battle)
    {
        battle.StopEffect(this);
    }
    
}