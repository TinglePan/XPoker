using System.Collections.Generic;
using XCardGame.Scripts.Cards;
using XCardGame.Scripts.Cards.AbilityCards;


namespace XCardGame.Scripts.GameLogic;

public class LevelUpInfo
{
    public int Cost;
    public int ShowDownHoleCardCountMax;
    public int DealCardCount;
    
    public List<BaseCard> AbilityCards;
    public List<PokerCard> PokerCards;
    public List<BasePokerCardMarker> PokerCardMarkers;
    
    public LevelUpInfo(int cost, int showDownHoleCardCountMax, int dealCardCount)
    {
        AbilityCards = new List<BaseCard>();
        PokerCards = new List<PokerCard>();
        PokerCardMarkers = new List<BasePokerCardMarker>();
        Cost = cost;
        ShowDownHoleCardCountMax = showDownHoleCardCountMax;
        DealCardCount = dealCardCount;
    }
}