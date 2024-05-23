using System.Collections.Generic;
using XCardGame.Scripts.Cards;
using XCardGame.Scripts.Cards.AbilityCards;


namespace XCardGame.Scripts.GameLogic;

public class LevelUpInfo
{
    public int Energy;
    public int HitPoint;
    public int ShowDownHoleCardCountMax;
    public int DealCardCount;
    
    public List<BaseCard> AbilityCards;
    public List<MarkerCard> PokerCards;
    
    public LevelUpInfo(int energy, int hitPoint, int showDownHoleCardCountMax, int dealCardCount, List<BaseCard> abilityCards,
        List<MarkerCard> pokerCards)
    {
        Energy = energy;
        HitPoint = hitPoint;
        ShowDownHoleCardCountMax = showDownHoleCardCountMax;
        DealCardCount = dealCardCount;
        AbilityCards = abilityCards;
        PokerCards = pokerCards;
    }
}