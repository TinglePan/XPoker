using System.Collections.Generic;
using Godot;
using XCardGame.Scripts.Cards;
using XCardGame.Scripts.Common;
using XCardGame.Scripts.Common.Constants;
using XCardGame.Scripts.Common.DataBinding;

namespace XCardGame.Scripts.Ui;

public partial class CardNode : Node, ISetup
{
	[Export] public Label SuitLabel;
	[Export] public Label RankLabel;

	[Export] public Control Front;
	[Export] public Control Back;

	public BaseCard Card;
	
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
		if (Card != null)
		{
			Card.Suit.DetailedValueChanged -= OnCardSuitChanged;
			Card.Face.DetailedValueChanged -= OnCardFaceChanged;
		}
	}

	public void Setup(Dictionary<string, object> args)
	{
		Card = args["card"] as BaseCard;
		if (Card != null)
		{
			Card.Suit.DetailedValueChanged += OnCardSuitChanged;
			Card.Face.DetailedValueChanged += OnCardFaceChanged;
			SuitLabel.Text = Utils.PrettyPrintCardSuit(Card.Suit.Value);
			RankLabel.Text = Utils.PrettyPrintCardRank(Card.Rank);
			if (Card.Face.Value == Enums.CardFace.Down)
			{
				Front.Visible = false;
				Back.Visible = true;
			}
			else
			{
				Front.Visible = true;
				Back.Visible = false;
			}
		}
	}

	public void OnCardSuitChanged(object sender, ValueChangedEventDetailedArgs<Enums.CardSuit> args)
	{
		SuitLabel.Text = Utils.PrettyPrintCardSuit(args.NewValue);
	}
	
	public void OnCardFaceChanged(object sender, ValueChangedEventDetailedArgs<Enums.CardFace> args)
	{
		if (args.NewValue == Enums.CardFace.Down)
		{
			Front.Visible = false;
			Back.Visible = true;
		}
		else
		{
			Front.Visible = true;
			Back.Visible = false;
		}
	}
}