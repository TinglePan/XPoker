using Godot;
using XCardGame.Scripts.InputHandling;

namespace XCardGame.Scripts.Ui;

public partial class BaseUi: Control, IHandleInput
{
    public virtual void OnClose()
    {
        QueueFree();
    }
    
}