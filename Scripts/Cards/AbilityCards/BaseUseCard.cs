using System.Collections.Generic;
using XCardGame.Scripts.Common;
using XCardGame.Scripts.Common.Constants;
using XCardGame.Scripts.GameLogic;
using XCardGame.Scripts.Nodes;

namespace XCardGame.Scripts.Cards.AbilityCards;

public class BaseUseCard: BaseInteractCard, IUseCard
{
    public bool IsRecharging { get; private set; }
    public int RankChangePerUse { get; private set; }
    public int Cost { get; }
    
    public BaseUseCard(string name, string description, string iconPath, Enums.CardSuit suit, Enums.CardRank rank,
        int cost, int rankChangePerUse = -1) : 
        base(name, description, iconPath, suit, rank)
    {
        Cost = cost;
        IsRecharging = false;
        RankChangePerUse = rankChangePerUse;
    }
    
    public override void Setup(Dictionary<string, object> args)
    {
        base.Setup(args);
        Battle.OnRoundStart += OnRoundStart;
    }

    public override bool CanInteract()
    {
        return ((CardContainer)Node.Container).AllowInteract && !IsRecharging;
    }
    
    public override void Interact()
    {
        base.Interact();
        ChooseTargets();
    }

    public virtual void ChooseTargets()
    {
        Use();
    }

    public virtual void Use()
    {
        var newRank = Utils.GetCardRank(Utils.GetCardRankValue(Rank.Value) + RankChangePerUse);
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
        Rank.Value = OriginalRank;
    }

    protected void StartRecharge()
    {
        IsRecharging = true;
        Node.TweenTap(true, Configuration.TapTweenTime);
    }
    
    protected void DoneRecharge()
    {
        IsRecharging = false;
        Node.TweenTap(false, Configuration.TapTweenTime);
    }
}