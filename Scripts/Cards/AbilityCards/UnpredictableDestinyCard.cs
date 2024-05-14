using XCardGame.Scripts.Common.Constants;
using XCardGame.Scripts.GameLogic;

namespace XCardGame.Scripts.Cards.AbilityCards;

public class UnpredictableDestinyCard: BaseSealableCard
{
    public int Count;
    
    public UnpredictableDestinyCard(Enums.CardFace face, Enums.CardSuit suit, Enums.CardRank rank, int cost = 1,
        int sealDuration = 1, bool isQuick = true, BattleEntity owner = null) :
        base("Unpredictable destiny", "Add more face-down community cards", 
            "res://Sprites/Cards/unpredictable_destiny.png", face, suit, rank, cost, sealDuration, isQuick, owner)
    {
    }
    
    
    public override void OnAppear(Battle battle)
    {
        base.OnAppear(battle);
        battle.FaceDownCommunityCardCount += Count;
    }
    
    public override void OnDisappear(Battle battle)
    {
        base.OnDisappear(battle);
        battle.FaceDownCommunityCardCount -= Count;
    }
}