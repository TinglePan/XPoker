using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Godot;
using XCardGame.CardProperties;
using XCardGame.Common;
using XCardGame.Common.HelperBoilerPlates;
using XCardGame.Ui;
using CardNode = XCardGame.Ui.CardNode;

namespace XCardGame;

public class BaseCardSelectTargetInputHandler : BaseSelectTargetInputHandler<CardNode>
{
    public CardInputHandlerHelper Helper;

    public int SelectTargetCountLimit;

    public BaseCardSelectTargetInputHandler(GameMgr gameMgr, CardNode originate, int selectTargetCountLimit = -1) :
        base(gameMgr)
    {
        Helper = new CardInputHandlerHelper(this, originate);
        Helper.ReBindHandler("Confirm", Confirm);
        SelectTargetCountLimit = selectTargetCountLimit;
    }

    public override async Task AwaitAndDisableInput(Task task)
    {
        await Helper.AwaitAndDisableInput(task);
    }

    public override void OnEnter()
    {
        base.OnEnter();
        Helper.OnEnter(Configuration.StandardUsableCardOptionsMenuName);
    }

    public override async void OnExit()
    {
        base.OnExit();
        var tasks = new List<Task>();
        tasks.Add(Helper.OriginateCardNode.AnimateSelect(false, Configuration.SelectTweenTime));
        foreach (var cardNode in SelectedNodes)
        {
            tasks.Add(cardNode.AnimateSelect(false, Configuration.SelectTweenTime));
        }

        SelectedNodes.Clear();
        Helper.OnExit();
        await Task.WhenAll(tasks);
    }
    
    protected virtual async void Confirm()
    {
        if (ReceiveInput)
        {
            await AwaitAndDisableInput(Helper.OriginateCard.GetProp<BaseCardPropUsable>().Effect(SelectedNodes));
            Exit();
        }
    }

    protected override async void SelectNode(CardNode node)
    {
        if (SelectTargetCountLimit == 0) return;
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
}

public class BaseCardSelectTargetInputHandlerWithConfirmConstraints : BaseCardSelectTargetInputHandler
{
    public bool AllowNoTarget;
    public bool MustFullTargets;
    public Func<int, bool> CustomFilter;
    
    public BaseCardSelectTargetInputHandlerWithConfirmConstraints(GameMgr gameMgr, CardNode originate, int selectTargetCountLimit = -1,
        bool allowNoTarget = false, bool mustFullTargets = false, Func<int, bool> customFilter = null) : base(gameMgr, originate, selectTargetCountLimit)
    {
        AllowNoTarget = allowNoTarget;
        MustFullTargets = mustFullTargets;
        CustomFilter = customFilter;
        Helper.ReBindHandler("Confirm", Confirm);
    }
    
    protected override async void Confirm()
    {
        if (ReceiveInput && ConstraintFulfilled())
        {
            await AwaitAndDisableInput(Helper.OriginateCard.GetProp<BaseCardPropUsable>().Effect(SelectedNodes));
            Exit();
        }
    }
    
    protected bool ConstraintFulfilled()
    {
        if (CustomFilter != null)
        {
            return CustomFilter(SelectedNodes.Count);
        }
        if (!AllowNoTarget && SelectedNodes.Count == 0) return false;
        if (MustFullTargets && SelectedNodes.Count < SelectTargetCountLimit) return false;
        return true;
    }
    
}