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

public partial class CardNode: BaseContentNode<CardNode, BaseCard>, ISelect
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
    public Node2D PriceNode;
    public Label PriceLabel;
    public AnimationPlayer AnimationPlayer;

    
    public Enums.CardFace OriginalFaceDirection;
    public ObservableProperty<Enums.CardFace> FaceDirection;
    public bool WithCardEffect;
    public bool IsSelected { get; set; }
    public Action OnSelected { get; }
    public ObservableProperty<bool> IsRevealed;
    public ObservableProperty<bool> IsTapped;
    public ObservableProperty<bool> IsBought;
    
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
		WithCardEffect = false;
		IsSelected = false;
		IsRevealed = new ObservableProperty<bool>(nameof(IsTapped), this, false);
		IsRevealed.DetailedValueChanged += OnToggleIsRevealed;
		IsTapped = new ObservableProperty<bool>(nameof(IsTapped), this, false);
		IsTapped.DetailedValueChanged += OnToggleIsTapped;
		Container.DetailedValueChanged += OnContainerChanged;
		InitPosition = Position;
	}

	public override void Setup(Dictionary<string, object> args)
    {
	    base.Setup(args);
	    Battle = GameMgr.CurrentBattle;
	    Content.Value = (BaseCard)args["card"];
	    MainIcon.Setup(new Dictionary<string, object>()
	    {
		    { "iconPath", Content.Value.IconPath },
		    { "displayName", Content.Value.Def.Name }
	    });
	    OriginalFaceDirection = (Enums.CardFace)args["faceDirection"];
	    FaceDirection.Value = OriginalFaceDirection;

	    if (args.ContainsKey("DisplayPrice"))
	    {
		    PriceNode = GetNode<Node2D>("Price");
		    PriceNode.Show();
		    PriceLabel = GetNode<Label>("Price/Value");
		    IsBought = new ObservableProperty<bool>(nameof(IsBought), this, false);
		    IsBought.DetailedValueChanged += OnIsBoughtChanged;
		    IsBought.FireValueChangeEventsOnInit();
	    }
    }
	
	public virtual bool CanSelect()
	{
		return true;
	}

	public void ToggleSelect(bool to)
	{
		IsSelected = to;
	}

	public void Reset(bool useTween = true)
	{
		Content.Value.Suit.Value = Content.Value.Def.Suit;
		Content.Value.Rank.Value = Content.Value.Def.Rank;
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
			IsRevealed.Value = false;
			FaceDirection.Value = OriginalFaceDirection;
			IsTapped.Value = false;
			Content.Value.IsNegated.Value = false;
		}
	}

	public async void TweenReveal(bool to, float tweenTime)
	{
		if (IsRevealed.Value == to) return;
		var newTween = CreateTween();
		newTween.TweenProperty(Back, "modulate:a", !to ? 1f : 0f, tweenTime);
		TweenControl.AddTween("IsRevealed", newTween, tweenTime);
		await ToSignal(newTween, Tween.SignalName.Finished);
		IsRevealed.Value = to;
	}

	public async void AnimateFlip(Enums.CardFace toFaceDir)
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

	public async Task AnimateSelectWithOrder(int order)
	{
		if (!IsSelected)
		{
			var newTween = CreateTween();
			var offset = Configuration.SelectedCardOffset;
			newTween.TweenProperty(this, "position", Position + offset, Configuration.SelectTweenTime).SetTrans(Tween.TransitionType.Linear).SetEase(Tween.EaseType.Out);
			await ToSignal(newTween, Tween.SignalName.Finished);
			IsSelected = true;
		}
	}

	public void OnFlipAnimationToggleCardFace()
	{
		FaceDirection.Value = FaceDirection.Value == Enums.CardFace.Down ? Enums.CardFace.Up : Enums.CardFace.Down;
	}

	public async void TweenTap(bool to, float tweenTime)
	{
		if (IsTapped.Value == to) return;
		var newTween = CreateTween();
		var offset = Configuration.SelectedCardOffset;
		newTween.TweenProperty(this, "position", to ? InitPosition + offset : InitPosition, tweenTime).SetTrans(Tween.TransitionType.Linear).SetEase(Tween.EaseType.Out);
		await ToSignal(newTween, Tween.SignalName.Finished);
		IsTapped.Value = to;
	}

	public async void TweenNegate(bool toState, float tweenTime)
	{
		if (Content.Value.IsNegated.Value == toState) return;
		var newTween = CreateTween();
		newTween.TweenProperty(this, "modulate", toState ? Colors.DimGray : Colors.White, tweenTime).SetTrans(Tween.TransitionType.Linear).SetEase(Tween.EaseType.Out);
		TweenControl.AddTween("negate", newTween, tweenTime);
		await ToSignal(newTween, Tween.SignalName.Finished);
		Content.Value.IsNegated.Value = toState;
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
		card.IsNegated.DetailedValueChanged += OnToggleIsNegated;
		card.IsNegated.FireValueChangeEventsOnInit();
		MainIcon.ResetIconPath(card.IconPath);
		StartCard();
	}
	

	protected override void OnContentDetached(BaseCard card)
	{
		// GD.Print($"On card detached {card}");
		card.Nodes.Remove(this);
		card.Rank.DetailedValueChanged -= OnCardRankChanged;
		card.Suit.DetailedValueChanged -= OnCardSuitChanged;
		card.IsNegated.DetailedValueChanged -= OnToggleIsNegated;
		MainIcon.ResetIconPath(null);
		StopCard();
	}

	protected void OnIsBoughtChanged(object sender, ValueChangedEventDetailedArgs<bool> args)
	{
		if (args.NewValue)
		{
			PriceLabel.Text = "Bought";
		}
		else
		{
			PriceLabel.Text = Content.Value.Def.BasePrice.ToString();
		}
	}

	protected void OnContainerChanged(object sender,
		ValueChangedEventDetailedArgs<ContentContainer<CardNode, BaseCard>> args)
	{
		var oldValue = (CardContainer)args.OldValue;
		var newValue = (CardContainer)args.NewValue;
		if (oldValue is { WithCardEffect: true } && newValue is not { WithCardEffect: true })
		{
			WithCardEffect = false;
			StopCard();
		}

		if (oldValue is not { WithCardEffect: true } && newValue is { WithCardEffect: true })
		{
			WithCardEffect = newValue.WithCardEffect;
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
		if (Content.Value is {} card)
		{
			card.OnStart(card.Battle);
		}
	}

	protected void StopCard()
	{
		if (Content.Value is {} card)
		{
			card.OnStop(card.Battle);
		}
	}
}