using System.Collections.Generic;
using System.Linq;
using XCardGame.Scripts.Cards;

using XCardGame.Scripts.Common;
using XCardGame.Scripts.Common.Constants;

namespace XCardGame.Scripts.HandEvaluate.HandEvaluateRules;

public class FlushRule: BaseHandEvaluateRule
{
    protected int CardCount;
    protected List<Enums.CardSuit> ValidSuits;

    public override Enums.HandTier Tier => Enums.HandTier.Flush;
    
    public FlushRule(int cardCount, List<Enums.CardSuit> validSuits)
    {
        CardCount = cardCount;
        ValidSuits = validSuits;
    }
    
    protected override List<List<BaseCard>> Pick(List<BaseCard> cards)
    {
        List<List<BaseCard>> picks = new List<List<BaseCard>>();
        var cardsBySuit = new Dictionary<Enums.CardSuit, List<BaseCard>>();
        foreach (var card in cards)
        {
            if (!cardsBySuit.ContainsKey(card.Suit.Value)) cardsBySuit[card.Suit.Value] = new List<BaseCard>();
            cardsBySuit[card.Suit.Value].Add(card);
        }
        foreach (var suit in ValidSuits)
        {
            if (!cardsBySuit.ContainsKey(suit) || cardsBySuit[suit].Count < CardCount) continue;
            picks.AddRange(Utils.GetCombinations(cardsBySuit[suit], CardCount));
        }
        return picks;
    }
}