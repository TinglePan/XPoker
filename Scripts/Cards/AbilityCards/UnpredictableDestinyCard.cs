using XCardGame.Scripts.Common;
using XCardGame.Scripts.Common.Constants;
using XCardGame.Scripts.GameLogic;

namespace XCardGame.Scripts.Cards.AbilityCards;

public class UnpredictableDestinyCard: BaseTapCard
{

    public int Count;
    
    public UnpredictableDestinyCard(Enums.CardSuit suit, Enums.CardRank rank, int tappedCost, int unTappedCost) :
        base("Unpredictable destiny", "Add more face-down community cards", 
            "res://Sprites/Cards/unpredictable_destiny.png", suit, rank, tappedCost, unTappedCost)
    {
        Count = 0;
    }
    
    public override void OnStart(Battle battle)
    {
        base.OnStart(battle);
        Count = Utils.GetCardRankValue(Rank.Value);
        battle.FaceDownCommunityCardCount += Count;
    }
    
    public override void OnStop(Battle battle)
    {
        base.OnStop(battle);
        battle.FaceDownCommunityCardCount -= Count;
    }
}