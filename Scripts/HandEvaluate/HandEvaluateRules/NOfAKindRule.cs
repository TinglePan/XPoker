using System.Collections.Generic;
using XCardGame.Scripts.Cards;

using XCardGame.Scripts.Common.Constants;
using Utils = XCardGame.Scripts.Common.Utils;

namespace XCardGame.Scripts.HandEvaluate.HandEvaluateRules;

public class NOfAKindRule: BaseHandEvaluateRule
{
    protected Enums.HandTier ConcreteHandTier;
    protected int N;
    protected List<Enums.CardRank> ValidRanks;
    
    public override Enums.HandTier Tier => ConcreteHandTier;

    public NOfAKindRule(Enums.HandTier handTier, int n, List<Enums.CardRank> validRanks)
    {
        ConcreteHandTier = handTier;
        N = n;
        ValidRanks = validRanks;
    }

    protected override List<List<BaseCard>> Pick(List<BaseCard> cards)
    {
        var picks = new List<List<BaseCard>>();
        var cardsByRank = new Dictionary<Enums.CardRank, List<BaseCard>>();
        foreach (var card in cards)
        {
            if (!cardsByRank.ContainsKey(card.Rank.Value)) cardsByRank[card.Rank.Value] = new List<BaseCard>();
            cardsByRank[card.Rank.Value].Add(card);
        }
        foreach (var rank in ValidRanks)
        {
            if (!cardsByRank.ContainsKey(rank) || cardsByRank[rank].Count < N) continue;
            picks.AddRange(Utils.GetCombinations(cardsByRank[rank], N));
        }
        return picks;
    }
}