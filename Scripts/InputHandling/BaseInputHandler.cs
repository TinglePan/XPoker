using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Godot;

namespace XCardGame.Scripts.InputHandling;

public class BaseInputHandler: IHandleInput
{
    protected GameMgr GameMgr;
    
    public Action<InputEventAction> OnInputAction;
    public Action<Vector2> OnLMouseButtonPressed;
    public Action<Vector2> OnRMouseButtonPressed;

    protected bool ReceiveInput;
    
    public BaseInputHandler(GameMgr gameMgr)
    {
        GameMgr = gameMgr;
        ReceiveInput = true;
    }
    
    public virtual async Task AwaitAndDisableInput(Task task)
    {
        ReceiveInput = false;
        await task;
        ReceiveInput = true;
    }
    
    public void HandleInputEvent(InputEvent @event)
    {
        if (ReceiveInput)
        {
            if (@event is InputEventAction { Pressed: true } action)
            {
                OnActionPressed(action);
            } else if (@event is InputEventMouseButton { Pressed: true } mouse)
            {
                if (mouse.ButtonIndex == MouseButton.Left)
                {
                    OnLeftMouseButtonPressed(mouse.Position);
                } else if (mouse.ButtonIndex == MouseButton.Right)
                {
                    OnRightMouseButtonPressed(mouse.Position);
                }
            }
        }
    }

    public void Exit()
    {
        GameMgr.InputMgr.QuitCurrentInputHandler();
    }

    public virtual void OnEnter()
    {
        
    }

    public virtual void OnExit()
    {
        
    }
    
    protected virtual void OnLeftMouseButtonPressed(Vector2 position)
    {
        OnLMouseButtonPressed?.Invoke(position);
    }
    
    protected virtual void OnRightMouseButtonPressed(Vector2 position)
    {
        OnRMouseButtonPressed?.Invoke(position);
    }
    
    protected virtual void OnActionPressed(InputEventAction action)
    {
        OnInputAction?.Invoke(action);
    }
}