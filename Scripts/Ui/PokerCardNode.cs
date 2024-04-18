using System.Collections.Generic;
using Godot;
using XCardGame.Scripts.Cards;
using XCardGame.Scripts.Cards.PokerCards;
using XCardGame.Scripts.Common;
using XCardGame.Scripts.Common.Constants;
using XCardGame.Scripts.Common.DataBinding;

namespace XCardGame.Scripts.Ui;

public partial class PokerCardNode : CardNode
{
	[Export] public Label SuitLabel;
	[Export] public Label RankLabel;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}
	

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public override void _ExitTree()
	{
		base._ExitTree();
		if (Card is { Value: BasePokerCard pokerCard } )
		{
			pokerCard.Suit.DetailedValueChanged -= OnCardSuitChanged;
		}
	}

	public void OnCardSuitChanged(object sender, ValueChangedEventDetailedArgs<Enums.CardSuit> args)
	{
		SuitLabel.Text = Utils.PrettyPrintCardSuit(args.NewValue);
	}

	public override void OnCardChanged(object sender, ValueChangedEventDetailedArgs<BaseCard> args)
	{
		base.OnCardChanged(sender, args);
		if (Card is { Value: BasePokerCard pokerCard })
		{
			pokerCard.Suit.DetailedValueChanged += OnCardSuitChanged;
			pokerCard.Suit.FireValueChangeEventsOnInit();
			RankLabel.Text = Utils.PrettyPrintCardRank(pokerCard.Rank);
		}
	}
}