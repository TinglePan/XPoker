using System;

namespace XCardGame;

public class BaseEffect: IEquatable<BaseEffect>
{

    public class SetupArgs
    {
        public GameMgr GameMgr;
        public Battle Battle;
    }

    public BaseCard OriginateCard;
    public GameMgr GameMgr => OriginateCard.GameMgr;
    public Battle Battle => OriginateCard.Battle;
    
    public string Name;
    public string DescriptionTemplate;

    public BaseEffect(string name, string descriptionTemplate, BaseCard originateCard)
    {
        Name = name;
        DescriptionTemplate = descriptionTemplate;
        OriginateCard = originateCard;
    }
    
    public bool Equals(BaseEffect other)
    {
        return GetType() == other?.GetType();
    }

    public virtual string Description()
    {
        return DescriptionTemplate;
    }

}