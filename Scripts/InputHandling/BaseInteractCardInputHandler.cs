using System.Collections.Generic;
using Godot;
using XCardGame.Scripts.Cards;
using XCardGame.Scripts.Nodes;

namespace XCardGame.Scripts.InputHandling;

public abstract class BaseInteractCardInputHandler<TCard>: BaseInputHandler where TCard: BaseInteractCard
{
    public TCard Card;
    public List<CardNode> SelectedCardNodes;
    
    public BaseInteractCardInputHandler(TCard card) : base(card.GameMgr)
    {
        Card = card;
        SelectedCardNodes = new List<CardNode>();
    }
    
    public override void OnEnter()
    {
        base.OnEnter();
        Card.Battle.ProceedButton.Pressed += Confirm;
        foreach (var selectTarget in GetValidSelectTargets())
        {
            selectTarget.OnPressed += ClickCard;
            selectTarget.IsSelected = false;
        }
        Card.Node.OnPressed += ClickSelf;
        // GD.Print("Enter NetherSwapCardInputHandler");
    }

    public override void OnExit()
    {
        base.OnExit();
        Card.Battle.ProceedButton.Pressed -= Confirm;
        foreach (var selectTarget in GetValidSelectTargets())
        {
            selectTarget.OnPressed -= ClickCard;
        }
        Card.Node.OnPressed -= ClickSelf;
    }

    protected abstract IEnumerable<CardNode> GetValidSelectTargets();
    
    protected void ClickSelf(CardNode node)
    {
        GameMgr.InputMgr.QuitCurrentInputHandler();
    }
    
    protected void ClickCard(CardNode node)
    {
        if (node.IsSelected)
        {
            node.IsSelected = false;
            if (SelectedCardNodes.Contains(node))
                SelectedCardNodes.Remove(node);
        }
        else
        {
            node.IsSelected = true;
            SelectedCardNodes.Add(node);
        }
    }
    
    protected override void OnRightMouseButtonPressed(Vector2 position)
    {
        GameMgr.InputMgr.QuitCurrentInputHandler();
    }
    
    protected override void OnActionPressed(InputEventAction action)
    {
        if (action.Action == "ui_escape")
        {
            GameMgr.InputMgr.QuitCurrentInputHandler();
        }
    }

    protected virtual void Confirm()
    {
        GameMgr.InputMgr.QuitCurrentInputHandler();
    }
}