using XCardGame.Scripts.Common;
using XCardGame.Scripts.Common.Constants;
using XCardGame.Scripts.GameLogic;

namespace XCardGame.Scripts.Cards.AbilityCards;

public class UnpredictableDestinyCard: BaseTapCard
{
    public UnpredictableDestinyCard(Enums.CardSuit suit, Enums.CardRank rank, int tapCost, int unTapCost) :
        base("Unpredictable destiny", "Add more face-down community cards", 
            "res://Sprites/Cards/unpredictable_destiny.png", suit, rank, tapCost, unTapCost)
    {
    }
    
    public override void OnStart(Battle battle)
    {
        base.OnStart(battle);
        battle.FaceDownCommunityCardCount += Utils.GetCardRankValue(Rank.Value);
    }
    
    public override void OnStop(Battle battle)
    {
        base.OnStop(battle);
        battle.FaceDownCommunityCardCount -= Utils.GetCardRankValue(Rank.Value);
    }
}