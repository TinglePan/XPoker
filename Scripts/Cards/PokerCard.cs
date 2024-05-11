using System;
using System.Collections.Generic;
using XCardGame.Scripts.Common.Constants;
using XCardGame.Scripts.GameLogic;

namespace XCardGame.Scripts.Cards;

public class PokerCard: BaseCard, IComparable<PokerCard>
{
	protected static string GetCardName(Enums.CardSuit suit, Enums.CardRank rank)
	{
		return $"{rank} of {suit}";
	}
	
	public List<BasePokerCardMarker> Markers;
	public bool SuitAsSecondComparer;
	
	public PokerCard(string texturePath, Enums.CardFace face, Enums.CardSuit suit, Enums.CardRank rank,
		List<BasePokerCardMarker> markers = null, BattleEntity owner = null, bool suitAsSecondComparer = false):
		base(GetCardName(suit, rank), GetCardName(suit, rank), texturePath, face, suit, rank, owner)
	{
		Markers = markers;
		SuitAsSecondComparer = suitAsSecondComparer;
	}

	public override void OnAppearInField(Battle battle)
	{
		base.OnAppearInField(battle);
		foreach (var marker in Markers)
		{
			marker.OnAppearInField(battle);
		}
	}

	public override void OnDisposalFromField(Battle battle)
	{
		base.OnDisposalFromField(battle);
		foreach (var marker in Markers)
		{
			marker.OnDisposalFromField(battle);
		}
	}

	public int CompareTo(PokerCard other)
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
}