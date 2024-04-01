using System.Collections.Generic;
using Godot;
using XCardGame.Scripts.Common.Constants;
using XCardGame.Scripts.Common.DataBinding;

namespace XCardGame.Scripts.Cards;

public class BaseCard 
{
	public ObservableProperty<Enums.Suit> Suit;
	public Enums.Color Color;
	public Enums.Rank Rank;
	public Enums.CardFace Face;

	public BaseCard(Enums.Suit suit, Enums.Rank rank, Enums.CardFace face)
	{
		Suit = new ObservableProperty<Enums.Suit>(nameof(Suit), suit);
		Suit.DetailedValueChanged += OnSuitChanged;
		Suit.FireValueChangeEvents();
		Rank = rank;
		Face = face;
	}

	private void OnSuitChanged(object o, ValueChangedEventDetailedArgs<Enums.Suit> args)
	{
		switch (Suit.Value)
		{
			case Enums.Suit.Clubs:
			case Enums.Suit.Spades:
				Color = Enums.Color.Black;
				break;
			case Enums.Suit.Hearts:
			case Enums.Suit.Diamonds:
				Color = Enums.Color.Red;
				break;
			default:
				Color = Enums.Color.None;
				break;
		}
	}
}