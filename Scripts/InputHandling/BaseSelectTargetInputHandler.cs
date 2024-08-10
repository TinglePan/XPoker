using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot;
using XCardGame.Ui;

namespace XCardGame;

public abstract class BaseSelectTargetInputHandler<TTargetNode>: BaseInputHandler 
    where TTargetNode: BaseContentNode, ISelect
{
    public List<TTargetNode> SelectedNodes;
    public Func<IEnumerable<TTargetNode>> GetValidSelectTargetsFunc;
    
    protected List<TTargetNode> ValidSelectTargets;
    
    public BaseSelectTargetInputHandler(GameMgr gameMgr, Func<IEnumerable<TTargetNode>> getValidSelectTargetsFunc = null) : base(gameMgr)
    {
        SelectedNodes = new List<TTargetNode>();
        GetValidSelectTargetsFunc = getValidSelectTargetsFunc;
        ValidSelectTargets = new List<TTargetNode>();
    }
    
    public override async Task OnEnter()
    {
        await base.OnEnter();
        foreach (var selectTarget in GetValidSelectTargets())
        {
            selectTarget.OnMousePressed += OnTargetPressed;
            ValidSelectTargets.Add(selectTarget);
        }
    }

    public override async Task OnExit()
    {
        await base.OnExit();
        foreach (var selectTarget in ValidSelectTargets)
        {
            selectTarget.OnMousePressed -= OnTargetPressed;
        }
    }

    protected virtual IEnumerable<TTargetNode> GetValidSelectTargets()
    {
        if (GetValidSelectTargetsFunc != null)
        {
            return GetValidSelectTargetsFunc.Invoke();
        }
        return Enumerable.Empty<TTargetNode>();
    }

    protected virtual Task SelectNode(TTargetNode node)
    {
        node.IsSelected = true;
        node.OnSelected?.Invoke();
        SelectedNodes.Add(node);
        return Task.CompletedTask;
    }

    protected virtual Task UnSelectNode(TTargetNode node)
    {
        node.IsSelected = false;
        SelectedNodes.Remove(node);
        return Task.CompletedTask;
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