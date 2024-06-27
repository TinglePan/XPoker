using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using XCardGame.Scripts.Cards.CardMarkers;
using XCardGame.Scripts.Common;
using XCardGame.Scripts.Common.Constants;
using XCardGame.Scripts.Common.DataBinding;
using XCardGame.Scripts.Defs;
using XCardGame.Scripts.Defs.Def.Card;
using XCardGame.Scripts.GameLogic;

namespace XCardGame.Scripts.Cards;

public class PokerCard: BaseCard
{
	
	public static string GetCardName(Enums.CardSuit suit, Enums.CardRank rank)
	{
		return $"{rank} of {suit}";
	}
	
	public ObservableCollection<BaseCardMarker> Markers;

	protected bool AlreadyFunctioning;
	
	public PokerCard(PokerCardDef def): base(def)
	{
		Markers = new ObservableCollection<BaseCardMarker>();
		def.Name = GetCardName(def.Suit, def.Rank);
		def.DescriptionTemplate = def.Name;
		Suit.DetailedValueChanged += OnSuitChanged;
	}

	public override void OnStart(Battle battle)
	{
		base.OnStart(battle);
		if (IsFunctioning() && !AlreadyFunctioning && Markers != null)
		{
			foreach (var marker in Markers)
			{
				marker.OnStart(battle);
			}

			AlreadyFunctioning = true;
		}
	}
	
	public override void OnStop(Battle battle)
	{
		base.OnStop(battle);
		if (AlreadyFunctioning && Markers != null)
		{
			foreach (var marker in Markers)
			{
				marker.OnStop(battle);
			}
		}
	}

	public override void Resolve(Battle battle, Engage engage, Enums.EngageRole role)
	{
		base.Resolve(battle, engage, role);
		foreach (var marker in Markers)
		{
			marker.Resolve(battle, engage, role);
		}
	}
	
	public override string ToString()
	{
		return $"{Rank.Value} of {Suit.Value}";
	}
	
	protected void OnSuitChanged(object sender, ValueChangedEventDetailedArgs<Enums.CardSuit> args)
	{
		IconPath.Value = Utils.GetCardTexturePath(args.NewValue);
	}
}