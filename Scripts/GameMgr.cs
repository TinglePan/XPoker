using System.Collections.Generic;
using Godot;

namespace XCardGame.Scripts;

public partial class GameMgr : Node
{
	[Export] public PackedScene MainScene;
	
	public Player MainPlayer;
	public Node CurrentScene;
	public Match CurrentMatch;

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
	}

	public void StartMatch()
	{
		CurrentMatch = new Match();
		CurrentMatch.Setup(new Dictionary<string, object>()
		{
			{ "players", new List<Player> { MainPlayer } }
		});
	}

	public void ChangeScene(PackedScene scene)
	{
		var node = scene.Instantiate<Node>();
		var root = GetTree().Root;
		root.AddChild(node);
		if (CurrentScene != null)
		{
			root.RemoveChild(CurrentScene);
			CurrentScene.QueueFree();
		}
		CurrentScene = node;
	}
	
}