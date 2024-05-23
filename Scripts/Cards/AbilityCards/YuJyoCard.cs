using XCardGame.Scripts.Common.Constants;
using XCardGame.Scripts.GameLogic;

namespace XCardGame.Scripts.Cards.AbilityCards;

public class YuJyoCard: BaseTapCard
{
    public YuJyoCard(Enums.CardSuit suit, Enums.CardRank rank, int tapCost, int unTapCost) : 
        base("Friendship", "You will be dealt cards in your opponent's deck and vice versa", 
            "res://Sprites/Cards/yujyo.png", suit, rank, tapCost, unTapCost)
    {
    }
    
    // NYI
}