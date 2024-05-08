using System.Collections.Generic;
using XCardGame.Scripts.Cards.AbilityCards;
using XCardGame.Scripts.Cards.PokerCards;

namespace XCardGame.Scripts.GameLogic;

public class LevelUpInfo
{
    public int Cost;
    public int ShowDownHoleCardCountMax;
    public int DealCardCount;
    
    public List<BaseAbilityCard> AbilityCards;
    public List<BasePokerCard> PokerCards;
    
    public LevelUpInfo(int cost, int showDownHoleCardCountMax, int dealCardCount)
    {
        AbilityCards = new List<BaseAbilityCard>();
        PokerCards = new List<BasePokerCard>();
        Cost = cost;
        ShowDownHoleCardCountMax = showDownHoleCardCountMax;
        DealCardCount = dealCardCount;
    }
}