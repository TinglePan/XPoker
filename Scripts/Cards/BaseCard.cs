using System;
using System.Collections.Generic;
using Godot;
using XCardGame.Scripts.Common.Constants;
using XCardGame.Scripts.Common.DataBinding;

namespace XCardGame.Scripts.Cards;

public class BaseCard: IComparable<BaseCard>
{
	public ObservableProperty<Enums.CardSuit> Suit;
	public Enums.CardColor CardColor;
	public Enums.CardRank Rank;
	public Enums.CardFace Face;

	public BaseCard(Enums.CardSuit cardSuit, Enums.CardRank rank, Enums.CardFace face)
	{
		Suit = new ObservableProperty<Enums.CardSuit>(nameof(Suit), cardSuit);
		Suit.DetailedValueChanged += OnSuitChanged;
		Suit.FireValueChangeEvents();
		Rank = rank;
		Face = face;
	}

	private void OnSuitChanged(object o, ValueChangedEventDetailedArgs<Enums.CardSuit> args)
	{
		switch (Suit.Value)
		{
			case Enums.CardSuit.Clubs:
			case Enums.CardSuit.Spades:
				CardColor = Enums.CardColor.Black;
				break;
			case Enums.CardSuit.Hearts:
			case Enums.CardSuit.Diamonds:
				CardColor = Enums.CardColor.Red;
				break;
			default:
				CardColor = Enums.CardColor.None;
				break;
		}
	}

	public virtual int CompareTo(BaseCard other)
	{
		return 0;
	}

	public override string ToString()
	{
		return $"{Rank} of {Suit.Value}, faced {Face}";
	}
}