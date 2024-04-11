using System;
using XCardGame.Scripts.Common.Constants;

namespace XCardGame.Scripts.Cards;

public class NumericCard: BaseCard
{
    
    private bool _suitAsSecondComparer = false;
    
    public NumericCard(Enums.CardSuit cardSuit, Enums.CardRank rank, Enums.CardFace face, bool suitAsSecondComparer=false) : base(cardSuit, rank, face)
    {
        _suitAsSecondComparer = suitAsSecondComparer;
    }

    public override int CompareTo(BaseCard other)
    {
        if (!(other is NumericCard))
        {
            return base.CompareTo(other);
        }
        var res = Rank.CompareTo(other.Rank);
        if (res == 0 && _suitAsSecondComparer)
        {
            res = Suit.Value.CompareTo(other.Suit.Value);
        }
        return res;
    }
}