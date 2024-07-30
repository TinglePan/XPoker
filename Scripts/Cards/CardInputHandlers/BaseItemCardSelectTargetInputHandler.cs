using System.Collections.Generic;
using System.Threading.Tasks;
using Godot;
using XCardGame.Common;
using XCardGame.Ui;
using CardNode = XCardGame.Ui.CardNode;

namespace XCardGame;

public abstract class BaseItemCardSelectTargetInputHandler<TOriginateCard>: BaseSelectTargetInputHandler<Ui.CardNode> where TOriginateCard: BaseCard
{

    public PackedScene MenuPrefab;
    
    public Battle Battle;
    public BaseButton ProceedButton;
    public CardNode OriginateCardNode;
    public TOriginateCard OriginateCard;
    public int SelectTargetCountLimit;
    public HMenuButtons Menu;
    
    public BaseItemCardSelectTargetInputHandler(GameMgr gameMgr, CardNode originate, int selectTargetCountLimit = 0) : base(gameMgr)
    {
        MenuPrefab = ResourceCache.Instance.Load<PackedScene>("res://Scenes/HMenuButtons.tscn");
        OriginateCardNode = originate;
        OriginateCard = (TOriginateCard)originate.Content.Value;
        SelectTargetCountLimit = selectTargetCountLimit;
    }

    public override async Task AwaitAndDisableInput(Task task)
    {
        ReceiveInput = false;
        var disabledButtons = new List<Button>();
        foreach (var button in Menu.Buttons.Values)
        {
            button.Disabled = true;
            disabledButtons.Add(button);
        }
        await task;
        ReceiveInput = true;
        foreach (var button in disabledButtons)
        {
            button.Disabled = false;
        }
    }

    public override async void OnEnter()
    {
        base.OnEnter();
        Battle = GameMgr.CurrentBattle;
        SetupButtons();
        await OriginateCardNode.AnimateSelect(true, Configuration.SelectTweenTime);
        if (ProceedButton is Button button)
        {
            button.Text = "Confirm";
        }
        ProceedButton.Pressed += Confirm;
        OriginateCardNode.OnMousePressed += OnOriginateCardPressed;
    }

    public override async void OnExit()
    {
        base.OnExit();
        foreach (var child in Battle.ButtonRoot.GetChildren())
        {
            if (child is Control control)
            {
                control.Hide();
            }
        }
        var tasks = new List<Task>();
        tasks.Add(OriginateCardNode.AnimateSelect(false, Configuration.SelectTweenTime));
        foreach (var cardNode in SelectedNodes)
        {
            tasks.Add(cardNode.AnimateSelect(false, Configuration.SelectTweenTime));
        }
        await Task.WhenAll(tasks);
        ProceedButton.Pressed -= Confirm;
        OriginateCardNode.OnMousePressed -= OnOriginateCardPressed;
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
        var menuName = "ItemCardSelectTargetMenu";
        
        if (!Battle.ButtonRoot.HasNode(menuName))
        {
            menu = (HMenuButtons)Utils.InstantiatePrefab(MenuPrefab, Battle.ButtonRoot);
            menu.Setup(new HMenuButtons.SetupArgs
            {
                Name = menuName,
                ButtonTagsAndLabels = new List<(string, string)>
                {
                    ("Cancel", Utils._("Cancel")),
                    ("Confirm", Utils._("Confirm"))
                }
            });
            menu.Buttons["Cancel"].Pressed += OnCancelButtonPressed;
            menu.Buttons["Confirm"].Pressed += OnConfirmButtonPressed;
        }
        else
        {
            menu = Battle.ButtonRoot.GetNode<HMenuButtons>(menuName);
        }
        menu.Show();
    }

    protected override async void SelectNode(CardNode node)
    {
        if (SelectTargetCountLimit > 0 && SelectedNodes.Count >= SelectTargetCountLimit)
        {
            UnSelectNode(SelectedNodes[0]);
        }
        SelectedNodes.Add(node);
        await node.AnimateSelect(true, Configuration.SelectTweenTime);
    }

    protected override async void UnSelectNode(CardNode node)
    {
        SelectedNodes.Remove(node);
        await node.AnimateSelect(false, Configuration.SelectTweenTime);
    }

    protected virtual void Confirm()
    {
        Exit();
    }

    protected virtual void Cancel()
    {
        Exit();
    }

    protected void OnOriginateCardPressed(BaseContentNode node, MouseButton mouseButton)
    {
        if (ReceiveInput)
        {
            Cancel();
        }
    }
    
    protected void OnConfirmButtonPressed()
    {
        if (ReceiveInput)
        {
            Confirm();
        }
    }
    
    protected void OnCancelButtonPressed()
    {
        if (ReceiveInput)
        {
            Cancel();
        }
    }
}