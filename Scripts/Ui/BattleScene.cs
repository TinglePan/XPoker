using Godot;

namespace XCardGame.Ui;

public partial class BattleScene: Node
{
    public Battle Battle;
    public Control ButtonRoot;
    public DialogueBox InspectorDialogueBox;
    public DialogueBox LogDialogueBox;
    
    public override void _Ready()
    {
        Battle = GetNode<Battle>("BattleUi/SubViewportContainer/SubViewport/Battle");
        ButtonRoot = GetNode<Control>("BattleUi/Buttons");
        InspectorDialogueBox = GetNode<DialogueBox>("InspectorBox");
        LogDialogueBox = GetNode<DialogueBox>("LogBox");
    }
}