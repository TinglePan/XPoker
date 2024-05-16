using System.Collections.Generic;
using XCardGame.Scripts.Cards;
using XCardGame.Scripts.Cards.AbilityCards;


namespace XCardGame.Scripts.GameLogic;

public class LevelUpInfo
{
    public int Cost;
    public int Concentration;
    public int Morale;
    public int ShowDownHoleCardCountMax;
    public int DealCardCount;
    
    public List<BaseCard> AbilityCards;
    public List<PokerCard> PokerCards;
    
    public LevelUpInfo(int cost, int concentration, int morale, int showDownHoleCardCountMax, int dealCardCount, List<BaseCard> abilityCards,
        List<PokerCard> pokerCards)
    {
        Cost = cost;
        Concentration = concentration;
        Morale = morale;
        ShowDownHoleCardCountMax = showDownHoleCardCountMax;
        DealCardCount = dealCardCount;
        AbilityCards = abilityCards;
        PokerCards = pokerCards;
    }
}