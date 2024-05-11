using System.Collections.Generic;
using XCardGame.Scripts.Cards;
using XCardGame.Scripts.Cards.PokerCards;
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
    
    protected override List<List<PokerCard>> Pick(List<PokerCard> cards)
    {
        List<List<PokerCard>> picks = new List<List<PokerCard>>();
        var cardsBySuit = new Dictionary<Enums.CardSuit, List<PokerCard>>();
        foreach (var card in cards)
        {
            if (!cardsBySuit.ContainsKey(card.Suit.Value)) cardsBySuit[card.Suit.Value] = new List<PokerCard>();
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