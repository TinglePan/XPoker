using System;
using System.Collections.Generic;
using System.Diagnostics;
using Godot;
using XCardGame.Scripts.Cards.AbilityCards;
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
		var dbgButton = GetNode<Button>("/root/Main/Button");
		dbgButton.Pressed += () => CurrentBattle.Start();
	}

	public void StartBattle()
	{
		void MockSetup()
		{
			var player = Utils.InstantiatePrefab(PlayerPrefab, CurrentBattle) as PlayerBattleEntity;
			Debug.Assert(player != null);
			player.Setup(new Dictionary<string, object>()
			{
				{ "name", "you" },
				{ "battle", CurrentBattle },
				{ "deck", Decks.PlayerInitialDeck }
			});
			player.AbilityCards.Add(new D6Card(this, Enums.CardFace.Up, player));
			player.AbilityCards.Add(new NetherSwapCard(this, Enums.CardFace.Up, player));
		
		
			var opponent = Utils.InstantiatePrefab(OpponentPrefab, CurrentBattle) as BattleEntity;
			Debug.Assert(opponent != null);
			opponent.Setup(new Dictionary<string, object>()
			{
				{ "name", "cpu"},
				{ "battle", CurrentBattle },
				{ "deck", Decks.OpponentInitialDeck },
				{ "factionId", Enums.FactionId.Opponent },
			
			});

			UiMgr.OpenCommunityCardContainer(CurrentBattle.CommunityCards);
			UiMgr.OpenBattleEntityUiCollection(player);
			UiMgr.OpenBattleEntityUiCollection(opponent);
			UiMgr.OpenAbilityCardUi(player.AbilityCards);
		
			InputMgr.SwitchToInputHandler(new MainInputHandler(this));
		
			CurrentBattle.Setup(new Dictionary<string, object>()
			{
				{ "entities", new List<BattleEntity>
					{
						player,
						opponent
					}
				},
				{ "player", player }
			});
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