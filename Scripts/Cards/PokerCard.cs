using System;
using System.Collections.Generic;
using XCardGame.Scripts.Common;
using XCardGame.Scripts.Common.Constants;
using XCardGame.Scripts.Common.DataBinding;
using XCardGame.Scripts.GameLogic;

namespace XCardGame.Scripts.Cards;

public class PokerCard: BaseCard, IComparable<PokerCard>
{
	protected static string GetCardName(Enums.CardSuit suit, Enums.CardRank rank)
	{
		return $"{rank} of {suit}";
	}
	
	public List<BasePokerCardMarker> Markers;
	
	public PokerCard(string texturePath, Enums.CardFace face, Enums.CardSuit suit, Enums.CardRank rank,
		List<BasePokerCardMarker> markers = null, BattleEntity owner = null, bool suitAsSecondComparer = false):
		base(GetCardName(suit, rank), GetCardName(suit, rank), texturePath, face, suit, rank, owner)
	{
		Markers = markers;
		Suit.DetailedValueChanged += OnSuitChanged;
	}

	public override void OnAppear(Battle battle)
	{
		base.OnAppear(battle);
		foreach (var marker in Markers)
		{
			marker.OnAppear(battle);
		}
	}

	public override void OnDisappear(Battle battle)
	{
		base.OnDisappear(battle);
		foreach (var marker in Markers)
		{
			marker.OnDisappear(battle);
		}
	}

	public int CompareTo(PokerCard other)
	{
		return CompareTo(other, false);
	}

	public int CompareTo(PokerCard other, bool isSuitSecondComparer)
	{
		
		var res = Rank.Value.CompareTo(other.Rank.Value);
		if (res == 0 && isSuitSecondComparer)
		{
			res = Suit.Value.CompareTo(other.Suit.Value);
		}
		return res;
	}

	public override string ToString()
	{
		return $"{Rank.Value} of {Suit.Value}(faced {Face.Value})";
	}
	
	protected virtual void OnSuitChanged(object sender, ValueChangedEventDetailedArgs<Enums.CardSuit> args)
	{
		TexturePath.Value = Utils.GetCardTexturePath(args.NewValue);
	}
}