using System.Collections.Generic;
using Godot;
using XCardGame.Scripts.Cards;
using XCardGame.Scripts.GameLogic;
using XCardGame.Scripts.Nodes;

namespace XCardGame.Scripts.InputHandling;

public abstract class BaseInteractCardInputHandler<TCard>: BaseInputHandler where TCard: BaseCard, IInteractCard
{
    public Battle Battle;
    public TCard Card;
    public CardNode CardNode;
    public List<CardNode> SelectedCardNodes;
    public BaseButton ProceedButton;
    
    public BaseInteractCardInputHandler(GameMgr gameMgr, Battle battle, TCard card) : base(gameMgr)
    {
        Battle = battle;
        Card = card;
        SelectedCardNodes = new List<CardNode>();
        ProceedButton = GameMgr.CurrentBattle.ProceedButton;
    }
    
    public override void OnEnter()
    {
        base.OnEnter();
        ProceedButton.Pressed += Confirm;
        foreach (var selectTarget in GetValidSelectTargets())
        {
            selectTarget.OnPressed += ClickCard;
            selectTarget.IsSelected = false;
        }
        CardNode.OnPressed += ClickSelf;
    }

    public override void OnExit()
    {
        base.OnExit();
        ProceedButton.Pressed -= Confirm;
        foreach (var selectTarget in GetValidSelectTargets())
        {
            selectTarget.OnPressed -= ClickCard;
        }
        CardNode.OnPressed -= ClickSelf;
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