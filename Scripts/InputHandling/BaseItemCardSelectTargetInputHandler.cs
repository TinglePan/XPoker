﻿using System.Collections.Generic;
using Godot;
using XCardGame.Scripts.Cards;


using Battle = XCardGame.Scripts.Game.Battle;
using CardNode = XCardGame.Scripts.Ui.CardNode;

namespace XCardGame.Scripts.InputHandling;

public abstract class BaseItemCardSelectTargetInputHandler<TOriginateCard>: BaseSelectTargetInputHandler<CardNode, BaseCard> where TOriginateCard: BaseCard
{
    public Battle Battle;
    public BaseButton ProceedButton;
    public CardNode OriginateCardNode;
    public TOriginateCard OriginateCard;
    
    public BaseItemCardSelectTargetInputHandler(GameMgr gameMgr, CardNode originate) : base(gameMgr)
    {
        OriginateCardNode = originate;
        OriginateCard = (TOriginateCard)originate.Content.Value;
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
        ProceedButton.Pressed -= Confirm;
        OriginateCardNode.OnPressed -= OnOriginateCardPressed;
    }

    protected virtual void Confirm()
    {
        Exit();
    }

    protected void OnOriginateCardPressed(Ui.BaseContentNode<BaseCard> node)
    {
        Exit();
    }
}