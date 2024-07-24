using System;

namespace XCardGame;

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
    
    public virtual void Setup(object o)
    {
        var args = (SetupArgs)o;
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

    public virtual void OnStartEffect(Battle battle)
    {
    }

    public virtual void OnStopEffect(Battle battle)
    {
    }

}