﻿using System.Collections.Generic;
using Godot;
using XCardGame.Scripts.Cards.PokerCards;
using XCardGame.Scripts.InputHandling;

namespace XCardGame.Scripts;

public partial class InputMgr: Node
{
    public List<BaseInputHandler> InputHandlerStack;
    public BaseInputHandler CurrentInputHandler;
    
    public override void _Ready()
    {
        var gameMgr = GetNode<GameMgr>("/root/GameMgr");
        InputHandlerStack = new List<BaseInputHandler>();
        CurrentInputHandler = new BaseInputHandler(gameMgr);
    }
    
    public override void _Input(InputEvent @event)
    {
        CurrentInputHandler.HandleInputEvent(@event);
    }
    
    public void SwitchToInputHandler(BaseInputHandler inputHandler)
    {
        GD.Print($"Switching to new input handler {inputHandler}");
        if (CurrentInputHandler != null)
        {
            InputHandlerStack.Add(CurrentInputHandler);
        }
        CurrentInputHandler = inputHandler;
    }
    
    public void QuitCurrentInputHandler()
    {
        GD.Print($"Quit input handler {CurrentInputHandler}");
        if (InputHandlerStack.Count > 0)
        {
            CurrentInputHandler = InputHandlerStack[^1];
            InputHandlerStack.RemoveAt(InputHandlerStack.Count - 1);
        }
    }
    
}