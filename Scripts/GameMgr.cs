using System.Collections.Generic;
using Godot;
using XCardGame.Scripts.Common;
using XCardGame.Scripts.Ui;

namespace XCardGame.Scripts;

public partial class GameMgr : Node
{
	[Export] public PackedScene MainScene;
	[Export] public PackedScene PlayerPrefab;
	
	public PokerPlayer PlayerControlledPlayer;
	
	public Node CurrentScene;
	public Hand CurrentHand;
	public ActionUi ActionUi;

	private bool IsGameStarted;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		IsGameStarted = false;
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
		ActionUi = GetNode<ActionUi>("/root/Main/ActionUi");
		ActionUi.Hide();
		ActionUi.SetProcess(false);
		StartHand();
		var dbgButton = GetNode<Button>("/root/Main/Button");
		dbgButton.Pressed += () => CurrentHand.Start();
	}

	public void StartHand()
	{
		CurrentHand = new Hand();
		AddChild(CurrentHand);
		
		PlayerControlledPlayer = Utils.InstantiatePrefab(PlayerPrefab, CurrentHand) as PokerPlayer;
		PlayerControlledPlayer?.Setup(new Dictionary<string, object>()
		{
			{ "creature", new Creature("you", 100) },
			{ "hand", CurrentHand },
			{ "brainScriptPath", "res://Scripts/Brain/PlayerBrain.cs" }
		});
		var opponent = Utils.InstantiatePrefab(PlayerPrefab, CurrentHand) as PokerPlayer;
		opponent?.Setup(new Dictionary<string, object>()
		{
			{ "creature", new Creature("cpu", 100) },
			{ "hand", CurrentHand },
			{ "brainScriptPath", "res://Scripts/Brain/Ai/BaseAi.cs" }
		});
		
		CurrentHand.Setup(new Dictionary<string, object>()
		{
			{ "players", new List<PokerPlayer>
				{
					PlayerControlledPlayer,
					opponent
				} 
			}
		});
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

	public ActionUi OpenActionUi(Dictionary<string, object> context, ActionUi.ConfirmActionEventHandler onConfirmAction)
	{
		ActionUi.SetProcess(true);
		ActionUi.Show();
		ActionUi.Setup(context);
		ActionUi.ConfirmAction += onConfirmAction;
		return ActionUi;
	}
	
	public void CloseActionUi()
	{
		ActionUi.Hide();
		ActionUi.SetProcess(false);
	}

	public void RunMatch()
	{
		CurrentHand.Start();
	}
}