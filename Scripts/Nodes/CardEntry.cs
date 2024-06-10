using System;
using System.Collections.Generic;
using Godot;
using XCardGame.Scripts.Cards;
using XCardGame.Scripts.Common;
using XCardGame.Scripts.Common.Constants;
using XCardGame.Scripts.Common.DataBinding;
using XCardGame.Scripts.GameLogic;

namespace XCardGame.Scripts.Nodes;

public partial class CardEntry: BaseContentNode<CardEntry, BaseCard>
{
    public GameMgr GameMgr;
    public Battle Battle;
    public Sprite2D SuitIcon;
    public Label RankLabel;
    public Label JokerLabel;
    public Label NameLabel;
    public Line2D NegateLine;
    
    public Action<CardEntry> OnPressed;

    public bool IsNegated;
    public bool IsSelected;
    
    
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
        IsNegated = false;
        IsSelected = false;
    }
    
    public override void _Notification(int what)
    {
        if (what == NotificationPredelete && Content.Value != null)
        {
            Content.Value = null;
        }
    }

    public override void Setup(Dictionary<string, object> args)
    {
        base.Setup(args);
        Battle = GameMgr.CurrentBattle;
        Content.Value = (BaseCard)args["card"];
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
	    if (args.NewValue is Enums.CardRank.BlackJoker or Enums.CardRank.RedJoker or Enums.CardRank.RainbowJoker)
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
	    if (args.NewValue is Enums.CardSuit.RedJoker or Enums.CardSuit.BlackJoker or Enums.CardSuit.RainbowJoker)
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

	protected override void OnContentAttached(BaseCard card)
	{
		base.OnContentAttached(card);
		// GD.Print($"On card attached {card}");
		card.Setup(new Dictionary<string, object>()
		{
			{ "gameMgr", GameMgr },
			{ "battle", Battle },
			{ "node", this }
		});
		card.Nodes.Add(this);
		card.Rank.DetailedValueChanged += OnCardRankChanged;
		card.Rank.FireValueChangeEventsOnInit();
		card.Suit.DetailedValueChanged += OnCardSuitChanged;
		card.Suit.FireValueChangeEventsOnInit();
		card.OnStart(card.Battle);
	}

	protected override void OnContentDetached(BaseCard card)
	{
		// GD.Print($"On card detached {card}");
		card.Nodes.Remove(this);
		card.Rank.DetailedValueChanged -= OnCardRankChanged;
		card.Suit.DetailedValueChanged -= OnCardSuitChanged;
		card.OnStop(card.Battle);
	}
    
}