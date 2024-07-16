using System;
using System.Collections.Generic;
using Godot;
using XCardGame.Scripts.Cards;
using XCardGame.Scripts.Common;
using XCardGame.Scripts.Common.Constants;
using XCardGame.Scripts.Common.DataBinding;
using XCardGame.Scripts.Game;

namespace XCardGame.Scripts.Ui;

public partial class CardEntry: BaseContentNode, ISelect
{

	public new class SetupArgs: BaseContentNode.SetupArgs
	{
		public bool OnlyDisplay;
	}
	
    public GameMgr GameMgr;
    public Battle Battle;
    public Sprite2D SuitIcon;
    public Label RankLabel;
    public Label JokerLabel;
    public Label NameLabel;
    public Line2D NegateLine;

    public BaseCard Card => (BaseCard)Content.Value;
    public Action<CardEntry> OnPressed;

    public bool OnlyDisplay;
    
    public Action OnSelected { get; }
    public bool IsSelected { get; set; }
    
    public override void _Ready()
    {
        base._Ready();
        GameMgr = GetNode<GameMgr>("/root/GameMgr");
        SuitIcon = GetNode<Sprite2D>("Suit");
        RankLabel = GetNode<Label>("Rank");
        JokerLabel = GetNode<Label>("Joker");
        NameLabel = GetNode<Label>("Name");
        NegateLine = GetNode<Line2D>("NegateLine");
        Area.InputEvent += InputEventHandler;
        OnlyDisplay = true;
        IsSelected = false;
    }
    
    public override void _Notification(int what)
    {
        if (what == NotificationPredelete && Content.Value != null)
        {
            Content.Value = null;
        }
    }

    public void Setup(SetupArgs args)
    {
        base.Setup(args);
        Battle = GameMgr.CurrentBattle;
        OnlyDisplay = args.OnlyDisplay;
        
        CurrentContainer.DetailedValueChanged += OnContainerChanged;
        CurrentContainer.FireValueChangeEventsOnInit();
    }
    
    public bool CanSelect()
    {
	    return true;
    }

    public void ToggleSelect(bool to)
    {
	    IsSelected = to;
    }
    
    protected void InputEventHandler(Node viewport, InputEvent @event, long shapeIdx)
    {
        if (@event is InputEventMouseButton mouseButton)
        {
            if (mouseButton.ButtonIndex == MouseButton.Left && mouseButton.Pressed)
            {
                GD.Print($"On mouse pressed {Content.Value}");
                OnPressed?.Invoke(this);
            }
        }
    }
    
    protected void OnCardRankChanged(object sender, ValueChangedEventDetailedArgs<Enums.CardRank> args)
    {
	    if (args.NewValue is Enums.CardRank.Joker)
	    {
		    RankLabel.Hide();
		    SuitIcon.Hide();
		    JokerLabel.Show();
	    }
	    else
	    {
		    RankLabel.Text = Utils.PrettyPrintCardRank(args.NewValue);
		    RankLabel.Show();
		    SuitIcon.Show();
		    JokerLabel.Hide();
	    }
    }
    
    protected void OnCardSuitChanged(object sender, ValueChangedEventDetailedArgs<Enums.CardSuit> args)
    {
	    if (args.NewValue is Enums.CardSuit.Joker)
	    {
		    RankLabel.Hide();
		    SuitIcon.Hide();
		    JokerLabel.Show();
	    }
	    else
	    {
		    SuitIcon.Texture = Utils.GetCardSuitTexture(args.NewValue);
		    RankLabel.Show();
		    SuitIcon.Show();
		    JokerLabel.Hide();
	    }
    }

	protected override void OnContentAttached(IContent content)
	{
		base.OnContentAttached(content);
		var card = (BaseCard)content;
		// GD.Print($"On card attached {card}");
		card.Setup(new BaseCard.SetupArgs
		{
			GameMgr = GameMgr,
			Battle = Battle,
			Node = this,
		});
		card.Nodes.Add(this);
		card.Rank.DetailedValueChanged += OnCardRankChanged;
		card.Rank.FireValueChangeEventsOnInit();
		card.Suit.DetailedValueChanged += OnCardSuitChanged;
		card.Suit.FireValueChangeEventsOnInit();
		if (!OnlyDisplay)
		{
			StartCard();
		}
	}

	protected override void OnContentDetached(IContent content)
	{
		base.OnContentDetached(content);
		var card = (BaseCard)content;
		// GD.Print($"On card detached {card}");
		card.Rank.DetailedValueChanged -= OnCardRankChanged;
		card.Suit.DetailedValueChanged -= OnCardSuitChanged;
		if (!OnlyDisplay)
		{
			StopCard();
		}
	}
	
	protected void OnContainerChanged(object sender,
		ValueChangedEventDetailedArgs<BaseContentContainer> args)
	{
		var oldValue = (CardContainer)args.OldValue;
		var newValue = (CardContainer)args.NewValue;
		if (oldValue is { OnlyDisplay: true } && newValue is not { OnlyDisplay: true })
		{
			OnlyDisplay = false;
			StopCard();
		}

		if (oldValue is not { OnlyDisplay: true } && newValue is { OnlyDisplay: true })
		{
			OnlyDisplay = true;
			StartCard();
		}
	}
	
	protected void OnToggleIsNegated(object sender, ValueChangedEventDetailedArgs<bool> args)
	{
		if (args.NewValue)
		{
			Modulate = Colors.DimGray;
			StartCard();
		}
		else
		{
			Modulate = Colors.White;
			StopCard();
		}
	}
	
	protected void StartCard()
	{
		Card?.OnStart(Card.Battle);
	}

	protected void StopCard()
	{
		Card?.OnStop(Card.Battle);
	}
}