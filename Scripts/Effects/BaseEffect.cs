using System;
using System.Collections.Generic;
using Godot;
using XCardGame.Scripts.Cards;
using XCardGame.Scripts.Common.DataBinding;
using XCardGame.Scripts.Game;

using XCardGame.Scripts.HandEvaluate;

using Battle = XCardGame.Scripts.Game.Battle;
using BuffNode = XCardGame.Scripts.Ui.BuffNode;

namespace XCardGame.Scripts.Effects;

public class BaseEffect: ILifeCycleTriggeredInBattle, IEquatable<BaseEffect>
{

    public class SetupArgs
    {
        public GameMgr GameMgr;
        public Battle Battle;
    }
    
    public GameMgr GameMgr;
    public Battle Battle;
    public BaseCard OriginateCard;
    
    public string Name;
    public string DescriptionTemplate;

    public BaseEffect(string name, string descriptionTemplate, BaseCard originateCard)
    {
        Name = name;
        DescriptionTemplate = descriptionTemplate;
        OriginateCard = originateCard;
    }
    
    public virtual void Setup(SetupArgs args)
    {
        GameMgr = args.GameMgr;
        Battle = args.Battle;
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