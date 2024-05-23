﻿using System.Collections.Generic;
using XCardGame.Scripts.Common.Constants;
using XCardGame.Scripts.Effects;
using XCardGame.Scripts.GameLogic;
using XCardGame.Scripts.Nodes;

namespace XCardGame.Scripts.Cards.AbilityCards;

public class BaseTapCard: BaseInteractCard, ITapCard
{
    public int TapCost { get; private set; }
    public int UnTapCost { get; private set; }
    
    public BaseEffect Effect { get; protected set; }
    
    public BaseTapCard(string name, string description, string iconPath, Enums.CardSuit suit, Enums.CardRank rank, int tapCost, int unTapCost) : base(name, description, iconPath, suit, rank)
    {
        TapCost = tapCost;
        UnTapCost = unTapCost;
    }
    
    public override void Setup(Dictionary<string, object> args)
    {
        base.Setup(args);
        Battle.OnRoundStart += OnRoundStart;
    }
    
    public override bool CanInteract()
    {
        return ((CardContainer)Node.Container).AllowEffect && Node.IsTapped && Battle.Player.Energy.Value >= TapCost || !Node.IsTapped && Battle.Player.Energy.Value >= UnTapCost;
    }

    public void StartEffect()
    {
        if (((CardContainer)Node.Container).AllowEffect)
        {
            Battle.StartEffect(Effect);
        }
    }

    public void ToggleTap()
    {
        Node.TweenTap(!Node.IsTapped, Configuration.TapTweenTime);
        Battle.Player.Energy.Value -= Node.IsTapped ? UnTapCost : TapCost;
    }

    // NOTE: Effect manages its stop on its own. And check if its source card in in position to decide whether its effect is skipped.
    // public override void OnStop(Battle battle)
    // {
    //     battle.StopEffect(Effect);
    // }
    
    protected void OnRoundStart(Battle battle)
    {
        if (!Node.IsTapped && !Node.IsNegated && Effect != null)
        {
            Battle.StartEffect(Effect);
        } 
    }
}