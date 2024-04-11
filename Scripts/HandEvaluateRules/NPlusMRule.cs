using System.Collections.Generic;
using System.Linq;
using Godot;
using XCardGame.Scripts.Cards;
using XCardGame.Scripts.Common;
using XCardGame.Scripts.Common.Constants;

namespace XCardGame.Scripts.HandEvaluateRules;

public class NPlusMRule: NOfAKindRule
{
    public int M;
    
    public NPlusMRule(Enums.HandRank handRank, int n, int m, List<Enums.CardRank> validRanks): base(handRank, n, validRanks)
    {
        M = m;
    }

    protected override List<List<BaseCard>> Pick(List<BaseCard> cards)
    {
        var res = new List<List<BaseCard>>();
        var nPicks = base.Pick(cards);
        var cardsByRank = new Dictionary<Enums.CardRank, List<BaseCard>>();
        foreach (var card in cards)
        {
            if (!cardsByRank.ContainsKey(card.Rank)) cardsByRank[card.Rank] = new List<BaseCard>();
            cardsByRank[card.Rank].Add(card);
        }
        foreach (var nPick in nPicks)
        {
            foreach (var rank in cardsByRank.Keys)
            {
                if (rank == nPick[0].Rank || cardsByRank[rank].Count < M) continue;
                var mPicks = Utils.GetCombinations(cardsByRank[rank], M);
                foreach (var mPick in mPicks)
                {
                    res.Add(nPick.Concat(mPick).ToList());
                }
            }
        }
        return res;
    }

    protected override List<BaseCard> GetPrimaryComparerCards(List<BaseCard> pick, List<BaseCard> cards)
    {
        return pick.GroupBy(card => card.Rank, (rank, groupCards) => new
            {
                Rank = rank,
                Cards = groupCards.OrderByDescending(c => c)
            }).OrderByDescending(group => group.Cards.Count())
            .SelectMany(group => group.Cards).ToList();
    }
}