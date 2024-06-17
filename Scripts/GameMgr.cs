using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Godot;
using XCardGame.Scripts.Cards;
using XCardGame.Scripts.Cards.AbilityCards;
using XCardGame.Scripts.Common;
using XCardGame.Scripts.Common.Constants;
using XCardGame.Scripts.Common.DataBinding;
using XCardGame.Scripts.Defs;
using XCardGame.Scripts.GameLogic;
using XCardGame.Scripts.InputHandling;
using XCardGame.Scripts.Nodes;

namespace XCardGame.Scripts;

public partial class GameMgr : Node
{
	public PackedScene IntroScene;
	public PackedScene BattleScene;
	
	public InputMgr InputMgr;
	
	public Node CurrentScene;
	public Battle CurrentBattle;

	public ObservableProperty<int> ProgressCounter;

	public Random Rand;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		base._Ready();
		IntroScene = ResourceCache.Instance.Load<PackedScene>("res://Scenes/Intro.tscn");
		BattleScene = ResourceCache.Instance.Load<PackedScene>("res://Scenes/BattleScene.tscn");
		InputMgr = GetNode<InputMgr>("/root/InputMgr");
		ProgressCounter = new ObservableProperty<int>(nameof(ProgressCounter), this, 0);
		Rand = new Random();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void StartBattle()
	{
		CurrentBattle = ((BattleScene)CurrentScene).Battle;
		CurrentBattle.Setup(new Dictionary<string, object>
		{
			{ "dealCommunityCardCount", 5 },
			{ "faceDownCommunityCardCount", 1 },
			{ "requiredHoleCardCountMin", 0 },
			{ "requiredHoleCardCountMax", 2 },
			{
				"entities", new List<Dictionary<string, object>>
				{
					new()
					{
						{ "name", "you" },
						{ "portraitPath", "res://Sprites/duster_guy.png" },
						{ "deck", Decks.PlayerInitialDeck },
						{ "dealCardCount", 2 },
						{ "factionId", Enums.FactionId.Player },
						{ "handPowers", HandPowerTables.DefaultPlayerHandPowerTable },
						{ "baseHandPower", 0 },
						{ "maxHp", 10 },
						{ "maxCost", 3 },
						{ "level", 1 },
						{ "levelUpTable", LevelUpTables.DefaultPlayerLevelUpTable },
						{ "isHoleCardDealtVisible", true },
						{
							"abilityCards", new ObservableCollection<BaseCard>
							{
								new D6Card(Defs.Cards.D6),
								new MagicalHatCard(Defs.Cards.MagicalHat)
							}
						},
						{
							"skillCards", new ObservableCollection<BaseCard>()
							{
							}
						}
					},
					new()
					{
						{ "name", "cpu" },
						{ "portraitPath", "res://Sprites/cloak_guy.png" },
						{ "deck", Decks.EnemyInitialDeck },
						{ "dealCardCount", 2 },
						{ "factionId", Enums.FactionId.Enemy },
						{ "handPowers", HandPowerTables.DefaultEnemyHandPowerTable },
						{ "baseHandPower", 0 },
						{ "maxHp", 10 },
						{ "level", 1 },
						{ "isHoleCardDealtVisible", false },
						{
							"abilityCards", new ObservableCollection<BaseCard>
							{
							}
						},
						{
							"skillCards", new ObservableCollection<BaseCard>()
							{
							}
						}
					}
				}
			}
		});
		
		InputMgr.SwitchToInputHandler(new BattleMainInputHandler(this, CurrentBattle));
		CurrentBattle.Start();
	}

	public Node ChangeScene(PackedScene scene)
	{
		GD.Print("Changing scene...");
		var root = GetTree().Root;
		if (CurrentScene != null)
		{
			GD.Print($"remove {CurrentScene}");
			root.RemoveChild(CurrentScene);
			CurrentScene.QueueFree();
		}
		var node = Utils.InstantiatePrefab(scene, root);
		CurrentScene = node;
		return CurrentScene;
	}

	public void Quit()
	{
		GetTree().Quit();
	}

	public void GameEnd()
	{
		
	}
}