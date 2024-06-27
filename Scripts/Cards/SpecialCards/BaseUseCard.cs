using System.Collections.Generic;
using System.Diagnostics;
using Godot;
using XCardGame.Scripts.Common;
using XCardGame.Scripts.Common.Constants;
using XCardGame.Scripts.Common.DataBinding;
using XCardGame.Scripts.Defs;
using XCardGame.Scripts.Defs.Def.Card;
using XCardGame.Scripts.GameLogic;
using XCardGame.Scripts.Nodes;

namespace XCardGame.Scripts.Cards.AbilityCards;

public class BaseUseCard: BaseInteractCard
{
    public BaseUseCard(InteractCardDef def): base(def)
    {
        Debug.Assert(def.InteractionType == Enums.InteractionType.Use, $"Incorrect card type for this interaction type {def.InteractionType}");
    }

    public override bool CanInteract(CardNode node)
    {
        if (!base.CanInteract(node)) return false;
        if (node.IsTapped.Value) return false;
        var interactCardDef = (InteractCardDef)Def;
        if (node.Container.Value is CardContainer cardContainer && cardContainer.ExpectedInteractCardType != interactCardDef.Type) return false;
        if (Battle.Player.Energy.Value < interactCardDef.UseCost) return false;
        return true;
    }

    public override void Interact(CardNode node)
    {
        ChooseTargets(node);
    }

    public virtual void ChooseTargets(CardNode node)
    {
        Use(node);
    }

    public virtual void Use(CardNode node)
    {
        var interactCardDef = (InteractCardDef)Def;
        ChangeRank(interactCardDef.RankChangePerUse);
        Battle.Player.Energy.Value -= interactCardDef.UseCost;
    }
}