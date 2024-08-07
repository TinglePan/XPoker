﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Godot;
using XCardGame.Ui;

namespace XCardGame;

public abstract class BaseSelectTargetInputHandler<TTargetNode>: BaseInputHandler 
    where TTargetNode: BaseContentNode, ISelect
{
    public List<TTargetNode> SelectedNodes;
    
    public BaseSelectTargetInputHandler(GameMgr gameMgr) : base(gameMgr)
    {
        SelectedNodes = new List<TTargetNode>();
    }
    
    public override async Task OnEnter()
    {
        await base.OnEnter();
        foreach (var selectTarget in GetValidSelectTargets())
        {
            selectTarget.OnMousePressed += OnTargetPressed;
        }
    }

    public override async Task OnExit()
    {
        await base.OnExit();
        foreach (var selectTarget in GetValidSelectTargets())
        {
            selectTarget.OnMousePressed -= OnTargetPressed;
        }
    }

    protected virtual IEnumerable<TTargetNode> GetValidSelectTargets()
    {
        yield break;
    }

    protected virtual void SelectNode(TTargetNode node)
    {
        node.IsSelected = true;
        node.OnSelected?.Invoke();
        SelectedNodes.Add(node);
    }

    protected virtual void UnSelectNode(TTargetNode node)
    {
        node.IsSelected = false;
        SelectedNodes.Remove(node);
    }
    
    protected void OnTargetPressed(BaseContentNode node, MouseButton mouseButton)
    {
        if (ReceiveInput)
        {
            if (mouseButton == MouseButton.Left)
            {
                var contentNode = (TTargetNode)node;
                if (contentNode.IsSelected)
                {
                    UnSelectNode(contentNode);
                }
                else
                {
                    SelectNode(contentNode);
                }
            }
        }
    }
    
    protected override void OnActionPressed(InputEventAction action)
    {
        base.OnActionPressed(action);
        if (action.Action == "ui_escape")
        {
            Exit();
        }
    }

    protected override void OnRightMouseButtonPressed(Vector2 position)
    {
        base.OnRightMouseButtonPressed(position);
        if (SelectedNodes.Count > 0)
        {
            UnSelectNode(SelectedNodes[^1]);
        }
        else
        {
            Exit();
        }
    }
}