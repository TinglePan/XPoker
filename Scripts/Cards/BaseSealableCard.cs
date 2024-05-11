using Godot;
using XCardGame.Scripts.Cards.AbilityCards;
using XCardGame.Scripts.Common.Constants;
using XCardGame.Scripts.GameLogic;

namespace XCardGame.Scripts.Cards;

public class BaseSealableCard: BaseActivatableCard
{
    
    public BaseSealableCard(string name, string description, string iconPath, Enums.CardFace face,
        Enums.CardSuit suit, Enums.CardRank rank, int cost, int sealDuration, bool isQuick=true, BattleEntity owner=null) : 
        base(name, description, iconPath, face, suit, rank, cost, sealDuration, isQuick, owner)
    {
    }

    public override bool CanActivate()
    {
        return CoolDownCounter.Value == 0 && (Face.Value == Enums.CardFace.Down || 
                ActualCost <= Battle.Player.Cost.Value);
    }

    public override void Activate()
    {
        Flip();
        if (Face.Value == Enums.CardFace.Down)
        {
            // Seal the card
            if (ActualCost != 0)
            {
                Battle.Player.Cost.Value = Mathf.Clamp(Battle.Player.Cost.Value - ActualCost, 0, Battle.Player.MaxCost.Value);
            }
        }
    }
}