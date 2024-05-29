using Godot;
using XCardGame.Scripts.Common;

namespace XCardGame.Scripts;

public partial class Intro: Node
{
    [Export] public BaseButton SetupButton;
    [Export] public BaseButton StartButton;
    public GameMgr GameMgr;
    
    public override void _Ready()
    {
        GameMgr = GetNode<GameMgr>("/root/GameMgr");
        SetupButton.Pressed += GameMgr.SetupBattle;
    }

}