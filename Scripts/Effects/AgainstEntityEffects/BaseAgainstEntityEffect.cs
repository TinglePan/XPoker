using System.Collections.Generic;
using XCardGame.Scripts.Cards;
using XCardGame.Scripts.Game;
using XCardGame.Scripts.HandEvaluate;

namespace XCardGame.Scripts.Effects.AgainstEntityEffects;

public class BaseAgainstEntityEffect: BaseEffect
{
    public Engage Engage => Battle.RoundEngage;
    public BattleEntity Src;
    public BattleEntity Dst;
    
    public BaseAgainstEntityEffect(string name, string descriptionTemplate, BaseCard originateCard, BattleEntity src, BattleEntity dst) : 
        base(name, descriptionTemplate, originateCard)
    {
        Src = src;
        Dst = dst;
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