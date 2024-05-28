using Godot;

namespace XCardGame.Scripts.Nodes;

public partial class BaseManagedNode2D: Node2D, IManagedNode
{
	[Export]
	public string Identifier { get; set; }
	public GameMgr GameMgr { get; private set; }
	public SceneMgr SceneMgr { get; private set;  }
	
	
	public override void _Ready()
	{
		GameMgr = GetNode<GameMgr>("/root/GameMgr");
		SceneMgr = GetNode<SceneMgr>("/root/SceneMgr");
		SceneMgr.Register(this);
	}
}
