using System.Collections.Generic;
using XCardGame.Scripts.Cards.PokerCards;
using XCardGame.Scripts.Common.DataBinding;
using XCardGame.Scripts.GameLogic;
using XCardGame.Scripts.HandEvaluate;
using XCardGame.Scripts.Ui;

namespace XCardGame.Scripts.Buffs;

public class BaseBuff: IGameEventTriggeredInBattle
{
    public string Name;
    public string Description;
    public string IconPath;

    public BattleEntity Entity;
    
    protected GameMgr GameMgr;
    
    public BaseBuff(string name, string description, string iconPath, GameMgr gameMgr, BattleEntity entity)
    {
        Name = name;
        Description = description;
        IconPath = iconPath;
        GameMgr = gameMgr;
        Entity = entity;
    }
    
    public virtual void OnInflicted(Battle battle)
    {
    }
    
    public virtual void OnExhausted(Battle battle)
    {
    }

    public virtual void AfterAddedToContainer(Battle battle, CardContainer container)
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
}