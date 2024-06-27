using System.Collections.Generic;
using Godot;
using XCardGame.Scripts.Cards;
using XCardGame.Scripts.Common.Constants;
using XCardGame.Scripts.GameLogic;
using XCardGame.Scripts.HandEvaluate;
using XCardGame.Scripts.Nodes;

namespace XCardGame.Scripts.Effects.SkillEffects;

public class BaseAgainstEntityEffect: BaseEffect
{
    public Engage Engage;
    public BattleEntity Self;
    public BattleEntity Opponent;
    
    public BaseAgainstEntityEffect(string name, string descriptionTemplate, BaseCard originateCard) : 
        base(name, descriptionTemplate, originateCard)
    {
    }

    public override void Setup(Dictionary<string, object> args)
    {
        base.Setup(args);
        if (args.TryGetValue("engage", out var engage))
        {
            Engage = (Engage)engage;
        }
        Self = OriginateCard.OwnerEntity;
        Opponent = Battle.GetOpponentOf(Self);
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