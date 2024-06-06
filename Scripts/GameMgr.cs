using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Godot;
using XCardGame.Scripts.Cards;
using XCardGame.Scripts.Cards.AbilityCards;
using XCardGame.Scripts.Common;
using XCardGame.Scripts.Common.Constants;
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
	public SceneMgr SceneMgr;
	
	public Node CurrentScene;
	public Battle CurrentBattle;

	public Random Rand;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		base._Ready();
		IntroScene = ResourceCache.Instance.Load<PackedScene>("res://Scenes/Intro.tscn");
		BattleScene = ResourceCache.Instance.Load<PackedScene>("res://Scenes/BattleScene.tscn");
		InputMgr = GetNode<InputMgr>("/root/InputMgr");
		SceneMgr = GetNode<SceneMgr>("/root/SceneMgr");
		Rand = new Random();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void StartBattle()
	{
		ChangeScene(BattleScene);
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
						{ "maxHp", 20 },
						{ "maxCost", 3 },
						{ "level", 1 },
						{ "levelUpTable", LevelUpTables.DefaultPlayerLevelUpTable },
						{ "isHoleCardDealtVisible", true },
						{
							"abilityCards", new ObservableCollection<BaseCard>
							{
								new D6Card(Enums.CardSuit.Diamonds, Enums.CardRank.Six),
								new NetherSwapCard(Enums.CardSuit.Hearts, Enums.CardRank.Six)
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
						{ "maxHp", 20 },
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
		
		InputMgr.SwitchToInputHandler(new MainInputHandler(this));
		CurrentBattle.Start();
	}

	public Node ChangeScene(PackedScene scene)
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
		return CurrentScene;
	}
}