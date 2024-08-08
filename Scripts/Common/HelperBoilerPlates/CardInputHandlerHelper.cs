using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Godot;
using XCardGame.CardProperties;
using XCardGame.Ui;

namespace XCardGame.Common.HelperBoilerPlates;

public class CardInputHandlerHelper
{
    public static readonly List<(string, string, Action)> StandardUsableCardOptionsMenuSetupArgs = new ()
    {
        ("Cancel", Utils._("Cancel"), null),
        ("Confirm", Utils._("Confirm"), null)
    };
    
    public PackedScene MenuPrefab;

    public BaseInputHandler InputHandler;
    public Battle Battle;
    public CardNode OriginateCardNode;
    public BaseCard OriginateCard;
    public HMenuButtons Menu;
    public Dictionary<string, Action> HandlerBindings;
    
    public CardInputHandlerHelper(BaseInputHandler inputHandler, CardNode originateCardNode)
    {
        MenuPrefab = ResourceCache.Instance.Load<PackedScene>("res://Scenes/HMenuButtons.tscn");
        InputHandler = inputHandler;
        OriginateCardNode = originateCardNode;
        OriginateCard = (BaseCard)originateCardNode.Content.Value;
        HandlerBindings = new Dictionary<string, Action>();
        Battle = originateCardNode.Battle;
    }
    
    public async Task AwaitAndDisableInput(Task task)
    {
        InputHandler.ReceiveInput = false;
        var disabledButtons = new List<Button>();
        foreach (var button in Menu.Buttons.Values)
        {
            button.Disabled = true;
            disabledButtons.Add(button);
        }
        await task;
        foreach (var button in disabledButtons)
        {
            button.Disabled = false;
        }
        InputHandler.ReceiveInput = true;
    } 

    public void OnEnter(string menuName, List<(string, string, Action)> buttonsSetupArgs = null)
    {
        SetupButtons(menuName, buttonsSetupArgs);
        OriginateCardNode.OnMousePressed += OnOriginateCardPressed;
    }

    public void OnExit()
    {
        foreach (var child in Battle.ButtonRoot.GetChildren())
        {
            if (child is Control control)
            {
                control.Hide();
            }
        }
        
        foreach (var (buttonName, func) in HandlerBindings)
        {
            Menu.Buttons[buttonName].Pressed -= func;
        }
        
        OriginateCardNode.OnMousePressed -= OnOriginateCardPressed;
    }

    public void SetupButtons(string menuName, List<(string, string, Action)> buttonsSetupArgs)
    {
        var useDefaultSetupArgs = false;
        if (buttonsSetupArgs == null)
        {
            buttonsSetupArgs = StandardUsableCardOptionsMenuSetupArgs;
            useDefaultSetupArgs = true;
        }
        buttonsSetupArgs ??= StandardUsableCardOptionsMenuSetupArgs;
        foreach (var child in OriginateCard.Battle.ButtonRoot.GetChildren())
        {
            if (child is Control control)
            {
                control.Hide();
            }
        }

        if (!Battle.ButtonRoot.HasNode(menuName))
        {
            Menu = (HMenuButtons)Utils.InstantiatePrefab(MenuPrefab, Battle.ButtonRoot);
            
            Menu.Setup(new HMenuButtons.SetupArgs
            {
                Name = menuName,
                ButtonSetupArgs = buttonsSetupArgs,
            });
            foreach (var (buttonName, _, func) in buttonsSetupArgs)
            {
                HandlerBindings[buttonName] = func;
            }
        }
        else
        {
            Menu = Battle.ButtonRoot.GetNode<HMenuButtons>(menuName);
            foreach (var (buttonName, _, func) in buttonsSetupArgs)
            {
                ReBindHandler(buttonName, func);
            }
        }

        if (useDefaultSetupArgs)
        {
            Menu.Buttons["Cancel"].Pressed += Cancel;
            HandlerBindings["Cancel"] = Cancel;
        }
        Menu.Show();
    }

    public void ReBindHandler(string buttonName, Action handler)
    {
        if (HandlerBindings.TryGetValue(buttonName, out var currFunc))
        {
            if (currFunc != null)
            {
                Menu.Buttons[buttonName].Pressed -= currFunc;
            }
        }
        if (handler != null)
        {
            Menu.Buttons[buttonName].Pressed += handler;
        }
        HandlerBindings[buttonName] = handler;
    }

    protected void Cancel()
    {
        if (InputHandler.ReceiveInput)
        {
            InputHandler.Exit();
        }
    }
    
    protected void OnOriginateCardPressed(BaseContentNode node, MouseButton mouseButton)
    {
        if (InputHandler.ReceiveInput)
        {
            InputHandler.Exit();
        }
    }
    
}