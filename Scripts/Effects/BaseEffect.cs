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

public class BaseEffect: ILifeCycleTriggeredInBattle, IEquatable<BaseEffect>, ISetup
{
    public Battle Battle;
    public BaseCard OriginateCard;

    public bool HasSetup { get; set; }
    
    public string Name;
    public string DescriptionTemplate;

    public BaseEffect(string name, string descriptionTemplate, BaseCard originateCard)
    {
        Name = name;
        DescriptionTemplate = descriptionTemplate;
        OriginateCard = originateCard;
    }
    
    public virtual void Setup(Dictionary<string, object> args)
    {
        Battle = (Battle)args["battle"];
    }
    
    public void EnsureSetup()
    {
        if (!HasSetup)
        {
            GD.PrintErr($"{this} not setup yet");
        }
    }
    
    public bool Equals(BaseEffect other)
    {
        return GetType() == other?.GetType();
    }

    public virtual string Description()
    {
        return DescriptionTemplate;
    }

    public virtual void OnStart(Battle battle)
    {
    }

    public virtual void OnStop(Battle battle)
    {
    }

}