using System;
using XCardGame.Scripts.Common.Constants;
using XCardGame.Scripts.Common.DataBinding;

namespace XCardGame.Scripts.Cards.PokerCards;

public class BasePokerCard: BaseCard, IComparable<BasePokerCard>
{
	public ObservableProperty<Enums.CardSuit> Suit;

	public ObservableProperty<Enums.CardRank> Rank;
	public bool SuitAsSecondComparer;

	public Enums.CardColor CardColor => Suit.Value switch
	{
		Enums.CardSuit.Spades => Enums.CardColor.Black,
		Enums.CardSuit.Clubs => Enums.CardColor.Black,
		Enums.CardSuit.Hearts => Enums.CardColor.Red,
		Enums.CardSuit.Diamonds => Enums.CardColor.Red,
		_ => Enums.CardColor.None
	};
	
	public BasePokerCard(Enums.CardSuit cardSuit, Enums.CardRank rank, Enums.CardFace face, bool suitAsSecondComparer=false): base(GetCardName(rank, cardSuit), GetCardName(rank, cardSuit), face)
	{
		Suit = new ObservableProperty<Enums.CardSuit>(nameof(Suit), this, cardSuit);
		Rank = new ObservableProperty<Enums.CardRank>(nameof(Rank), this, rank);
		SuitAsSecondComparer = suitAsSecondComparer;
	}
	
	public BasePokerCard(BasePokerCard card): base(card)
	{
		Suit = new ObservableProperty<Enums.CardSuit>(nameof(Suit), this, card.Suit.Value);
		Rank = new ObservableProperty<Enums.CardRank>(nameof(Rank), this, card.Rank.Value);
		SuitAsSecondComparer = card.SuitAsSecondComparer;
	}
	
	public int CompareTo(BasePokerCard other)
	{
		var res = Rank.Value.CompareTo(other.Rank.Value);
		if (res == 0 && SuitAsSecondComparer)
		{
			res = Suit.Value.CompareTo(other.Suit.Value);
		}
		return res;
	}

	public override string ToString()
	{
		return $"{Rank.Value} of {Suit.Value}(faced {Face.Value})";
	}

	protected static string GetCardName(Enums.CardRank rank, Enums.CardSuit suit)
	{
		return $"{rank} of {suit}";
	}
}