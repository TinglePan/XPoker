using XCardGame.Scripts.Common.Constants;
using XCardGame.Scripts.GameLogic;

namespace XCardGame.Scripts.Cards.PokerCards;

public class EventTriggeredPokerCard: PokerCard, IGameEventTriggeredInBattle
{
    public EventTriggeredPokerCard(Enums.CardSuit cardSuit, Enums.CardFace face, Enums.CardRank rank, BattleEntity owner = null, bool suitAsSecondComparer = false) : base(cardSuit, face, rank, owner, suitAsSecondComparer)
    {
    }

    public virtual void BeforeDealCard(Battle battle, BattleEntity entity)
    {
        
    }

    public virtual void AfterDealCard(Battle battle, BattleEntity entity)
    {
        
    }

    public virtual void OnRoundStart(Battle battle)
    {
        
    }

    public virtual void OnRoundEnd(Battle battle)
    {
        
    }

    public virtual void BeforeShowDown(Battle battle)
    {
        
    }

    public virtual void BeforeEngage(Battle battle)
    {
        
    }

    public virtual void BeforeApplyDamage(Battle battle, AttackObj attackObj)
    {
        
    }

    public virtual void AfterShowDown(Battle battle)
    {
        
    }

    public virtual void OnHoleCardChanged(Battle battle, BattleEntity entity, int index, PokerCard from, PokerCard to)
    {
        
    }

    public virtual void OnCommunityCardChanged(Battle battle, int index, PokerCard from, PokerCard to)
    {
        
    }
}