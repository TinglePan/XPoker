using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Godot;
using XCardGame.Common;
using BattleScene = XCardGame.Ui.BattleScene;

namespace XCardGame;

public partial class GameMgr : Node
{
	public PackedScene IntroScenePrefab;
	public PackedScene BattleScenePrefab;
	
	public InputMgr InputMgr;
	
	public Node CurrentScene => SceneStack[^1];
	public Battle CurrentBattle;
	public BattleLog BattleLog;

	public ObservableProperty<int> ProgressCounter;

	public List<Node> SceneStack;
	
	public Random Rand;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		base._Ready();
		IntroScenePrefab = ResourceCache.Instance.Load<PackedScene>("res://Scenes/Intro.tscn");
		BattleScenePrefab = ResourceCache.Instance.Load<PackedScene>("res://Scenes/BattleScene.tscn");
		InputMgr = GetNode<InputMgr>("/root/InputMgr");
		ProgressCounter = new ObservableProperty<int>(nameof(ProgressCounter), this, 0);
		Rand = new Random();
		SceneStack = new List<Node> { GetTree().Root };
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void StartBattle()
	{
		ChangeScene(BattleScenePrefab);
		var battleScene = (BattleScene)CurrentScene;
		BattleLog = battleScene.GetNode<BattleLog>("LogBox");
		CurrentBattle = battleScene.Battle;
		var playerBattleEntityInitArgs = PlayerBattleEntity.InitArgs(BattleEntityDefs.PiratePlayerBattleEntityDef);
		playerBattleEntityInitArgs.Deck.MixIn(DeckDefs.Standard52Deck());
		
		CurrentBattle.Setup(new Battle.SetupArgs
		{
			DealCommunityCardCount = Configuration.CommunityCardCount,
			FaceDownCommunityCardCount = Configuration.DefaultFaceDownCommunityCardCount,
			RequiredHoleCardCountMin = 0,
			RequiredHoleCardCountMax = 2,
			EntitySetupArgs = new List<BattleEntity.SetupArgs>()
			{
				playerBattleEntityInitArgs,
				BattleEntity.InitArgs(BattleEntityDefs.DefaultEnemyBattleEntityDef)
			}
		});
		
		InputMgr.SwitchToInputHandler(new BattleMainInputHandler(this));
		CurrentBattle.Start();
	}

	public Node ChangeScene(PackedScene scene)
	{
		GD.Print("Changing scene...");
		Node parent;
		if (SceneStack.Count == 1)
		{
			parent = CurrentScene;
		}
		else
		{
			parent = SceneStack[^2];
			QuitCurrentScene();
		}
		var node = Utils.InstantiatePrefab(scene, parent);
		SceneStack.Add(node);
		return CurrentScene;
	}

	public Node OverlayScene(PackedScene scene)
	{
		var parent = CurrentScene;
		var node = Utils.InstantiatePrefab(scene, parent);
		SceneStack.Add(node);
		return CurrentScene;
	}

	public void QuitCurrentScene()
	{
		var parent = CurrentScene.GetParent();
		var current = CurrentScene;
		parent.RemoveChild(current);
		SceneStack.Remove(current);
		current.QueueFree();
	}

	public void Quit()
	{
		GetTree().Quit();
	}

	public void GameEnd()
	{
		
	}

	public async Task AwaitAndDisableInput(Task task)
	{
		await InputMgr.CurrentInputHandler.AwaitAndDisableInput(task);
	}
}