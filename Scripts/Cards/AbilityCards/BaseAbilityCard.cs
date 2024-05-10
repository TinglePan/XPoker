using System.Collections.Generic;
using Godot;
using XCardGame.Scripts.Cards.PokerCards;
using XCardGame.Scripts.Common.Constants;
using XCardGame.Scripts.GameLogic;
using XCardGame.Scripts.HandEvaluate;

namespace XCardGame.Scripts.Cards.AbilityCards;

public class BaseAbilityCard: BaseCard, IGameEventTriggeredInBattle, ILifeCycleTriggeredInBattle
{
    public string IconPath;
    
    protected GameMgr GameMgr;
    protected Battle Battle;
    
    public BaseAbilityCard(GameMgr gameMgr, string name, string description, Enums.CardFace face, Enums.CardSuit suit, string iconPath, BattleEntity owner=null) : base(name, description, face, suit, owner)
    {
        IconPath = iconPath;
        GameMgr = gameMgr;
        Battle = GameMgr.CurrentBattle;
    }

    public void Disposal()
    {
        OnExhausted(Battle);
        Node.QueueFree();
    }
    
    public virtual void AfterEffect()
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

    public virtual void OnHoleCardChanged(Battle battle, BattleEntity entity, int index, BasePokerCard from, BasePokerCard to)
    {
        
    }

    public virtual void OnCommunityCardChanged(Battle battle, int index, BasePokerCard from, BasePokerCard to)
    {
        
    }

    public virtual void OnSpawn(Battle battle)
    {
        
    }

    public virtual void OnExhausted(Battle battle)
    {
        
    }
}