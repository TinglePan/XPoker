using System.Collections.Generic;
using Microsoft.VisualBasic.CompilerServices;
using XCardGame.Scripts.Cards;
using XCardGame.Scripts.Common.Constants;
using Utils = XCardGame.Scripts.Common.Utils;

namespace XCardGame.Scripts.HandEvaluateRules;

public class NOfAKindRule: BaseHandEvaluateRule
{
    protected Enums.HandRank ConcreteHandRank;
    protected int N;
    protected List<Enums.CardRank> ValidRanks;
    
    public override Enums.HandRank Rank => ConcreteHandRank;

    public NOfAKindRule(Enums.HandRank handRank, int n, List<Enums.CardRank> validRanks)
    {
        ConcreteHandRank = handRank;
        N = n;
        ValidRanks = validRanks;
    }

    protected override List<List<BaseCard>> Pick(List<BaseCard> cards)
    {
        var picks = new List<List<BaseCard>>();
        var cardsByRank = new Dictionary<Enums.CardRank, List<BaseCard>>();
        foreach (var card in cards)
        {
            if (!cardsByRank.ContainsKey(card.Rank)) cardsByRank[card.Rank] = new List<BaseCard>();
            cardsByRank[card.Rank].Add(card);
        }
        foreach (var rank in ValidRanks)
        {
            if (!cardsByRank.ContainsKey(rank) || cardsByRank[rank].Count < N) continue;
            picks.AddRange(Utils.GetCombinations(cardsByRank[rank], N));
        }
        return picks;
    }
}