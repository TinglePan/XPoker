using System.Collections.Generic;
using Godot;
using XCardGame.Scripts.Cards;
using XCardGame.Scripts.Common.DataBinding;
using XCardGame.Scripts.GameLogic;
using XCardGame.Scripts.HandEvaluate;
using XCardGame.Scripts.Nodes;
using BuffNode = XCardGame.Scripts.Nodes.BuffNode;

namespace XCardGame.Scripts.Effects;

public class BaseEffect: ILifeCycleTriggeredInBattle
{
    public BaseCard CreatedByCard;
    
    public bool HasSetup { get; set; }
    
    public string Name;
    public string Description;
    public string IconPath;
    
    public BaseEffect(string name, string description, string iconPath, BaseCard createdByCard)
    {
        HasSetup = false;
        Name = name;
        Description = description;
        IconPath = iconPath;
        CreatedByCard = createdByCard;
    }
    
    ~BaseEffect()
    {
        OnDisposal(CreatedByCard.Battle);
    }

    public void EnsureSetup()
    {
        if (!HasSetup)
        {
            GD.PrintErr($"{this} not setup yet");
        }
    }

    public virtual void OnStart(Battle battle)
    {
    }

    public virtual void OnStop(Battle battle)
    {
    }

    public void OnDisposal(Battle battle)
    {
        OnStop(battle);
    }
}