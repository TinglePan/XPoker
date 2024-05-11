using System;
using System.Collections.Generic;
using Godot;
using XCardGame.Scripts.Cards;
using XCardGame.Scripts.Cards.AbilityCards;

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
    
    public ObservableProperty<bool> IsFocused;
    public ObservableProperty<bool> IsSelected;

    protected GameMgr GameMgr;
    
    public override void _ExitTree()
	{
	 	base._ExitTree();
	    Card.Value = null;
	}

	public override void _Ready()
	{
		GameMgr = GetNode<GameMgr>("/root/GameMgr");
		Card = new ObservableProperty<BaseCard>(nameof(Card), this, null);
		IsFocused = new ObservableProperty<bool>(nameof(IsFocused), this, false);
		IsSelected = new ObservableProperty<bool>(nameof(IsSelected), this, false);
		Card.DetailedValueChanged += OnCardChanged;
		MouseEntered += OnMouseEntered;
		MouseExited += OnMouseExited;
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

	public async void Reveal(float duration = 0f, float delay = 0f)
	{
		if (Card.Value.Face.Value != Enums.CardFace.Down) return;
		if (delay != 0f)
		{
			var timer = GetTree().CreateTimer(delay);
			await ToSignal(timer, Timer.SignalName.Timeout);
		}
		Tween tween = GetTree().CreateTween();
		var originalAlpha = Back.Modulate.A;
		tween.TweenProperty(Back, "modulate:a", 0f, Configuration.RevealFadeInDuration);
		await ToSignal(tween, Tween.SignalName.Finished);
		if (duration != 0f)
		{
			var timer = GetTree().CreateTimer(duration);
			await ToSignal(timer, Timer.SignalName.Timeout);
			tween = GetTree().CreateTween();
			tween.TweenProperty(Back, "modulate:a", originalAlpha, Configuration.RevealFadeOutDuration);
			await ToSignal(tween, Tween.SignalName.Finished);
		}
	}

	protected void OnMouseEntered()
	{
		GD.Print($"On mouse entered {Card.Value}");
		if (Card is { Value: not null })
		{
			IsFocused.Value = true;
		}
	}

	protected void OnMouseExited()
	{
		GD.Print($"On mouse exited {Card.Value}");
		if (Card is { Value: not null })
		{
			IsFocused.Value = false;
		}
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
		card.Rank.DetailedValueChanged += OnCardRankChanged;
		card.Rank.FireValueChangeEventsOnInit();
		card.Suit.DetailedValueChanged += OnCardSuitChanged;
		card.Suit.FireValueChangeEventsOnInit();
		if (card.TexturePath != null)
		{
			Icon.Texture = GD.Load<Texture2D>(card.TexturePath);
		}
	}

	protected void OnCardDetached(BaseCard card)
	{
		GD.Print($"On card detached {card}");
		if (card != null)
		{
			card.Face.DetailedValueChanged -= OnCardFaceChanged;
			card.Rank.DetailedValueChanged -= OnCardRankChanged;
			card.Suit.DetailedValueChanged -= OnCardSuitChanged;
		}
	}
}