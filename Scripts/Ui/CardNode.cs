using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Godot;
using XCardGame.Scripts.Cards;
using XCardGame.Scripts.Common;
using XCardGame.Scripts.Common.Constants;
using XCardGame.Scripts.Common.DataBinding;
using XCardGame.Scripts.Game;

namespace XCardGame.Scripts.Ui;

public partial class CardNode: BaseContentNode, ISelect
{
	public new class SetupArgs: BaseContentNode.SetupArgs
	{
		public Enums.CardFace FaceDirection;
		public bool OnlyDisplay;
	}
	
	public GameMgr GameMgr;
	public Battle Battle;
	public Node2D Front;
    public Node2D Back;
    public IconWithTextFallback MainIcon;
    public Sprite2D SuitIcon;
    public Label RankLabel;
    public Sprite2D JokerMark;
    public Label CostLabel;
    public AnimationPlayer AnimationPlayer;
    
    public BaseCard Card => (BaseCard)Content.Value;
    public Enums.CardFace OriginalFaceDirection;
    public ObservableProperty<Enums.CardFace> FaceDirection;
    public bool OnlyDisplay;
    public ObservableProperty<bool> IsRevealed;
    public ObservableProperty<bool> IsTapped;

    private ObservableProperty<bool> _isSelected;
    public bool IsSelected { get; set; }
    public Action OnSelected { get; }
    
    protected Vector2 InitPosition;

	public override void _Ready()
	{
		base._Ready();
		GameMgr = GetNode<GameMgr>("/root/GameMgr");
		Front = GetNode<Node2D>("Outline/Front");
		Back = GetNode<Node2D>("Outline/Back");
		MainIcon = GetNode<IconWithTextFallback>("Outline/Front/IconWithTextFallback");
		SuitIcon = GetNode<Sprite2D>("Outline/Front/Suit");
		RankLabel = GetNode<Label>("Outline/Front/Rank");
		JokerMark = GetNode<Sprite2D>("Outline/Front/Joker");
		CostLabel = GetNode<Label>("Outline/Front/Cost");
		AnimationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
		
		FaceDirection = new ObservableProperty<Enums.CardFace>(nameof(FaceDirection), this, Enums.CardFace.Down);
		FaceDirection.DetailedValueChanged += OnCardFaceChanged;
		FaceDirection.FireValueChangeEventsOnInit();
		OnlyDisplay = false;
		IsSelected = false;
		IsRevealed = new ObservableProperty<bool>(nameof(IsTapped), this, false);
		IsRevealed.DetailedValueChanged += OnToggleIsRevealed;
		IsTapped = new ObservableProperty<bool>(nameof(IsTapped), this, false);
		IsTapped.DetailedValueChanged += OnToggleIsTapped;
		InitPosition = Position;
	}

	public void Setup(SetupArgs args)
    {
	    base.Setup(args);
	    Battle = GameMgr.CurrentBattle;
	    MainIcon.Setup(new IconWithTextFallback.SetupArgs
	    {
		    IconPath = Card.IconPath,
		    DisplayName =  Card.Def.Name,
	    });
	    OriginalFaceDirection = args.FaceDirection;
	    FaceDirection.Value = OriginalFaceDirection;
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

	public async void Reset(bool useTween = true)
	{
		Card.Suit.Value = Card.Def.Suit;
		Card.Rank.Value = Card.Def.Rank;
		IsSelected = false;
		if (useTween)
		{
			var tasks = new List<Task>
			{
				AnimateReveal(false, Configuration.RevealTweenTime),
				AnimateFlip(OriginalFaceDirection),
				AnimateTap(false, Configuration.TapTweenTime),
				AnimateNegate(false, Configuration.NegateTweenTime)
			};
			await Task.WhenAll(tasks);
		}
		else
		{
			IsRevealed.Value = false;
			FaceDirection.Value = OriginalFaceDirection;
			IsTapped.Value = false;
			Card.IsNegated.Value = false;
		}
	}

	public async Task AnimateReveal(bool to, float tweenTime)
	{
		if (IsRevealed.Value == to) return;
		var newTween = CreateTween();
		newTween.TweenProperty(Back, "modulate:a", !to ? 1f : 0f, tweenTime);
		TweenControl.AddTween("IsRevealed", newTween, tweenTime);
		await ToSignal(newTween, Tween.SignalName.Finished);
		IsRevealed.Value = to;
	}

	public async Task AnimateFlip(Enums.CardFace toFaceDir)
	{
		if (FaceDirection.Value == toFaceDir) return;
		AnimationPlayer.Play("flip");
		if (toFaceDir == Enums.CardFace.Down)
		{
			StopCard();
		}
		else
		{
			StartCard();
		}
		await ToSignal(AnimationPlayer, AnimationMixer.SignalName.AnimationFinished);
	}

	public async Task AnimateSelect(bool to, float time)
	{
		if (IsSelected != to)
		{
			await AnimateLift(to, time);
			IsSelected = to;
		}
	}

	public async Task AnimateLift(bool to, float time)
	{
		if (Position == InitPosition ^ to)
		{
			var newTween = CreateTween();
			var offset = Configuration.SelectedCardOffset;
			newTween.TweenProperty(this, "position", to ? InitPosition + offset : InitPosition, time).SetTrans(Tween.TransitionType.Linear).SetEase(Tween.EaseType.Out);
			TweenControl.AddTween("transform", newTween, time);
			await ToSignal(newTween, Tween.SignalName.Finished);
		}
	}

	public void OnFlipAnimationToggleCardFace()
	{
		FaceDirection.Value = FaceDirection.Value == Enums.CardFace.Down ? Enums.CardFace.Up : Enums.CardFace.Down;
	}

	public async Task AnimateTap(bool to, float tweenTime)
	{
		if (IsTapped.Value == to) return;
		var newTween = CreateTween();
		newTween.TweenProperty(this, "rotation_degrees", to ? 90f : 0, tweenTime).SetTrans(Tween.TransitionType.Linear).SetEase(Tween.EaseType.Out);
		TweenControl.AddTween("tap", newTween, tweenTime);
		await ToSignal(newTween, Tween.SignalName.Finished);
		IsTapped.Value = to;
	}

	public async Task AnimateNegate(bool toState, float tweenTime)
	{
		if (Card.IsNegated.Value == toState) return;
		var newTween = CreateTween();
		newTween.TweenProperty(this, "modulate", toState ? Colors.DimGray : Colors.White, tweenTime).SetTrans(Tween.TransitionType.Linear).SetEase(Tween.EaseType.Out);
		TweenControl.AddTween("negate", newTween, tweenTime);
		await ToSignal(newTween, Tween.SignalName.Finished);
		Card.IsNegated.Value = toState;
	}

	protected void OnCardFaceChanged(object sender, ValueChangedEventDetailedArgs<Enums.CardFace> args)
	{
		if (args.NewValue == Enums.CardFace.Down)
		{
			Back.Show();
			Front.Hide();
			StartCard();
		}
		else
		{
			Back.Hide();
			Front.Show();
			StopCard();
		}
	}
    
    protected void OnCardRankChanged(object sender, ValueChangedEventDetailedArgs<Enums.CardRank> args)
    {
	    if (args.NewValue is Enums.CardRank.Joker)
	    {
		    RankLabel.Hide();
		    SuitIcon.Hide();
		    JokerMark.Show();
	    }
	    else
	    {
		    RankLabel.Text = Utils.PrettyPrintCardRank(args.NewValue);
		    RankLabel.Show();
		    SuitIcon.Show();
		    JokerMark.Hide();
	    }
    }
    
    protected void OnCardSuitChanged(object sender, ValueChangedEventDetailedArgs<Enums.CardSuit> args)
    {
	    if (args.NewValue is Enums.CardSuit.Joker)
	    {
		    RankLabel.Hide();
		    SuitIcon.Hide();
		    JokerMark.Show();
	    }
	    else
	    {
		    SuitIcon.Texture = Utils.GetCardSuitTexture(args.NewValue);
		    RankLabel.Show();
		    SuitIcon.Show();
		    JokerMark.Hide();
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
		card.Rank.DetailedValueChanged += OnCardRankChanged;
		card.Rank.FireValueChangeEventsOnInit();
		card.Suit.DetailedValueChanged += OnCardSuitChanged;
		card.Suit.FireValueChangeEventsOnInit();
		card.IsNegated.DetailedValueChanged += OnToggleIsNegated;
		card.IsNegated.FireValueChangeEventsOnInit();
		MainIcon.ResetIconPath(card.IconPath);
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
		card.IsNegated.DetailedValueChanged -= OnToggleIsNegated;
		MainIcon.ResetIconPath(null);
		if (!OnlyDisplay)
		{
			card.OnStop(card.Battle);
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

	protected void OnToggleIsTapped(object sender, ValueChangedEventDetailedArgs<bool> args)
	{
		if (args.NewValue)
		{
			RotationDegrees = 90f;
			StartCard();
		}
		else
		{
			RotationDegrees = 0;
			StopCard();
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
	
	protected void OnToggleIsRevealed(object sender, ValueChangedEventDetailedArgs<bool> args)
	{
		if (args.NewValue)
		{
			Modulate = new Color(Modulate.R, Modulate.G, Modulate.B, 0);
		}
		else
		{
			Modulate = new Color(Modulate.R, Modulate.G, Modulate.B, 1);
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