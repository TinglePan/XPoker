using System;
using System.Collections.Generic;
using Godot;
using XCardGame.Scripts.Cards.SpecialCards;
using XCardGame.Scripts.Common;
using XCardGame.Scripts.Common.Constants;
using XCardGame.Scripts.GameLogic;
using XCardGame.Scripts.InputHandling;
using XCardGame.Scripts.Ui;

namespace XCardGame.Scripts;

public partial class GameMgr : Node
{
	[Export] public PackedScene MainScene;
	[Export] public PackedScene PlayerPrefab;
	
	public GameLogic.PokerPlayer PlayerControlledPlayer;
	
	public Node CurrentScene;
	public GameLogic.Hand CurrentHand;
	public ActionUi ActionUi;
	public InputMgr InputMgr;

	public Random Rand;

	private bool IsGameStarted;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		IsGameStarted = false;
		Rand = new Random();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (!IsGameStarted)
		{
			StartGame();
			IsGameStarted = true;
		}
	}

	public void StartGame()
	{
		ChangeScene(MainScene);
		InputMgr = GetNode<InputMgr>("/root/InputMgr");
		ActionUi = GetNode<ActionUi>("/root/Main/BottomBoxUi/ActionUi");
		ActionUi.Hide();
		ActionUi.SetProcess(false);
		StartHand();
		var dbgButton = GetNode<Button>("/root/Main/Button");
		dbgButton.Pressed += () => CurrentHand.Start();
	}

	public void StartHand()
	{
		CurrentHand = new GameLogic.Hand();
		AddChild(CurrentHand);
		
		var communityCardContainer = GetNode<CardContainer>("/root/Main/CommunityCardContainer");
		communityCardContainer.Setup(new Dictionary<string, object>
		{
			{ "cards", CurrentHand.CommunityCards }
		});
		PlayerControlledPlayer = Utils.InstantiatePrefab(PlayerPrefab, CurrentHand) as GameLogic.PokerPlayer;
		PlayerControlledPlayer?.Setup(new Dictionary<string, object>()
		{
			{ "creature", new Creature("you", 1000) },
			{ "hand", CurrentHand },
			{ "brainScriptPath", "res://Scripts/Brain/PlayerBrain.cs" }
		});
		var playerTab = GetNode<PlayerTab>("/root/Main/Player");
		playerTab.Setup(new Dictionary<string, object>()
		{
			{ "player", PlayerControlledPlayer }
		});
		var opponent = Utils.InstantiatePrefab(PlayerPrefab, CurrentHand) as GameLogic.PokerPlayer;
		opponent?.Setup(new Dictionary<string, object>()
		{
			{ "creature", new Creature("cpu", 1000) },
			{ "hand", CurrentHand },
			{ "brainScriptPath", "res://Scripts/Brain/Ai/AllStrategyAi.cs" },
			{ "openRangeMin", CurrentHand.BigBlindAmount * 2 },
			{ "openRangeMax", CurrentHand.BigBlindAmount * 4 },
			{ "raisePercentageRangeMin", 0.33f },
			{ "raisePercentageRangeMax", 1.5f },
		});
		var opponentTab = GetNode<PlayerTab>("/root/Main/Opponent");
		opponentTab.Setup(new Dictionary<string, object>()
		{
			{ "player", opponent }
		});
		InputMgr.SwitchToInputHandler(new MainInputHandler(this, playerTab.SpecialCardContainer));
		if (PlayerControlledPlayer != null) {
			PlayerControlledPlayer.SpecialCards.Add(
				new D6Card(this, playerTab.HoleCardContainer, PlayerControlledPlayer, Enums.CardFace.Up));
			PlayerControlledPlayer.SpecialCards.Add(
				new NetherSwapCard(this, playerTab.HoleCardContainer, opponentTab.HoleCardContainer, 
					communityCardContainer, PlayerControlledPlayer, Enums.CardFace.Up));
		}
		CurrentHand.Setup(new Dictionary<string, object>()
		{
			{ "players", new List<GameLogic.PokerPlayer>
				{
					PlayerControlledPlayer,
					opponent
				} 
			}
		});
		CurrentHand.Finished += () =>
		{
			PlayerControlledPlayer?.ResetHandState();
			CurrentHand.Reset();
			opponent?.ResetHandState();
		};
		// CurrentMatch.Run();
	}

	public void ChangeScene(PackedScene scene)
	{
		var root = GetTree().Root;
		var node = Utils.InstantiatePrefab(scene, root);
		if (CurrentScene != null)
		{
			root.RemoveChild(CurrentScene);
			CurrentScene.QueueFree();
		}
		CurrentScene = node;
	}

	public void OpenUi<T>(T target, Dictionary<string, object> context) where T: Control, ISetup
	{
		target.SetProcess(true);
		target.Show();
		target.Setup(context);
	}
	
	public void CloseUi(Control target)
	{
		target.Hide();
		target.SetProcess(false);
	}
}