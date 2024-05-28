using XCardGame.Scripts.Common.Constants;

namespace XCardGame.Scripts.Cards.AbilityCards;

public class BaseAbilityCard: BaseCard
{

    public int Cost;
    
    public BaseAbilityCard(string name, string description, string iconPath, Enums.CardSuit suit, Enums.CardRank rank, int cost) : base(name, description, iconPath, suit, rank)
    {
        Cost = cost;
    }
    
    
}