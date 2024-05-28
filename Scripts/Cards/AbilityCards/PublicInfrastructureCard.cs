using System.Collections.Generic;
using XCardGame.Scripts.Common;
using XCardGame.Scripts.Common.Constants;
using XCardGame.Scripts.GameLogic;

namespace XCardGame.Scripts.Cards.AbilityCards;

public class PublicInfrastructureCard: BaseTapCard
{
    public int Count;
    
    public PublicInfrastructureCard(Enums.CardSuit suit, Enums.CardRank rank, int tappedCost, int unTappedCost) : 
        base("Public Infrastructure", "Add community cards dealt each round", "res://Sprites/Cards/public_infrastructure.png",
        suit, rank, tappedCost, unTappedCost)
    {
        Count = 0;
    }

    public override void OnStart(Battle battle)
    {
        base.OnStart(battle);
        Count = Utils.GetCardRankValue(Rank.Value);
        battle.DealCommunityCardCount += Count;
    }

    public override void OnStop(Battle battle)
    {
        base.OnStop(battle);
        battle.DealCommunityCardCount -= Count;
    }
}