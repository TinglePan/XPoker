using System.Collections.Generic;
using System.Linq;
using Godot;
using XCardGame.Scripts.GameLogic;

namespace XCardGame.Scripts.Nodes;

public partial class BattleScene: Node
{
    [Export] public Battle Battle;
    [Export] public Button ProceedButton;
    [Export] public DialogueBox InspectorDialogueBox;
    [Export] public DialogueBox LogDialogueBox;

    public List<CardNode> WatchedCardNodes;
    
    public override void _Ready()
    {
        Battle.ProceedButton = ProceedButton;
        WatchedCardNodes = new List<CardNode>();
    }

    public override void _Process(double delta)
    {
        List<string> contents = new List<string>();
        foreach (var cardNode in WatchedCardNodes)
        {
            contents.Add($"{cardNode}({cardNode.Content}): {cardNode.Position}");
        }
        InspectorDialogueBox.Content.Value = string.Join("\n", contents);
    }
}