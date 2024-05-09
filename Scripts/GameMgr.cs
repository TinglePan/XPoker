using System;
using System.Collections.Generic;
using System.Diagnostics;
using Godot;
using XCardGame.Scripts.Cards.AbilityCards;
using XCardGame.Scripts.Common;
using XCardGame.Scripts.Common.Constants;
using XCardGame.Scripts.Defs;
using XCardGame.Scripts.GameLogic;
using XCardGame.Scripts.InputHandling;
using XCardGame.Scripts.Ui;

namespace XCardGame.Scripts;

public partial class GameMgr : Node
{
	[Export] public PackedScene MainScene;
	[Export] public PackedScene PlayerPrefab;
	[Export] public PackedScene OpponentPrefab;
	
	public InputMgr InputMgr;
	public UiMgr UiMgr;
	public Node CurrentScene;
	public Battle CurrentBattle;

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
		InputMgr ??= GetNode<InputMgr>("/root/InputMgr");
		UiMgr ??= GetNode<UiMgr>("/root/UiMgr");
		var setupButton = GetNode<Button>("/root/Main/SetupButton");
		setupButton.Pressed += SetupBattle;
		var startButton = GetNode<Button>("/root/Main/StartButton");
		startButton.Pressed += () => CurrentBattle.Start();
		var nextRoundButton = GetNode<Button>("/root/Main/NextRoundButton");
		nextRoundButton.Pressed += () =>
		{
			CurrentBattle.NewRound();
			InputMgr.SwitchToInputHandler(new MainInputHandler(this));
		};
		var proceedButton = GetNode<Button>("/root/Main/ProceedButton");
		proceedButton.Pressed += () =>
		{
			CurrentBattle.ShowDown();
			InputMgr.SwitchToInputHandler(new MainInputHandler(this));
		};
	}

	public void SetupBattle()
	{
		void MockSetup()
		{
			var player = Utils.InstantiatePrefab(PlayerPrefab, CurrentBattle) as PlayerBattleEntity;
			Debug.Assert(player != null);
			player.Setup(new Dictionary<string, object>()
			{
				{ "name", "you" },
				{ "battle", CurrentBattle },
				{ "deck", Decks.PlayerInitialDeck },
				{ "damageTable", DamageTables.DefaultPlayerDamageTable },
				{ "maxMorale", 20 }
			});
			var enemy = Utils.InstantiatePrefab(OpponentPrefab, CurrentBattle) as BattleEntity;
			Debug.Assert(enemy != null);
			enemy.Setup(new Dictionary<string, object>()
			{
				{ "name", "cpu"},
				{ "battle", CurrentBattle },
				{ "deck", Decks.OpponentInitialDeck },
				{ "factionId", Enums.FactionId.Opponent },
				{ "damageTable", DamageTables.DefaultOpponentDamageTable },
				{ "maxMorale", 20 }
			});
			CurrentBattle.Setup(new Dictionary<string, object>()
			{
				{ "entities", new List<BattleEntity>
					{
						player,
						enemy
					}
				},
				{ "player", player }
			});
			// GD.Print("open player");
			UiMgr.OpenBattleEntityUiCollection(player);
			// GD.Print("open enemy");
			UiMgr.OpenBattleEntityUiCollection(enemy);
			UiMgr.OpenCommunityCardContainer(CurrentBattle.CommunityCards);
		
			// Add ability cards after player ui collection is set up to avoid firing init event for observable collection.
			player.AbilityCards.Add(new D6Card(this, Enums.CardFace.Up, Enums.CardSuit.Diamonds, player));
			player.AbilityCards.Add(new NetherSwapCard(this, Enums.CardFace.Up, Enums.CardSuit.Spades, player));

			InputMgr.SwitchToInputHandler(new MainInputHandler(this));
		
		}
		
		if (CurrentBattle == null)
		{
			CurrentBattle = new Battle();
			AddChild(CurrentBattle);
		}
		MockSetup();
		CurrentBattle.Start();
		
		// CurrentMatch.Run();
	}

	public void ChangeScene(PackedScene scene)
	{
		GD.Print("Changing scene...");
		var root = GetTree().Root;
		var node = Utils.InstantiatePrefab(scene, root);
		if (CurrentScene != null)
		{
			root.RemoveChild(CurrentScene);
			CurrentScene.QueueFree();
		}
		CurrentScene = node;
	}
}