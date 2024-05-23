using XCardGame.Scripts.Common;
using XCardGame.Scripts.Common.Constants;
using XCardGame.Scripts.GameLogic;

namespace XCardGame.Scripts.Cards.AbilityCards;

public class SocialPolarizationCard: BaseTapCard
{
    public SocialPolarizationCard(Enums.CardSuit suit, Enums.CardRank rank, int tapCost, int unTapCost) : 
        base("Social polarization", "Add community cards dealt each round",
        "res://Sprites/Cards/public_infrastructure.png", suit, rank, tapCost, unTapCost)
    {
    }

    public override void OnStart(Battle battle)
    {
        base.OnStart(battle);
        battle.HandEvaluator.IsCompareHandTierOnly = true;
    }

    public override void OnStop(Battle battle)
    {
        base.OnStop(battle);
        battle.HandEvaluator.IsCompareHandTierOnly = false;
    }
}