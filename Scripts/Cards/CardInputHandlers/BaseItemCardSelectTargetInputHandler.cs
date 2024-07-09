using System.Collections.Generic;
using Godot;
using XCardGame.Scripts.Common.Constants;
using XCardGame.Scripts.InputHandling;
using Battle = XCardGame.Scripts.Game.Battle;
using CardNode = XCardGame.Scripts.Ui.CardNode;

namespace XCardGame.Scripts.Cards.CardInputHandlers;

public abstract class BaseItemCardSelectTargetInputHandler<TOriginateCard>: BaseSelectTargetInputHandler<CardNode, BaseCard> where TOriginateCard: BaseCard
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
    
    public override void OnEnter()
    {
        base.OnEnter();
        Battle = GameMgr.CurrentBattle;
        ProceedButton = GameMgr.CurrentBattle.BigButton;
        
        if (ProceedButton is Button button)
        {
            button.Text = "Confirm";
        }
        ProceedButton.Pressed += Confirm;

        OriginateCardNode.OnPressed += OnOriginateCardPressed;
    }

    public override void OnExit()
    {
        base.OnExit();
        OriginateCardNode.TweenSelect(false, Configuration.SelectTweenTime);
        ProceedButton.Pressed -= Confirm;
        OriginateCardNode.OnPressed -= OnOriginateCardPressed;
    }

    protected override void SelectNode(CardNode node)
    {
        if (SelectedNodes.Count >= SelectTargetCountLimit)
        {
            UnSelectNode(SelectedNodes[0]);
        }
        base.SelectNode(node);
    }

    protected virtual async void Confirm()
    {
        Exit();
    }

    protected async void OnOriginateCardPressed(Ui.BaseContentNode<BaseCard> node)
    {
        Exit();
    }
}