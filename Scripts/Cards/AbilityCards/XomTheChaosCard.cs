using XCardGame.Scripts.Common.Constants;
using XCardGame.Scripts.GameLogic;

namespace XCardGame.Scripts.Cards.AbilityCards;

public class XomTheChaosCard: BaseSealableCard
{
    public XomTheChaosCard(Enums.CardFace face, Enums.CardSuit suit, Enums.CardRank rank, int cost = 1,
        int sealDuration = 1, bool isQuick = true, BattleEntity owner = null) : 
        base("Xom the Chaos", "Random effects that change every turn.", 
            "res://Sprites/Cards/xom_the_chaos.png", face, suit, rank, cost, sealDuration, isQuick, owner)
    {
    }
    
    // NYI
}