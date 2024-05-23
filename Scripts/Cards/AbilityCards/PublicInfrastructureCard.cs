using System.Collections.Generic;
using XCardGame.Scripts.Common;
using XCardGame.Scripts.Common.Constants;
using XCardGame.Scripts.GameLogic;

namespace XCardGame.Scripts.Cards.AbilityCards;

public class PublicInfrastructureCard: BaseTapCard
{
    public int Count;
    
    public PublicInfrastructureCard(Enums.CardSuit suit, Enums.CardRank rank, int tapCost, int unTapCost) : 
        base("Public Infrastructure", "Add community cards dealt each round", "res://Sprites/Cards/public_infrastructure.png",
        suit, rank, tapCost, unTapCost)
    {
    }

    public override void OnStart(Battle battle)
    {
        base.OnStart(battle);
        battle.DealCommunityCardCount += Utils.GetCardRankValue(Rank.Value);
    }

    public override void OnStop(Battle battle)
    {
        base.OnStop(battle);
        battle.DealCommunityCardCount -= Utils.GetCardRankValue(Rank.Value);
    }
}