using System;
using System.Collections.Generic;
using Godot;
using XCardGame.Scripts.Cards;
using XCardGame.Scripts.GameLogic;
using XCardGame.Scripts.Nodes;
using XCardGame.Scripts.Ui;

namespace XCardGame.Scripts.InputHandling;

public abstract class BaseSelectTargetInputHandler<TContentNode, TContent>: BaseInputHandler 
    where TContentNode: BaseContentNode<TContent>, ISelect
    where TContent: IContent<TContent>
{
    public List<TContentNode> SelectedNodes;
    
    public BaseSelectTargetInputHandler(GameMgr gameMgr) : base(gameMgr)
    {
        SelectedNodes = new List<TContentNode>();
    }
    
    public override void OnEnter()
    {
        base.OnEnter();
        foreach (var selectTarget in GetValidSelectTargets())
        {
            selectTarget.OnPressed += OnTargetPressed;
            selectTarget.IsSelected = false;
        }
    }

    public override void OnExit()
    {
        base.OnExit();
        foreach (var selectTarget in GetValidSelectTargets())
        {
            selectTarget.OnPressed -= OnTargetPressed;
            selectTarget.IsSelected = false;
        }
    }

    protected abstract IEnumerable<TContentNode> GetValidSelectTargets();

    protected void SelectNode(TContentNode node)
    {
        node.IsSelected = true;
        node.OnSelected?.Invoke();
        SelectedNodes.Add(node);
    }

    protected void UnSelectNode(TContentNode node)
    {
        node.IsSelected = false;
        SelectedNodes.Remove(node);
    }
    
    protected void OnTargetPressed(BaseContentNode<TContent> node)
    {
        var contentNode = (TContentNode)node;
        if (contentNode.IsSelected)
        {
            UnSelectNode(contentNode);
        }
        else
        {
            SelectNode(contentNode);
        }
    }
    
    protected override void OnActionPressed(InputEventAction action)
    {
        if (action.Action == "ui_escape")
        {
            Exit();
        }
    }

    protected override void OnRightMouseButtonPressed(Vector2 position)
    {
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