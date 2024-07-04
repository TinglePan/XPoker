using System.Collections.Generic;
using XCardGame.Scripts.Cards;
using XCardGame.Scripts.Game;
using XCardGame.Scripts.HandEvaluate;

namespace XCardGame.Scripts.Effects.AgainstEntityEffects;

public class BaseAgainstEntityEffect: BaseEffect
{
    public Engage Engage;
    public BattleEntity Src;
    public BattleEntity Dst;
    
    public BaseAgainstEntityEffect(string name, string descriptionTemplate, BattleEntity src, BattleEntity dst, BaseCard originateCard) : 
        base(name, descriptionTemplate, originateCard)
    {
        Src = src;
        Dst = dst;
    }

    public override void Setup(Dictionary<string, object> args)
    {
        base.Setup(args);
        if (args.TryGetValue("engage", out var engage))
        {
            Engage = (Engage)engage;
        }
    }

    public virtual string GetDescription()
    {
        return DescriptionTemplate;
    }

    public virtual bool CanTrigger(CompletedHand hand)
    {
        return OriginateCard.IsFunctioning();
    }

    public virtual void Resolve()
    {
        
    }
    
    
}