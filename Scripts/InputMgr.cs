using System.Collections.Generic;
using System.Threading.Tasks;
using Godot;

namespace XCardGame;

public partial class InputMgr: Node
{
    private GameMgr _gameMgr;
    
    public List<BaseInputHandler> InputHandlerStack;
    public BaseInputHandler CurrentInputHandler;
    
    public override void _Ready()
    {
        _gameMgr = GetNode<GameMgr>("/root/GameMgr");
        InputHandlerStack = new List<BaseInputHandler>();
        CurrentInputHandler = new BaseInputHandler(_gameMgr);
    }
    
    public override void _Input(InputEvent @event)
    {
        CurrentInputHandler.HandleInputEvent(@event);
    }
    
    public async void SwitchToInputHandler(BaseInputHandler inputHandler)
    {
        GD.Print($"Switching to new input handler {inputHandler}");
        // var tasks = new List<Task>();
        if (CurrentInputHandler != null)
        {
            InputHandlerStack.Add(CurrentInputHandler);
            await CurrentInputHandler.OnExit();
        }
        CurrentInputHandler = inputHandler;
        await inputHandler.OnEnter();
        // await Task.WhenAll(tasks);
    }
    
    public async void QuitCurrentInputHandler()
    {
        GD.Print($"Quit input handler {CurrentInputHandler}");
        if (InputHandlerStack.Count > 0)
        {
            await CurrentInputHandler.OnExit();
            CurrentInputHandler = InputHandlerStack[^1];
            InputHandlerStack.RemoveAt(InputHandlerStack.Count - 1);
            await CurrentInputHandler.OnEnter();
        }
    }
    
}