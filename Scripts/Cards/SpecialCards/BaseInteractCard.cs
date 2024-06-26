﻿using System;
using System.Collections.Generic;
using Godot;
using XCardGame.Scripts.Common;
using XCardGame.Scripts.Common.Constants;
using XCardGame.Scripts.Defs;
using XCardGame.Scripts.Defs.Def.Card;
using XCardGame.Scripts.GameLogic;
using XCardGame.Scripts.Nodes;

namespace XCardGame.Scripts.Cards.AbilityCards;

public class BaseInteractCard: BaseCard, IInteractCard
{
    public InteractCardDef InteractCardDef => (InteractCardDef)Def;
    
    protected bool AlreadyFunctioning;
    
    public BaseInteractCard(InteractCardDef def): base(def)
    {
        AlreadyFunctioning = false;
    }

    public virtual bool CanInteract(CardNode node)
    {
        if (!IsFunctioning()) return false;
        if (node.Container.Value is CardContainer { AllowInteract: false }) return false;
        return true;
    }

    public virtual void Interact(CardNode node)
    {
        
    }
    
    public override void OnStart(Battle battle)
    {
        base.OnStart(battle);
        if (IsFunctioning() && !AlreadyFunctioning)
        {
            Battle.OnRoundStart += OnRoundStart;
            AlreadyFunctioning = true;
        }
    }

    public override void OnStop(Battle battle)
    {
        base.OnStop(battle);
        if (AlreadyFunctioning)
        {
            Battle.OnRoundStart -= OnRoundStart;
            AlreadyFunctioning = false;
        }
    }

    public override void ChangeRank(int delta)
    {
        var cardNode = Node<CardNode>();
        var interactCardDef = (InteractCardDef)Def;
        var resultRankValue = Utils.GetCardRankValue(Rank.Value) + delta;
        if (resultRankValue < interactCardDef.SealWhenLessThan && !cardNode.IsTapped.Value)
        {
            cardNode.TweenTap(true, Configuration.TapTweenTime);
        } else if (resultRankValue > interactCardDef.UnSealWhenGreaterThan && cardNode.IsTapped.Value)
        {
            cardNode.TweenTap(false, Configuration.TapTweenTime);
        }
        resultRankValue = Mathf.Clamp(resultRankValue, interactCardDef.NaturalRankChangeRange.X,
            interactCardDef.NaturalRankChangeRange.Y);
        var resultRank = Utils.GetCardRank(resultRankValue);
        Rank.Value = resultRank;
    }
    
    protected void OnRoundStart(Battle battle)
    {
        var interactCardDef = (InteractCardDef)Def;
        var cardNode = Node<CardNode>();
        ChangeRank(cardNode.IsTapped.Value ? interactCardDef.SealedRankChangePerTurn : interactCardDef.RankChangePerTurn);
    }
}