﻿using System;
using System.Collections.Generic;
using System.Linq;
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

    public BaseCardSelectTargetInputHandler(GameMgr gameMgr, CardNode originate, int selectTargetCountLimit = -1,
        Func<IEnumerable<CardNode>> getValidSelectTargetsFunc = null) :
        base(gameMgr, getValidSelectTargetsFunc)
    {
        Helper = new CardInputHandlerHelper(this, originate);
        SelectTargetCountLimit = selectTargetCountLimit;
    }

    public override async Task AwaitAndDisableInput(Task task)
    {
        await Helper.AwaitAndDisableInput(task);
    }

    public override async Task OnEnter()
    {
        var tasks = new List<Task>();
        Helper.OnEnter(Configuration.StandardUsableCardOptionsMenuName);
        Helper.ReBindHandler("Confirm", Confirm);
        tasks.Add(base.OnEnter());
        tasks.Add(Helper.OriginateCardNode.AnimateSelect(true, Configuration.SelectTweenTime));
        await Task.WhenAll(tasks);
    }

    public override async Task OnExit()
    {
        var tasks = new List<Task>();
        tasks.Add(Helper.OriginateCardNode.AnimateSelect(false, Configuration.SelectTweenTime));
        foreach (var cardNode in SelectedNodes.ToList())
        {
            if (GodotObject.IsInstanceValid(cardNode))
            {
                tasks.Add(UnSelectNode(cardNode));
            }
        }
        tasks.Add(base.OnExit());
        await Task.WhenAll(tasks);
        Helper.OnExit();
        GD.Print("BaseCardSelectTargetInputHandler On Exit");
    }
    
    protected virtual async void Confirm()
    {
        if (ReceiveInput)
        {
            await AwaitAndDisableInput(Helper.OriginateCard.GetProp<BaseCardPropUsable>().Effect(SelectedNodes));
            Exit();
        }
    }

    protected override async Task SelectNode(CardNode node)
    {
        if (SelectTargetCountLimit == 0) return;
        var tasks = new List<Task>();
        if (SelectTargetCountLimit > 0 && SelectedNodes.Count >= SelectTargetCountLimit)
        {
            tasks.Add(UnSelectNode(SelectedNodes[0]));
        }

        SelectedNodes.Add(node);
        GD.Print($"select node {node}");
        tasks.Add(node.AnimateSelect(true, Configuration.SelectTweenTime));
        await Task.WhenAll(tasks);
    }

    protected override async Task UnSelectNode(CardNode node)
    {
        SelectedNodes.Remove(node);
        GD.Print($"unselect node {node}");
        await node.AnimateSelect(false, Configuration.SelectTweenTime);
    }
}

public class BaseCardSelectTargetInputHandlerWithConfirmConstraints : BaseCardSelectTargetInputHandler
{
    public bool AllowNoTarget;
    public bool MustFullTargets;
    public Func<int, bool> CustomFilter;
    
    public BaseCardSelectTargetInputHandlerWithConfirmConstraints(GameMgr gameMgr, CardNode originate, int selectTargetCountLimit = -1,
        Func<IEnumerable<CardNode>> getValidSelectTargetsFunc = null, bool allowNoTarget = false,
        bool mustFullTargets = false, Func<int, bool> customFilter = null) : base(gameMgr, originate, selectTargetCountLimit, getValidSelectTargetsFunc)
    {
        AllowNoTarget = allowNoTarget;
        MustFullTargets = mustFullTargets;
        CustomFilter = customFilter;
    }

    public override async Task OnEnter()
    {
        await base.OnEnter();
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