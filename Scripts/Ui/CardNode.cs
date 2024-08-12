using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Godot;
using XCardGame.CardProperties;
using XCardGame.Common;

namespace XCardGame.Ui;

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
    public DerivedObservableProperty<bool> IsEffective;
    
    public bool IsSelected { get; set; }
    public Action OnSelected { get; }
    
    public Vector2 InitPosition;

    // public bool PrintPosition = false;

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
		IsEffective = new DerivedObservableProperty<bool>(nameof(IsEffective), GetIsEffective,
			IsTapped, FaceDirection, CurrentContainer);
		IsEffective.DetailedValueChanged += OnIsEffectiveChanged;
		InitPosition = Position;
	}
	
	// public override void _Process(double delta)
	// {
	// 	if (PrintPosition)
	// 	{
	// 		GD.Print($"{this} position is {Position}:{GetParent()}/{GlobalPosition}");
	// 	}
	// }

	public override void Setup(object o)
    {
	    Battle = GameMgr.CurrentBattle;
	    base.Setup(o);
	    var args = (SetupArgs)o;
	    MainIcon.Setup(new IconWithTextFallback.SetupArgs
	    {
		    IconPath = Card?.IconPath,
		    DisplayName =  Card?.Def.Name,
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

	public bool GetIsEffective()
	{
		if (FaceDirection.Value == Enums.CardFace.Down) return false;
		if (IsTapped.Value) return false;
		if (CurrentContainer.Value is CardContainer { OnlyDisplay: true }) return false;
		return true;
	}
	
	public void OnIsEffectiveChanged(object sender, ValueChangedEventDetailedArgs<Lazy<bool>> args)
	{
		if (args.NewValue.Value)
		{
			StartCard();
		}
		else
		{
			StopCard();
		}
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

	public async Task AnimateReveal(bool to, float time)
	{
		if (IsRevealed.Value == to) return;
		var controlledTween = TweenControl.CreateTween("modulate_back", time);
		if (controlledTween != null)
		{
			var tween = controlledTween.Tween.Value;
			tween.TweenProperty(Back, "modulate:a", !to ? 1f : 0f, controlledTween.Time);
			await TweenControl.WaitComplete("modulate_back");
			IsRevealed.Value = to;
		}
		else
		{
			GD.Print("Animate reveal did not preempt");
		}
	}

	public async Task AnimateFlip(Enums.CardFace toFaceDir)
	{
		if (FaceDirection.Value == toFaceDir) return;
		AnimationPlayer.Play("flip");
		await ToSignal(AnimationPlayer, AnimationMixer.SignalName.AnimationFinished);
	}

	public async Task AnimateSelect(bool to, float time)
	{
		if (to == IsSelected) return;
		GD.Print($"Animate Select {this} to {to}");
		IsSelected = to;
		await AnimateLift(to, time);
		if (IsSelected)
		{
			OnSelected?.Invoke();
		}
	}

	public async Task AnimateLift(bool to, float time)
	{
		if ((Position == InitPosition) == to || TweenControl.IsRunning("position"))
		{
			var controlledTween = TweenControl.CreateTween("position", time, Configuration.CardLiftTweenPriority);
			if (controlledTween != null)
			{
				var tween = controlledTween.Tween.Value;
				var offset = Configuration.SelectedCardOffset;
				var targetPosition = to ? InitPosition + offset : InitPosition;
				// GD.Print($"Animate lift: {this} to {targetPosition}");
				tween.TweenProperty(this, "position", targetPosition, controlledTween.Time).SetTrans(Tween.TransitionType.Linear).SetEase(Tween.EaseType.Out);
				await TweenControl.WaitComplete("position");
			}
			else
			{
				// GD.Print("Animate lift did not preempt");
			}
		}
	}

	public void OnFlipAnimationToggleCardFace()
	{
		FaceDirection.Value = FaceDirection.Value == Enums.CardFace.Down ? Enums.CardFace.Up : Enums.CardFace.Down;
	}

	public async Task AnimateLeaveField()
	{
		Card.OnLeaveField();
		if (Card.Def is { IsExhaust: true })
		{
			await AnimateExhaust(Configuration.ExhaustAnimationTime);
		}
		else
		{
			await Battle.Dealer.AnimateDiscard(this);
		}
	}

	public async Task AnimateExhaust(float time)
	{
		GD.Print("Animate exhaust");
		var controlledTween = TweenControl.CreateTween("modulate", time);
		if (controlledTween != null)
		{
			var tween = controlledTween.Tween.Value;
			tween.TweenProperty(this, "modulate:a", 0f, controlledTween.Time);
			await TweenControl.WaitComplete("modulate");
			if (CurrentContainer != null)
			{
				CurrentContainer.Value.ContentNodes.Remove(this);
			}
			QueueFree();
		}
		else
		{
			GD.Print("Animate negate did not preempt");
		}
	}

	public async Task AnimateTap(bool to, float time)
	{
		if (IsTapped.Value == to) return;
		var controlledTween = TweenControl.CreateTween("rotation", time, Configuration.CardTapTweenPriority);
		if (controlledTween != null)
		{
			var tween = controlledTween.Tween.Value;
			var toDegrees = to ? 90f : 0;
			tween.TweenProperty(this, "rotation_degrees", toDegrees, controlledTween.Time).SetTrans(Tween.TransitionType.Linear).SetEase(Tween.EaseType.Out);
			await TweenControl.WaitComplete("rotation");
			IsTapped.Value = to;
		}
		else
		{
			GD.Print("Animate tap did not preempt");
		}
	}

	public async Task AnimateNegate(bool toState, float time)
	{
		if (Card.IsNegated.Value == toState) return;
		var controlledTween = TweenControl.CreateTween("modulate", time);
		if (controlledTween != null)
		{
			var tween = controlledTween.Tween.Value;
			tween.TweenProperty(this, "modulate", toState ? Colors.DimGray : Colors.White, controlledTween.Time).SetTrans(Tween.TransitionType.Linear).SetEase(Tween.EaseType.Out);
			await TweenControl.WaitComplete("modulate");
			Card.IsNegated.Value = toState;
		}
		else
		{
			GD.Print("Animate negate did not preempt");
		}
	}

	public override async Task AnimateTransform(Vector2 position, float rotationDegrees, float animationTime, 
		int priority = 0, Action callback = null,
		TweenControl.ConflictTweenAction conflictTweenAction = TweenControl.ConflictTweenAction.Interrupt)
	{
		await base.AnimateTransform(position, rotationDegrees, animationTime, priority, callback, conflictTweenAction);
		// GD.Print($"Animate transform: {this} to {position} res: {Position}");
		InitPosition = Position;
	}

	public void AdjustCostLabel()
	{
		var prop = Card.GetProp<BaseCardPropUsable>();
		if (prop is { Enabled: true })
		{
			if (!CostLabel.Visible)
			{
				CostLabel.Show();
			}
			prop.Cost.DetailedValueChanged += OnCardCostChanged;
			prop.Cost.FireValueChangeEventsOnInit();
		}
		else
		{
			CostLabel.Hide();
		}

	}

	protected void OnCardFaceChanged(object sender, ValueChangedEventDetailedArgs<Enums.CardFace> args)
	{
		if (args.NewValue == Enums.CardFace.Down)
		{
			Back.Show();
			Front.Hide();
		}
		else
		{
			Back.Hide();
			Front.Show();
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
    
    protected void OnCardCostChanged(object sender, ValueChangedEventDetailedArgs<int> args)
	{
	    CostLabel.Text = args.NewValue.ToString();
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
		AdjustCostLabel();
		MainIcon.DisplayName.Value = card.Def.Name;
		MainIcon.ResetIconPath(card.IconPath);
		if (IsEffective.Value)
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
		if (card.Def.IsUsable)
		{
			CostLabel.Hide();
			var prop = card.GetProp<BaseCardPropUsable>();
			prop.Cost.DetailedValueChanged -= OnCardCostChanged;
		}
		MainIcon.DisplayName.Value = null;
		MainIcon.ResetIconPath(null);
		if (!IsEffective.Value)
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
		}
		else
		{
			Modulate = Colors.White;
		}
	}
	
	protected void OnToggleIsRevealed(object sender, ValueChangedEventDetailedArgs<bool> args)
	{
		if (args.NewValue)
		{
			Back.Modulate = new Color(Back.Modulate.R, Back.Modulate.G, Back.Modulate.B, 0);
		}
		else
		{
			Back.Modulate = new Color(Back.Modulate.R, Back.Modulate.G, Back.Modulate.B, 1);
		}
	}

	protected void StartCard()
	{
		Card?.OnStartEffect();
	}

	protected void StopCard()
	{
		Card?.OnStopEffect();
	}
}