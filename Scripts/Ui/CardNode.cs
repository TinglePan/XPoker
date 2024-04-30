using System;
using System.Collections.Generic;
using Godot;
using XCardGame.Scripts.Cards;
using XCardGame.Scripts.Cards.AbilityCards;
using XCardGame.Scripts.Cards.PokerCards;
using XCardGame.Scripts.Common;
using XCardGame.Scripts.Common.Constants;
using XCardGame.Scripts.Common.DataBinding;
using XCardGame.Scripts.InputHandling;

namespace XCardGame.Scripts.Ui;

public partial class CardNode: Control, ISetup
{
	[Export] public Control Front;
    [Export] public Control Back;
    
    
    [Export]
    public TextureRect Icon;
    
    [Export] public Label SuitLabel;
    [Export] public Label RankLabel;
 
    public ObservableProperty<BaseCard> Card;
    public CardContainer Container;
    public Action<CardNode> OnPressed;

    protected GameMgr GameMgr;
    protected ObservableProperty<bool> IsFocused;
    
    
    public override void _ExitTree()
	{
	 	base._ExitTree();
	    Card.Value = null;
	}

	public override void _Ready()
	{
		GameMgr = GetNode<GameMgr>("/root/GameMgr");
		Card = new ObservableProperty<BaseCard>(nameof(Card), this, null);
		Card.DetailedValueChanged += OnCardChanged;
		IsFocused = new ObservableProperty<bool>(nameof(IsFocused), this, false);
		IsFocused.DetailedValueChanged += (sender, args) =>
		{
			if (Card is { Value: { } card })
			{
				if (args.NewValue)
				{
					card.OnFocused();
				}
				else
				{
					card.OnLoseFocus();
				}
			}
		};
	}

	public override void _GuiInput(InputEvent @event)
	{
		if (@event is InputEventMouseButton mouseButton)
		{
			if (mouseButton.ButtonIndex == MouseButton.Left && mouseButton.Pressed)
			{
				OnPressed?.Invoke(this);
			}
		}
	}

	public virtual void Setup(Dictionary<string, object> args)
    {
	    if (args["card"] is BaseCard card)
     	{
	        Card.Value = card;
	        card.Node = this;
        }

	    if (args.ContainsKey("container") && args["container"] is CardContainer container)
	    {
		    Container = container;
	    }
    }

	public void SwapCard(CardNode other)
	{
		var card = Card.Value;
		var otherCard = other.Card.Value;
		if (card != null)
		{
			card.Node = null;
		}
		if (otherCard != null)
		{
			otherCard.Node = null;
		}
		Card.Value = otherCard;
		other.Card.Value = card;
	}
    
    protected void OnCardFaceChanged(object sender, ValueChangedEventDetailedArgs<Enums.CardFace> args)
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
    
    protected void OnCardSuitChanged(object sender, ValueChangedEventDetailedArgs<Enums.CardSuit> args)
    {
	    SuitLabel.Text = Utils.PrettyPrintCardSuit(args.NewValue);
    }
    
    protected void OnCardRankChanged(object sender, ValueChangedEventDetailedArgs<Enums.CardRank> args)
	{
	    RankLabel.Text = Utils.PrettyPrintCardRank(args.NewValue);
	}
    
	protected void OnCardChanged(object sender, ValueChangedEventDetailedArgs<BaseCard> args)
	{
		if (args.OldValue != null) OnCardDetached(args.OldValue);
		if (args.NewValue != null) OnCardAttached(args.NewValue);
	}

	protected void OnCardAttached(BaseCard card)
	{
		GD.Print($"On card attached {card}");
		card.Node = this;
		card.Face.DetailedValueChanged += OnCardFaceChanged;
		card.Face.FireValueChangeEventsOnInit();
		if (card is BasePokerCard pokerCard)
		{
			pokerCard.Rank.DetailedValueChanged += OnCardRankChanged;
			pokerCard.Rank.FireValueChangeEventsOnInit();
			pokerCard.Suit.DetailedValueChanged += OnCardSuitChanged;
			pokerCard.Suit.FireValueChangeEventsOnInit();
		}
		else if (card is BaseAbilityCard abilityCard)
		{
			Icon.Texture = GD.Load<Texture2D>(abilityCard.IconPath);
		}
	}

	protected void OnCardDetached(BaseCard card)
	{
		GD.Print($"On card detached {card}");
		if (card != null)
		{
			card.Face.DetailedValueChanged -= OnCardFaceChanged;
			if (card is BasePokerCard pokerCard)
			{
				pokerCard.Rank.DetailedValueChanged -= OnCardRankChanged;
				pokerCard.Suit.DetailedValueChanged -= OnCardSuitChanged;
			} else if (card is BaseAbilityCard _)
			{
				Icon.Texture = null;
			}
			card.Node = null;
		}
	}
}