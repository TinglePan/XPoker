using XCardGame.Scripts.Common.Constants;

namespace XCardGame.Scripts.Cards.AbilityCards;

public class ShieldCard: BaseTapCard
{
    public ShieldCard(Enums.CardSuit suit, Enums.CardRank rank) : base("Shield", 
        "When this card is untapped, you can neither attack nor be attacked.", "res://Sprites/Cards/shield.png", suit, rank, 0, 0)
    {
    }
}