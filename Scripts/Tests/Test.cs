using Godot;

namespace XCardGame.Tests;



public partial class Test : Node
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		var gameMgr = GetNode<GameMgr>("/root/GameMgr");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}