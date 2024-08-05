using System.Collections.Generic;
using System.Threading.Tasks;
using Godot;
using XCardGame.Common;
using XCardGame.Common.HelperBoilerPlates;
using XCardGame.TimingInterfaces;
using XCardGame.Ui;

namespace XCardGame.CardProperties;

public class CardPropRule: BaseCardPropUsable, IStartStopEffect
{
    public bool IsEffectActive { get; set; }
    
    public CardPropRule(BaseCard card) : base(card)
    {
        IsEffectActive = false;
    }

    public override bool CanUse()
    {
        if (!base.CanUse()) return false;
        if (CardNode.CurrentContainer.Value is CardContainer { AllowUseRuleCard: true }) return false;
        return true;
    }

    public override async Task Effect(List<CardNode> targets)
    {
        await CardNode.AnimateTap(!CardNode.IsTapped.Value, Configuration.TapTweenTime);
    }

    public virtual void OnStartEffect()
    {
        if (!IsEffectActive)
        {
            IsEffectActive = true;
        }
    }
    
    public virtual void OnStopEffect()
    {
        if (IsEffectActive)
        {
            IsEffectActive = false;
        }
    }
}