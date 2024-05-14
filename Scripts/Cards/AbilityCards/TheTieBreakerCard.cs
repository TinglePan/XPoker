using XCardGame.Scripts.Common.Constants;
using XCardGame.Scripts.GameLogic;

namespace XCardGame.Scripts.Cards.AbilityCards;

public class TheTieBreakerCard: BaseSealableCard
{
    public TheTieBreakerCard(Enums.CardFace face, Enums.CardSuit suit, Enums.CardRank rank, int cost = 1,
        int sealDuration = 1, bool isQuick = true, BattleEntity owner = null) : 
        base("The Tie Breaker", "Card suit is used to break a tie, suit order from high to low: Spades, Hearts, Clubs, Diamonds",
            "res://Sprites/Cards/the_tie_breaker.png", face, suit, rank, cost, sealDuration, isQuick, owner)
    {
    }
    
    public override void OnAppear(Battle battle)
    {
        base.OnAppear(battle);
        battle.HandEvaluator.IsSuitSecondComparer = true;
    }
    
    public override void OnDisappear(Battle battle)
    {
        base.OnDisappear(battle);
        battle.HandEvaluator.IsSuitSecondComparer = false;
    }
}