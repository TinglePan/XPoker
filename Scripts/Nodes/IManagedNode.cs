namespace XCardGame.Scripts.Nodes;

public interface IManagedNode
{
    public string Identifier { get; }
    public GameMgr GameMgr { get; }
    public SceneMgr SceneMgr { get; }
    
}