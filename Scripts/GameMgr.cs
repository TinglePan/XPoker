using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using Godot;
using XCardGame.Scripts.Cards;
using XCardGame.Scripts.Common;
using XCardGame.Scripts.Common.Constants;
using XCardGame.Scripts.Common.DataBinding;
using XCardGame.Scripts.Defs;
using XCardGame.Scripts.Game;
using XCardGame.Scripts.InputHandling;
using XCardGame.Scripts.Ui;
using Battle = XCardGame.Scripts.Game.Battle;
using BattleEntity = XCardGame.Scripts.Game.BattleEntity;
using BattleScene = XCardGame.Scripts.Ui.BattleScene;
using PlayerBattleEntity = XCardGame.Scripts.Game.PlayerBattleEntity;

namespace XCardGame.Scripts;

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
		CurrentBattle.Setup(new Dictionary<string, object>
		{
			{ "dealCommunityCardCount", 5 },
			{ "faceDownCommunityCardCount", 1 },
			{ "requiredHoleCardCountMin", 0 },
			{ "requiredHoleCardCountMax", 2 },
			{
				"entities", new List<Dictionary<string, object>>
				{
					PlayerBattleEntity.InitArgs(BattleEntityDefs.DefaultPlayerBattleEntityDef),
					BattleEntity.InitArgs(BattleEntityDefs.DefaultEnemyBattleEntityDef)
				}
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

	public async Task AwaitAndDisableProceed(Task task)
	{
		if (CurrentScene is BattleScene battleScene)
		{
			battleScene.ProceedButton.Disabled = true;
			await task;
			battleScene.ProceedButton.Disabled = false;
		}
		else
		{
			await task;
		}
	}
}