using XCardGame.Scripts.Common.Constants;
using XCardGame.Scripts.GameLogic;

namespace XCardGame.Scripts.Cards.AbilityCards;

public class TheTieBreakerCard: BaseTapCard
{
    public TheTieBreakerCard(Enums.CardSuit suit, Enums.CardRank rank, int tappedCost, int unTappedCost) : 
        base("The Tie Breaker", "Card suit is used to break a tie, suit order from high to low: Spades, Hearts, Clubs, Diamonds",
            "res://Sprites/Cards/the_tie_breaker.png", suit, rank, tappedCost, unTappedCost)
    {
    }
    
    public override void OnStart(Battle battle)
    {
        base.OnStart(battle);
        battle.HandEvaluator.IsSuitSecondComparer = true;
    }

    public override void OnStop(Battle battle)
    {
        base.OnStop(battle);
        battle.HandEvaluator.IsSuitSecondComparer = false;
    }
}