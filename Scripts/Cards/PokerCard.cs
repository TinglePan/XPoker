using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using XCardGame.Scripts.Cards.CardMarkers;
using XCardGame.Scripts.Common;
using XCardGame.Scripts.Common.Constants;
using XCardGame.Scripts.Common.DataBinding;
using XCardGame.Scripts.Defs;
using XCardGame.Scripts.Defs.Def.Card;
using XCardGame.Scripts.Game;

using Battle = XCardGame.Scripts.Game.Battle;

namespace XCardGame.Scripts.Cards;

public class PokerCard: BaseCard
{
	
	public static string GetCardName(Enums.CardSuit suit, Enums.CardRank rank)
	{
		return $"{rank} of {suit}";
	}

	protected bool AlreadyFunctioning;
	
	public PokerCard(BaseCardDef def): base(def)
	{
		def.Name = GetCardName(def.Suit, def.Rank);
		def.DescriptionTemplate = def.Name;
		Suit.DetailedValueChanged += OnSuitChanged;
	}

	public override void OnStart(Battle battle)
	{
		base.OnStart(battle);
		if (IsFunctioning() && !AlreadyFunctioning)
		{
			AlreadyFunctioning = true;
		}
	}
	
	public override void OnStop(Battle battle)
	{
		base.OnStop(battle);
		if (AlreadyFunctioning)
		{
			AlreadyFunctioning = false;
		}
	}
	
	public override string ToString()
	{
		return Description();
	}
	
	public override string Description()
	{
		return string.Format(Def.DescriptionTemplate, Rank.Value, Suit.Value);
	}

	protected void OnSuitChanged(object sender, ValueChangedEventDetailedArgs<Enums.CardSuit> args)
	{
		IconPath.Value = Utils.GetCardTexturePath(args.NewValue);
	}
}