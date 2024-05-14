using XCardGame.Scripts.Common.Constants;
using XCardGame.Scripts.GameLogic;

namespace XCardGame.Scripts.Cards.AbilityCards;

public class SocialPolarizationCard: BaseSealableCard
{
    public SocialPolarizationCard(Enums.CardFace face,
        Enums.CardSuit suit, Enums.CardRank rank, int count, int cost = 1, int sealDuration = 1, bool isQuick = true,
        BattleEntity owner = null) : base("Public Infrastructure", "Add community cards dealt each round",
        "res://Sprites/Cards/public_infrastructure.png", face, suit, rank, cost, sealDuration, isQuick, owner)
    {
    }
    
    public override void OnAppear(Battle battle)
    {
        base.OnAppear(battle);
        battle.HandEvaluator.IsCompareHandTierOnly = true;
    }
    
    public override void OnDisappear(Battle battle)
    {
        base.OnDisappear(battle);
        battle.HandEvaluator.IsCompareHandTierOnly = false;
    }
}