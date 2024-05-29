using Godot;
using XCardGame.Scripts.GameLogic;

namespace XCardGame.Scripts.Nodes;

public partial class BattleScene: Node
{
    [Export] public Battle Battle;
    [Export] public Button ProceedButton;

    public override void _Ready()
    {
        Battle.ProceedButton = ProceedButton;
    }
}