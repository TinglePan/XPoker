using Godot;

namespace XCardGame.Scripts;

public class UiMgr: Node
{
    private GameMgr _gameMgr;
    
    public override void _Ready()
    {
        _gameMgr = GetNode<GameMgr>("/root/GameMgr");
        
    }
}