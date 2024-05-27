using System;
using System.Collections.Generic;
using Godot;
using XCardGame.Scripts.Cards;
using XCardGame.Scripts.Common;
using XCardGame.Scripts.Common.Constants;
using XCardGame.Scripts.Common.DataBinding;

namespace XCardGame.Scripts.Nodes;

public partial class CardNode: BaseContentNode<CardNode, BaseCard>
{
	[Export] public Node2D Front;
    [Export] public Node2D Back;

    [Export] public IconWithTextFallback MainIcon;
    
    [Export]
    public Sprite2D SuitIcon;
    [Export] public Label RankLabel;
    [Export] public Sprite2D JokerMark;
    [Export] public Label CostLabel;
    [Export] public AnimationPlayer AnimationPlayer;
    
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
		Area.InputEvent += InputEventHandler;
		FaceDirection = new ObservableProperty<Enums.CardFace>(nameof(FaceDirection), this, Enums.CardFace.Down);
		FaceDirection.DetailedValueChanged += OnCardFaceChanged;
		IsTapped = false;
		IsNegated = false;
		IsRevealed = false;
		IsSelected = false;
	}
	
	public override void _ExitTree()
	{
		base._ExitTree();
		if (Content.Value != null)
		{
			Content.Value.OnDisposal(Content.Value.Battle);
			Content.Value = null;
		}
	}

	public override void Setup(Dictionary<string, object> args)
    {
	    base.Setup(args);
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
		if (IsRevealed == toState) return;
		var tween = GetTree().CreateTween();
		tween.TweenProperty(Back, "modulate:a", !toState ? 1f : 0f, tweenTime);
		await ToSignal(tween, Tween.SignalName.Finished);
		IsRevealed = toState;
	}

	// public async void TweenFlip(Enums.CardFace toFaceDir, float tweenTime)
	// {
	// 	if (FaceDirection == toFaceDir) return;
	// 	var tween = GetTree().CreateTween();
	// 	tween.TweenProperty(this, "rotation:y", toFaceDir == Enums.CardFace.Down ? 180f : 0f, tweenTime);
	// 	await ToSignal(tween, Tween.SignalName.Finished);
	// 	FaceDirection = toFaceDir;
	// 	if (FaceDirection == Enums.CardFace.Down)
	// 	{
	// 		Content.Value.OnStop(Content.Value.Battle);
	// 	}
	// 	else
	// 	{
	// 		Content.Value.OnStart(Content.Value.Battle);
	// 	}
	// }

	public async void AnimateFlip(Enums.CardFace toFaceDir)
	{
		if (FaceDirection.Value == toFaceDir) return;
		AnimationPlayer.Play("flip");
		await ToSignal(AnimationPlayer, "animation_finished");
		if (FaceDirection.Value == Enums.CardFace.Down)
		{
			Content.Value.OnStop(Content.Value.Battle);
		}
		else
		{
			Content.Value.OnStart(Content.Value.Battle);
		}
		GD.Print($"Flip animation finished {FaceDirection}");
	}

	public void OnFlipAnimationToggleCardFace()
	{
		if (FaceDirection.Value == Enums.CardFace.Down)
		{
			FaceDirection.Value = Enums.CardFace.Up;
			Back.Hide();
			Front.Show();
		}
		else
		{
			FaceDirection.Value = Enums.CardFace.Down;
			Front.Hide();
			Back.Show();
		}
	}

	public async void TweenTap(bool toState, float tweenTime)
	{
		if (IsTapped == toState) return;
		var tween = GetTree().CreateTween();
		tween.TweenProperty(this, "rotation:z", toState ? -90f : 0, tweenTime);
		await ToSignal(tween, Tween.SignalName.Finished);
		IsTapped = toState;
	}

	public async void TweenNegate(bool toState, float tweenTime)
	{
		if (IsNegated == toState) return;
		var tween = GetTree().CreateTween();
		tween.TweenProperty(this, "modulate", toState ? Colors.DimGray : Colors.White, tweenTime);
		await ToSignal(tween, Tween.SignalName.Finished);
		IsNegated = toState;
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
		GD.Print($"On card attached {card}");
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
		GD.Print($"On card detached {card}");
		card.Rank.DetailedValueChanged -= OnCardRankChanged;
		card.Suit.DetailedValueChanged -= OnCardSuitChanged;
		MainIcon.ResetIconPath(null);
		card.OnStop(card.Battle);
	}
}