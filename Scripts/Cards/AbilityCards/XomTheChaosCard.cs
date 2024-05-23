using XCardGame.Scripts.Common.Constants;
using XCardGame.Scripts.GameLogic;

namespace XCardGame.Scripts.Cards.AbilityCards;

public class XomTheChaosCard: BaseTapCard
{
    public XomTheChaosCard(Enums.CardSuit suit, Enums.CardRank rank, int tapCost, int unTapCost) : 
        base("Xom the Chaos", "Random effects that change every turn.", 
            "res://Sprites/Cards/xom_the_chaos.png", suit, rank, tapCost, unTapCost)
    {
    }
    
    // NYI
}