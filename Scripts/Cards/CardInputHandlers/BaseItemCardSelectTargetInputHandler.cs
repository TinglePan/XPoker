using System.Collections.Generic;
using System.Threading.Tasks;
using Godot;
using XCardGame.Common;
using XCardGame.Ui;
using CardNode = XCardGame.Ui.CardNode;

namespace XCardGame;

public abstract class BaseItemCardSelectTargetInputHandler<TOriginateCard>: BaseSelectTargetInputHandler<Ui.CardNode> where TOriginateCard: BaseCard
{
    public Battle Battle;
    public BaseButton ProceedButton;
    public CardNode OriginateCardNode;
    public TOriginateCard OriginateCard;
    public int SelectTargetCountLimit;
    
    public BaseItemCardSelectTargetInputHandler(GameMgr gameMgr, CardNode originate, int selectTargetCountLimit = 0) : base(gameMgr)
    {
        OriginateCardNode = originate;
        OriginateCard = (TOriginateCard)originate.Content.Value;
        SelectTargetCountLimit = selectTargetCountLimit;
    }

    public override async Task AwaitAndDisableInput(Task task)
    {
        ReceiveInput = false;
        ProceedButton.Disabled = true;
        await task;
        ReceiveInput = true;
        ProceedButton.Disabled = false;
    }

    public override async void OnEnter()
    {
        base.OnEnter();
        Battle = GameMgr.CurrentBattle;
        ProceedButton = GameMgr.CurrentBattle.BigButton;

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

    protected void OnOriginateCardPressed(BaseContentNode node, MouseButton mouseButton)
    {
        if (ReceiveInput)
        {
            Exit();
        }
    }
}