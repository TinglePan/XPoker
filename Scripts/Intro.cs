using Godot;
using XCardGame.Scripts.Common;

namespace XCardGame.Scripts;

public partial class Intro: Node
{
    public GameMgr GameMgr;
    public BaseButton StartButton;
    
    public override void _Ready()
    {
        GameMgr = GetNode<GameMgr>("/root/GameMgr");
        StartButton = GetNode<BaseButton>("StartButton");
        StartButton.Pressed += GameMgr.StartBattle;
    }

}