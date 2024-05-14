using System.Collections.Generic;
using XCardGame.Scripts.Common.Constants;
using XCardGame.Scripts.GameLogic;

namespace XCardGame.Scripts.Cards.AbilityCards;

public class PublicInfrastructureCard: BasePassiveCard
{
    public int Count;
    
    public PublicInfrastructureCard(Enums.CardFace face,
        Enums.CardSuit suit, Enums.CardRank rank, int count, int cost = 1, int sealDuration = 1, bool isQuick = true,
        BattleEntity owner = null) : base("Public Infrastructure", "Add community cards dealt each round",
        "res://Sprites/Cards/public_infrastructure.png", face, suit, rank, cost, sealDuration, isQuick, owner)
    {
        Count = count;
    }

    public override void OnAppear(Battle battle)
    {
        base.OnAppear(battle);
        battle.DealCommunityCardCount += Count;
    }
    
    public override void OnDisappear(Battle battle)
    {
        base.OnDisappear(battle);
        battle.DealCommunityCardCount -= Count;
    }
}