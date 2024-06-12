using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using XCardGame.Scripts.Cards.CardMarkers;
using XCardGame.Scripts.Common;
using XCardGame.Scripts.Common.Constants;
using XCardGame.Scripts.Common.DataBinding;
using XCardGame.Scripts.GameLogic;

namespace XCardGame.Scripts.Cards;

public class MarkerCard: BaseCard
{
	public ObservableCollection<BaseCardMarker> Markers;
	
	public MarkerCard(string name, string description, string texturePath, Enums.CardSuit suit, Enums.CardRank rank,
		BattleEntity ownerEntity): base(name, description, texturePath, suit, rank, ownerEntity)
	{
		Markers = new ObservableCollection<BaseCardMarker>();
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
}