using System;
using System.Collections.Generic;
using Godot;
using XCardGame.Scripts.Cards;
using XCardGame.Scripts.Common;
using XCardGame.Scripts.Common.Constants;
using XCardGame.Scripts.Common.DataBinding;
using XCardGame.Scripts.GameLogic;

namespace XCardGame.Scripts.Nodes;

public partial class CardNode: BaseContentNode<CardNode, BaseCard>
{
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
    
    public Action<CardNode> OnPressed;

    public Enums.CardFace OriginalFaceDirection;
    public ObservableProperty<Enums.CardFace> FaceDirection;
    public bool IsTapped;
    public bool IsNegated;
    public bool IsRevealed;
    public bool IsSelected;


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
		Area.InputEvent += InputEventHandler;
		FaceDirection = new ObservableProperty<Enums.CardFace>(nameof(FaceDirection), this, Enums.CardFace.Down);
		FaceDirection.DetailedValueChanged += OnCardFaceChanged;
		IsTapped = false;
		IsNegated = false;
		IsRevealed = false;
		IsSelected = false;
	}
	
	public override void _Notification(int what)
	{
		if (what == NotificationPredelete && Content.Value != null)
		{
			var card = Content.Value;
			Content.Value = null;
			card.OnDisposal(card.Battle);
		}
	}

	public override void Setup(Dictionary<string, object> args)
    {
	    base.Setup(args);
	    Battle = GameMgr.CurrentBattle;
	    Content.Value = (BaseCard)args["card"];
	    MainIcon.Setup(new Dictionary<string, object>()
	    {
		    { "iconPath", Content.Value.IconPath },
		    { "displayName", Content.Value.Name }
	    });
	    OriginalFaceDirection = (Enums.CardFace)args["faceDirection"];
	    FaceDirection.Value = OriginalFaceDirection;
	    FaceDirection.FireValueChangeEventsOnInit();
    }

	public void Reset(bool useTween = true)
	{
		Content.Value.Suit.Value = Content.Value.OriginalSuit;
		Content.Value.Rank.Value = Content.Value.OriginalRank;
		IsSelected = false;
		if (useTween)
		{
			TweenReveal(false, Configuration.RevealTweenTime);
			// TweenFlip(OriginalFaceDirection, Configuration.FlipTweenTime);
			AnimateFlip(OriginalFaceDirection);
			TweenTap(false, Configuration.TapTweenTime);
			TweenNegate(false, Configuration.NegateTweenTime);
		}
		else
		{
			IsRevealed = false;
			FaceDirection.Value = OriginalFaceDirection;
			IsTapped = false;
			IsNegated = false;
		}
	}

	public async void TweenReveal(bool toState, float tweenTime)
	{
		void UpdateRevealed()
		{
			IsRevealed = toState;
		}
		if (IsRevealed == toState) return;
		var newTween = CreateTween();
		newTween.TweenProperty(Back, "modulate:a", !toState ? 1f : 0f, tweenTime);
		TweenControl.AddTween("IsRevealed", newTween, tweenTime, UpdateRevealed);
		await ToSignal(TweenControl.TweenMap["isRevealed"].Tween.Value, Tween.SignalName.Finished);
	}

	public async void AnimateFlip(Enums.CardFace toFaceDir, float delay = 0f)
	{
		if (FaceDirection.Value == toFaceDir) return;
		if (delay > 0f)
		{
			var timer = GetTree().CreateTimer(delay);
			await ToSignal(timer, Timer.SignalName.Timeout);
		}
		AnimationPlayer.Play("flip");
		if (toFaceDir == Enums.CardFace.Down)
		{
			Content.Value.OnStop(Content.Value.Battle);
		}
		else
		{
			Content.Value.OnStart(Content.Value.Battle);
		}
		await ToSignal(AnimationPlayer, AnimationMixer.SignalName.AnimationFinished);
	}

	public void OnFlipAnimationToggleCardFace()
	{
		FaceDirection.Value = FaceDirection.Value == Enums.CardFace.Down ? Enums.CardFace.Up : Enums.CardFace.Down;
	}

	public async void TweenTap(bool toState, float tweenTime, float delay = 0f)
	{
		if (IsTapped == toState) return;
		if (delay > 0f)
		{
			var timer = GetTree().CreateTimer(delay);
			await ToSignal(timer, Timer.SignalName.Timeout);
		}
		IsTapped = toState;
		var newTween = CreateTween();
		newTween.TweenProperty(this, "rotation_degrees", toState ? 90f : 0, tweenTime).SetTrans(Tween.TransitionType.Linear).SetEase(Tween.EaseType.Out);
		TweenControl.AddTween("tap", newTween, tweenTime);
		await ToSignal(newTween, Tween.SignalName.Finished);
	}

	public async void TweenNegate(bool toState, float tweenTime, float delay = 0f)
	{
		if (IsNegated == toState) return;
		if (delay > 0f)
		{
			var timer = GetTree().CreateTimer(delay);
			await ToSignal(timer, Timer.SignalName.Timeout);
		}
		IsNegated = toState;
		var newTween = CreateTween();
		newTween.TweenProperty(this, "modulate", toState ? Colors.DimGray : Colors.White, tweenTime).SetTrans(Tween.TransitionType.Linear).SetEase(Tween.EaseType.Out);
		TweenControl.AddTween("negate", newTween, tweenTime);
		await ToSignal(newTween, Tween.SignalName.Finished);
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
	    if (args.NewValue is Enums.CardRank.BlackJoker or Enums.CardRank.RedJoker or Enums.CardRank.RainbowJoker)
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
	    if (args.NewValue is Enums.CardSuit.RedJoker or Enums.CardSuit.BlackJoker or Enums.CardSuit.RainbowJoker)
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
		card.Node = this;
		card.Rank.DetailedValueChanged += OnCardRankChanged;
		card.Rank.FireValueChangeEventsOnInit();
		card.Suit.DetailedValueChanged += OnCardSuitChanged;
		card.Suit.FireValueChangeEventsOnInit();
		MainIcon.ResetIconPath(card.IconPath);
		card.OnStart(card.Battle);
	}

	protected override void OnContentDetached(BaseCard card)
	{
		// GD.Print($"On card detached {card}");
		card.Rank.DetailedValueChanged -= OnCardRankChanged;
		card.Suit.DetailedValueChanged -= OnCardSuitChanged;
		MainIcon.ResetIconPath(null);
		card.OnStop(card.Battle);
	}
}