using System.Collections.Generic;
using XCardGame.Scripts.Cards.PokerCards;
using XCardGame.Scripts.Common.Constants;
using Utils = XCardGame.Scripts.Common.Utils;

namespace XCardGame.Scripts.HandEvaluate.HandEvaluateRules;

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

    protected override List<List<BasePokerCard>> Pick(List<BasePokerCard> cards)
    {
        var picks = new List<List<BasePokerCard>>();
        var cardsByRank = new Dictionary<Enums.CardRank, List<BasePokerCard>>();
        foreach (var card in cards)
        {
            if (!cardsByRank.ContainsKey(card.Rank.Value)) cardsByRank[card.Rank.Value] = new List<BasePokerCard>();
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