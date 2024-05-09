using Godot;
using XCardGame.Scripts.Common.Constants;
using XCardGame.Scripts.Common.DataBinding;
using XCardGame.Scripts.GameLogic;

namespace XCardGame.Scripts.Cards.AbilityCards;

public class BaseSealableAbilityCard: BaseActivatableAbilityCard
{
    
    public BaseSealableAbilityCard(GameMgr gameMgr, string name, string description, Enums.CardFace face,
        Enums.CardSuit suit, string iconPath, int cost, int sealDuration, bool isQuick=true, BattleEntity owner=null) : 
        base(gameMgr, name, description, face, suit, iconPath, cost, sealDuration, isQuick, owner)
    {
    }

    public override bool CanActivate()
    {
        return CoolDownCounter.Value == 0 && (Face.Value == Enums.CardFace.Down || 
                ActualCost <= Battle.Player.Cost.Value);
    }

    public override void Activate()
    {
        Flip(Battle, Battle.Player);
    }

    public override void Flip(Battle battle, BattleEntity flippedBy)
    {
        base.Flip(battle, flippedBy);
        if (Face.Value == Enums.CardFace.Up)
        {
            // Seal the card
            if (ActualCost != 0)
            {
                battle.Player.Cost.Value = Mathf.Clamp(battle.Player.Cost.Value - ActualCost, 0, battle.Player.MaxCost.Value);
            }
        }
    }
}