using System.Collections.Generic;
using XCardGame.Scripts.Common;
using XCardGame.Scripts.Common.Constants;
using XCardGame.Scripts.Defs;
using XCardGame.Scripts.GameLogic;
using XCardGame.Scripts.Nodes;

namespace XCardGame.Scripts.Cards.AbilityCards;

public class BaseUseCard: BaseAbilityCard, IUseCard
{
    public readonly UseCardDef UseCardDef;
    public bool IsRecharging { get; private set; }
    
    public BaseUseCard(UseCardDef def): base(def)
    {
        UseCardDef = def;
        IsRecharging = false;
        Interactions.Add(Enums.CardInteractions.Use);
    }
    
    public override void Setup(Dictionary<string, object> args)
    {
        base.Setup(args);
        Battle.OnRoundStart += OnRoundStart;
    }

    public override bool CanInteract()
    {
        var node = Node<CardNode>();
        if (node.Container.Value is CardContainer { AllowInteract: false })
        {
            return false;
        }
        return !IsRecharging;
    }

    public override void Interact()
    {
        ChooseTargets();
    }

    public virtual void ChooseTargets()
    {
        Use();
    }

    public virtual void Use()
    {
        var newRank = Utils.GetCardRank(Utils.GetCardRankValue(Rank.Value) + UseCardDef.RankChangePerUse);
        if (newRank == Enums.CardRank.None)
        {
            StartRecharge();
        }
        else
        {
            Rank.Value = newRank;
        }
    }
    
    protected void OnRoundStart(Battle battle)
    {
        if (IsRecharging)
        {
            DoneRecharge();
        }
        Rank.Value = Def.Rank;
    }

    protected void StartRecharge()
    {
        IsRecharging = true;
        var node = Node<CardNode>();
        node.TweenTap(true, Configuration.TapTweenTime);
    }
    
    protected void DoneRecharge()
    {
        IsRecharging = false;
        var node = Node<CardNode>();
        node.TweenTap(false, Configuration.TapTweenTime);
    }
}