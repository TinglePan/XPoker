using System.Collections.Generic;
using System.Linq;
using XCardGame.Scripts.Cards;
using XCardGame.Scripts.Cards.PokerCards;
using XCardGame.Scripts.Common;
using XCardGame.Scripts.Common.Constants;

namespace XCardGame.Scripts.HandEvaluate.HandEvaluateRules;

public class NPlusMRule: NOfAKindRule
{
    public int M;
    
    public NPlusMRule(Enums.HandTier handTier, int n, int m, List<Enums.CardRank> validRanks): base(handTier, n, validRanks)
    {
        M = m;
    }

    protected override List<List<PokerCard>> Pick(List<PokerCard> cards)
    {
        var res = new List<List<PokerCard>>();
        var nPicks = base.Pick(cards);
        var cardsByRank = new Dictionary<Enums.CardRank, List<PokerCard>>();
        foreach (var card in cards)
        {
            if (!cardsByRank.ContainsKey(card.Rank.Value)) cardsByRank[card.Rank.Value] = new List<PokerCard>();
            cardsByRank[card.Rank.Value].Add(card);
        }
        foreach (var nPick in nPicks)
        {
            foreach (var rank in cardsByRank.Keys)
            {
                if (rank == nPick[0].Rank.Value || cardsByRank[rank].Count < M) continue;
                var mPicks = Utils.GetCombinations(cardsByRank[rank], M);
                foreach (var mPick in mPicks)
                {
                    res.Add(nPick.Concat(mPick).ToList());
                }
            }
        }
        return res;
    }

    protected override List<PokerCard> GetPrimaryComparerCards(List<PokerCard> pick, List<PokerCard> cards)
    {
        return pick.GroupBy(card => card.Rank, (rank, groupCards) => new
            {
                Rank = rank,
                Cards = groupCards.OrderByDescending(c => c)
            }).OrderByDescending(group => group.Cards.Count())
            .SelectMany(group => group.Cards).ToList();
    }
}