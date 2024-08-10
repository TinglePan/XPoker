using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Threading.Tasks;
using Godot;
using XCardGame.CardProperties;
using XCardGame.Common;
using XCardGame.Ui;

namespace XCardGame;

public class BattleMainInputHandler: BaseInputHandler
{
    public BattleScene BattleScene;
    public Battle Battle;
    public PackedScene MenuPrefab;
    public List<CardContainer> UsableCardContainers;
    public Dictionary<Battle.State, HMenuButtons> Menus;
    
    public BattleMainInputHandler(GameMgr gameMgr) : base(gameMgr)
    {
        MenuPrefab = ResourceCache.Instance.Load<PackedScene>("res://Scenes/HMenuButtons.tscn");
        Menus = new Dictionary<Battle.State, HMenuButtons>();
    }

    public override async Task AwaitAndDisableInput(Task task)
    {
        // GD.Print("await and disable input 1");
        ReceiveInput = false;
        var disabledButtons = new List<Button>();
        var currentButtons = Menus[Battle.CurrentState.Value].Buttons;
        foreach (var button in currentButtons.Values)
        {
            button.Disabled = true;
            disabledButtons.Add(button);
        }
        await task;
        foreach (var button in disabledButtons)
        {
            button.Disabled = false;
        }
        ReceiveInput = true;
        // GD.Print("await and disable input 2");
    }

    public override async Task OnEnter()
    {
        await base.OnEnter();
        BattleScene = (BattleScene)GameMgr.CurrentScene;
        Battle = GameMgr.CurrentBattle;

        Battle.CurrentState.DetailedValueChanged += OnBattleStateChanged;
        SetupButtons();

        UsableCardContainers = new List<CardContainer>()
        {
            Battle.ItemCardContainer,
            Battle.RuleCardContainer
        };

        foreach (var cardContainer in UsableCardContainers)
        {
            cardContainer.ContentNodes.CollectionChanged += OnUsableCardNodesCollectionChanged;
            foreach (var cardNode in cardContainer.ContentNodes)
            {
                cardNode.OnMousePressed += OnCardNodePressed;
            }
        }
        foreach (var cardContainer in Battle.OpenedPiledCardContainer.CardContainers)
        {
            cardContainer.ContentNodes.CollectionChanged += OnUsableCardNodesCollectionChanged;
            foreach (var cardNode in cardContainer.ContentNodes)
            {
                cardNode.OnMousePressed += OnCardNodePressed;
            }
        }

        foreach (var cardContainer in Battle.EngageCardContainer.CardContainers)
        {
            cardContainer.ContentNodes.CollectionChanged += OnUsableCardNodesCollectionChanged;
            foreach (var cardNode in cardContainer.ContentNodes)
            {
                cardNode.OnMousePressed += OnCardNodePressed;
            }
        }
    }
    
    public override async Task OnExit()
    {
        await base.OnExit();
        foreach (var child in Battle.ButtonRoot.GetChildren())
        {
            if (child is Control control)
            {
                control.Hide();
            }
        }
        Battle.CurrentState.DetailedValueChanged -= OnBattleStateChanged;
        foreach (var cardContainer in UsableCardContainers)
        {
            cardContainer.ContentNodes.CollectionChanged -= OnUsableCardNodesCollectionChanged;
            foreach (var cardNode in cardContainer.ContentNodes)
            {
                cardNode.OnMousePressed -= OnCardNodePressed;
            }
        }
        foreach (var cardContainer in Battle.OpenedPiledCardContainer.CardContainers)
        {
            cardContainer.ContentNodes.CollectionChanged -= OnUsableCardNodesCollectionChanged;
            foreach (var cardNode in cardContainer.ContentNodes)
            {
                cardNode.OnMousePressed -= OnCardNodePressed;
            }
        }

        foreach (var cardContainer in Battle.EngageCardContainer.CardContainers)
        {
            cardContainer.ContentNodes.CollectionChanged -= OnUsableCardNodesCollectionChanged;
            foreach (var cardNode in cardContainer.ContentNodes)
            {
                cardNode.OnMousePressed -= OnCardNodePressed;
            }
        }
    }

    protected void SetupButtons()
    {
        foreach (var child in Battle.ButtonRoot.GetChildren())
        {
            if (child is Control control)
            {
                control.Hide();
            }
        }

        HMenuButtons menu;
        string menuName;
        
        menuName = Battle.State.BeforeDealCards.ToString();
        if (!Battle.ButtonRoot.HasNode(menuName))
        {
            
            menu = (HMenuButtons)Utils.InstantiatePrefab(MenuPrefab, Battle.ButtonRoot);
            menu.Setup(new HMenuButtons.SetupArgs
            {
                Name = menuName,
                ButtonSetupArgs = new List<(string, string, Action)>
                {
                    ("DealCards", Utils._("Deal cards"), OnDealCardsButtonPressed),
                }
            });
            menu.Hide();
            Menus[Battle.State.BeforeDealCards] = menu;
        }
        
        menuName = Battle.State.BeforeShowDown.ToString();
        if (!Battle.ButtonRoot.HasNode(menuName))
        {
            menu = (HMenuButtons)Utils.InstantiatePrefab(MenuPrefab, Battle.ButtonRoot);
            menu.Setup(new HMenuButtons.SetupArgs
            {
                Name = menuName,
                ButtonSetupArgs = new List<(string, string, Action)>
                {
                    ("Flip", Utils._("Flip"), OnFlipButtonPressed),
                    ("ShowDown", Utils._("Show down"), OnShowDownButtonPressed),
                    ("Fold", Utils._("Fold"), OnFoldButtonPressed),
                }
            });
            menu.Hide();
            Menus[Battle.State.BeforeShowDown] = menu;
        }

        menuName = Battle.State.BeforeEngage.ToString();
        if (!Battle.ButtonRoot.HasNode(menuName))
        {
            menu = (HMenuButtons)Utils.InstantiatePrefab(MenuPrefab, Battle.ButtonRoot);
            menu.Setup(new HMenuButtons.SetupArgs
            {
                Name = menuName,
                ButtonSetupArgs = new List<(string, string, Action)>
                {
                    ("Engage", Utils._("Engage!"), OnEngageButtonPressed),
                }
            });
            menu.Hide();
            Menus[Battle.State.BeforeEngage] = menu;
        }

        menuName = Battle.State.AfterEngage.ToString();
        if (!Battle.ButtonRoot.HasNode(menuName))
        {
            menu = (HMenuButtons)Utils.InstantiatePrefab(MenuPrefab, Battle.ButtonRoot);
            menu.Setup(new HMenuButtons.SetupArgs
            {
                Name = menuName,
                ButtonSetupArgs = new List<(string, string, Action)>
                {
                    ("NextRound", Utils._("Next round"), OnNextRoundButtonPressed),
                }
            });
            menu.Hide();
            Menus[Battle.State.AfterEngage] = menu;
        }
        Battle.CurrentState.FireValueChangeEventsOnInit();
    }
    
    protected async void OnCardNodePressed(BaseContentNode node, MouseButton mouseButton)
    {
        GD.Print($"on card node {node} pressed: {ReceiveInput}.");
        if (ReceiveInput)
        {
            if (mouseButton == MouseButton.Left)
            {
                if (node is PiledCardNode)
                {
                    var card = (BaseCard)node.Content.Value;
                    var piledProp = card.GetProp<CardPropPiled>();
                    if (piledProp.IsOpened)
                    {
                        await GameMgr.AwaitAndDisableInput(piledProp.Close());
                    } else
                    {
                        await GameMgr.AwaitAndDisableInput(piledProp.Open());
                    }
                } else if (node is CardNode)
                {
                    if (node.Content.Value is BaseCard card)
                    {
                        foreach (var prop in card.GetProps<BaseCardPropUsable>())
                        {
                            if (prop.CanUse())
                            {
                                prop.Use();
                            }
                        }
                    }
                }
            }
        }
    }
    
    protected void OnUsableCardNodesCollectionChanged(object sender, NotifyCollectionChangedEventArgs args)
    {
        switch (args.Action)
        {
            case NotifyCollectionChangedAction.Add:
                if (args.NewItems != null)
                    foreach (var t in args.NewItems)
                    {
                        if (t is CardNode cardNode)
                        {
                            cardNode.OnMousePressed += OnCardNodePressed;
                        }
                    }
                break;
            case NotifyCollectionChangedAction.Remove:
                if (args.OldItems != null)
                    foreach (var t in args.OldItems)
                    {
                        if (t is CardNode cardNode)
                        {
                            cardNode.OnMousePressed -= OnCardNodePressed;
                        }
                    }
                break;
        }
    }

    protected void OnBattleStateChanged(object sender, ValueChangedEventDetailedArgs<Battle.State> args)
    {
        Menus[args.OldValue].Hide();
        Menus[args.NewValue].Show();
    }

    protected async void OnDealCardsButtonPressed()
    {
        await GameMgr.AwaitAndDisableInput(Battle.DealCards());
        Battle.CurrentState.Value = Battle.State.BeforeShowDown;
        if (Battle.CanFlipCards())
        {
            Menus[Battle.State.BeforeShowDown].Buttons["Flip"].Show();
        }
    }

    protected async void OnFlipButtonPressed()
    {
        await GameMgr.AwaitAndDisableInput(Battle.FlipCards());
        if (!Battle.CanFlipCards())
        {
            Menus[Battle.State.BeforeShowDown].Buttons["Flip"].Hide();
        }
    }
    
    protected async void OnShowDownButtonPressed()
    {
        await GameMgr.AwaitAndDisableInput(Battle.ShowDown());
        Battle.CurrentState.Value = Battle.State.BeforeEngage;
    }
    
    protected async void OnFoldButtonPressed()
    {
        await GameMgr.AwaitAndDisableInput(Battle.Fold());
        Battle.CurrentState.Value = Battle.State.BeforeEngage;
    }
    
    protected async void OnEngageButtonPressed()
    {
        await GameMgr.AwaitAndDisableInput(Battle.Engage());
        Battle.CurrentState.Value = Battle.State.AfterEngage;
    }
    
    protected async void OnNextRoundButtonPressed()
    {
        await GameMgr.AwaitAndDisableInput(Battle.RoundEnd());
        Battle.CurrentState.Value = Battle.State.BeforeDealCards;
    }
}