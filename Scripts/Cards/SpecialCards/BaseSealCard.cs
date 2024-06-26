﻿using System.Collections.Generic;
using System.Diagnostics;
using XCardGame.Scripts.Common.Constants;
using XCardGame.Scripts.Defs;
using XCardGame.Scripts.Defs.Def.Card;
using XCardGame.Scripts.Effects;
using XCardGame.Scripts.GameLogic;
using XCardGame.Scripts.Nodes;

namespace XCardGame.Scripts.Cards.AbilityCards;

public class BaseTapCard: BaseInteractCard
{
    public BaseTapCard(InteractCardDef def) : base(def)
    {
        Debug.Assert(def.InteractionType == Enums.InteractionType.Seal);
    }
    
    public override bool CanInteract(CardNode node)
    {
        if (!base.CanInteract(node)) return false;
        var interactCardDef = (InteractCardDef)Def;
        if (node.Container.Value is CardContainer cardContainer && cardContainer.ExpectedInteractCardType != interactCardDef.Type) return false;
        if (node.IsTapped.Value)
        {
            // UnSeal
            if (Battle.Player.MaxEnergy.Value + interactCardDef.SealCost < interactCardDef.UnSealCost) return false;
        }
        else
        {
            // Seal
            if (Battle.Player.MaxEnergy.Value + interactCardDef.UnSealCost < interactCardDef.SealCost) return false;
        }
        return true;
    }

    public override void Interact(CardNode node)
    {
        var interactCardDef = (InteractCardDef)Def;
        if (node.IsTapped.Value)
        {
            // UnSeal
            Battle.Player.MaxEnergy.Value = Battle.Player.MaxEnergy.Value - interactCardDef.UnSealCost + interactCardDef.SealCost;
        }
        else
        {
            // Seal
            Battle.Player.MaxEnergy.Value = Battle.Player.MaxEnergy.Value + interactCardDef.UnSealCost - interactCardDef.SealCost;
        }
        node.TweenTap(!node.IsTapped.Value, Configuration.TapTweenTime);
    }
}