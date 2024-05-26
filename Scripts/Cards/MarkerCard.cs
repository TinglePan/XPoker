using System;
using System.Collections.Generic;
using XCardGame.Scripts.Cards.CardMarkers;
using XCardGame.Scripts.Common;
using XCardGame.Scripts.Common.Constants;
using XCardGame.Scripts.Common.DataBinding;
using XCardGame.Scripts.GameLogic;

namespace XCardGame.Scripts.Cards;

public class MarkerCard: BaseCard
{
	protected static string GetCardName(Enums.CardSuit suit, Enums.CardRank rank)
	{
		return $"{rank} of {suit}";
	}
	
	public List<BaseCardMarker> Markers;
	
	public MarkerCard(string texturePath, Enums.CardSuit suit, Enums.CardRank rank,
		List<BaseCardMarker> markers = null, bool suitAsSecondComparer = false):
		base(GetCardName(suit, rank), GetCardName(suit, rank), texturePath, suit, rank)
	{
		Markers = markers;
		Suit.DetailedValueChanged += OnSuitChanged;
	}

	public override void OnStart(Battle battle)
	{
		base.OnStart(battle);
		if (Markers != null)
		{
			foreach (var marker in Markers)
			{
				marker.OnStart(battle);
			}
		}
	}

	public override void OnStop(Battle battle)
	{
		base.OnStop(battle);
		if (Markers != null)
		{
			foreach (var marker in Markers)
			{
				marker.OnStop(battle);
			}
		}
	}

	public override string ToString()
	{
		return $"{Rank.Value} of {Suit.Value}";
	}
	
	protected virtual void OnSuitChanged(object sender, ValueChangedEventDetailedArgs<Enums.CardSuit> args)
	{
		IconPath.Value = Utils.GetCardTexturePath(args.NewValue);
	}
}