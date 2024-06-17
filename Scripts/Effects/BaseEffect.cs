using System;
using System.Collections.Generic;
using Godot;
using XCardGame.Scripts.Cards;
using XCardGame.Scripts.Common.DataBinding;
using XCardGame.Scripts.GameLogic;
using XCardGame.Scripts.HandEvaluate;
using XCardGame.Scripts.Nodes;
using BuffNode = XCardGame.Scripts.Nodes.BuffNode;

namespace XCardGame.Scripts.Effects;

public class BaseEffect: ILifeCycleTriggeredInBattle, IEquatable<BaseEffect>
{
    public Battle Battle;
    public BaseCard CreatedByCard;
    
    public string Name;
    public string Description;

    public BaseEffect(string name, string description, Battle battle, BaseCard createdByCard)
    {
        Name = name;
        Description = description;
        Battle = battle;
        CreatedByCard = createdByCard;
    }
    
    public bool Equals(BaseEffect other)
    {
        return GetType() == other?.GetType();
    }

    public virtual void OnStart(Battle battle)
    {
    }

    public virtual void OnStop(Battle battle)
    {
    }
}