using Godot;
using XCardGame.Scripts.Game;

namespace XCardGame.Scripts.Ui;

public partial class BattleScene: Node
{
    public Battle Battle;
    public Button ProceedButton;
    public DialogueBox InspectorDialogueBox;
    public DialogueBox LogDialogueBox;
    
    public override void _Ready()
    {
        Battle = GetNode<Battle>("BattleUi/SubViewportContainer/SubViewport/Battle");
        ProceedButton = GetNode<Button>("BattleUi/ProceedButton");
        InspectorDialogueBox = GetNode<DialogueBox>("InspectorBox");
        LogDialogueBox = GetNode<DialogueBox>("LogBox");
        Battle.BigButton = ProceedButton;
    }
}