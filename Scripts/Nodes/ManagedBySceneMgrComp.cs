using Godot;

namespace XCardGame.Scripts.Nodes;

public partial class ManagedBySceneMgrComp: Node
{
	[Export]
	public string Identifier { get; set; }
	public SceneMgr SceneMgr { get; private set;  }

	protected Node Parent;
	
	public override void _Ready()
	{
		SceneMgr = GetNode<SceneMgr>("/root/SceneMgr");
		Parent = GetParent<Node>();
		SceneMgr.Register(this, Parent);
	}
}
