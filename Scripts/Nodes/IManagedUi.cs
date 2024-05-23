namespace XCardGame.Scripts.Nodes;

public interface IManagedUi
{
    public string Identifier { get; }
    public GameMgr GameMgr { get; }
    public UiMgr UiMgr { get; }
    
}