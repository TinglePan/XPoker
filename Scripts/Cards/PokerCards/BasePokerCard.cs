using System;
using XCardGame.Scripts.Common.Constants;
using XCardGame.Scripts.Common.DataBinding;
using XCardGame.Scripts.GameLogic;

namespace XCardGame.Scripts.Cards.PokerCards;

public class BasePokerCard: BaseCard, IComparable<BasePokerCard>
{

	public ObservableProperty<Enums.CardRank> Rank;
	public bool SuitAsSecondComparer;

	
	public BasePokerCard(Enums.CardSuit cardSuit, Enums.CardFace face, Enums.CardRank rank, 
		BattleEntity owner=null, bool suitAsSecondComparer=false): base(GetCardName(rank, cardSuit), GetCardName(rank, cardSuit), face, cardSuit, owner)
	{
		Rank = new ObservableProperty<Enums.CardRank>(nameof(Rank), this, rank);
		SuitAsSecondComparer = suitAsSecondComparer;
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