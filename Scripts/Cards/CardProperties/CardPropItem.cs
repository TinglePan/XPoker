using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Godot;
using XCardGame.Common;
using XCardGame.Ui;

namespace XCardGame.CardProperties;

public class CardPropItem: BaseCardPropUsable, ICardRankChange, IRoundEnd
{
    public Action<BaseCard> OnOverload;
    public int RankChangePerUse;
    
    public CardPropItem(BaseCard card) : base(card)
    {
        RankChangePerUse = card.Def.RankChangePerUse;
    }
    
    public void OnCardRankChange()
    {
        if (Card.Rank.Value == Enums.CardRank.None)
        {
            OnOverload?.Invoke(Card);
        }
    }

    public override bool CanUse()
    {
        if (!base.CanUse()) return false;
        if (CardNode.CurrentContainer.Value is CardContainer { AllowUseItemCard: false }) return false;
        if (!CardNode.IsEffective.Value && Card.IsEffective.Value) return false;
        return true;
    }
    
    public override Task Effect(List<CardNode> targets)
    {
        Card.ChangeRank(RankChangePerUse);
        return Task.CompletedTask;
    }
    
    public async void OnRoundEnd()
    {
        if (Card.Rank.Value == Enums.CardRank.None)  // Overload
        {
            await GameMgr.AwaitAndDisableInput(CardNode?.AnimateLeaveField());
        } else if (Card.Rank.Value != Enums.CardRank.None && Card.Rank.Value < Card.OriginalRank)
            // Recharge
        {
            Card.ChangeRank(Mathf.Min(Card.Battle.Player.ItemRecharge.Value, Card.OriginalRank - Card.Rank.Value));
        }
    }
}