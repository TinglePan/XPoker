using System.Collections.Generic;
using Godot;
using XCardGame.Scripts.Cards;
using XCardGame.Scripts.Common.Constants;
using XCardGame.Scripts.GameLogic;
using XCardGame.Scripts.HandEvaluate;

namespace XCardGame.Scripts.Effects.SkillEffects;

public class BaseSkillEffect: BaseEffect
{

    public Enums.HandTier TriggerHandTier;
    public int Expanding;
    
    public BaseSkillEffect(string name, string description, Battle battle, BaseCard createdByCard, Enums.HandTier triggerHandTier, int expanding = 0) : 
        base(name, description, battle, createdByCard)
    {
        TriggerHandTier = triggerHandTier;
        Expanding = expanding;
    }

    public bool CanTrigger(CompletedHand hand)
    {
        var handTierValue = Battle.GetHandTierValue(hand.Tier);
        var triggerHandTierValue = Battle.GetHandTierValue(TriggerHandTier);
        return handTierValue >= triggerHandTierValue && handTierValue - triggerHandTierValue <= Expanding;
    }
    
    public List<Enums.HandTier> TriggerHandTiers()
    {
        var tiers = new List<Enums.HandTier>();
        for (var i = 0; i <= Expanding; i++)
        {
            var index = Battle.HandTierOrderAscend.IndexOf(TriggerHandTier);
            tiers.Add(Battle.HandTierOrderAscend[index + i]);
        }
        return tiers;
    }

    public virtual void Resolve(SkillResolver resolver, CompletedHand hand, BattleEntity self, BattleEntity opponent)
    {
    }

    public virtual string GetDescription()
    {
        return Description;
    }
    
    
}