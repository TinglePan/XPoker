﻿using XCardGame.Scripts.Cards.PokerCards;
using XCardGame.Scripts.Common.Constants;
using XCardGame.Scripts.GameLogic;
using XCardGame.Scripts.HandEvaluate;
using XCardGame.Scripts.Ui;

namespace XCardGame.Scripts.Cards.AbilityCards;

public class BasePassiveAbilityCard: BaseAbilityCard, IGameEventTriggered
{

    public int Cost;
    public int ActualCost => Cost + Battle.Player.Fatigue;
    
    
    public BasePassiveAbilityCard(GameMgr gameMgr, string name, string description, Enums.CardFace face,
        BattleEntity owner, string iconPath, int cost) : base(gameMgr, name, description, face, owner, iconPath)
    {
        Cost = cost;
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
    
    public virtual void AfterShowDown(Battle battle)
    {
        
    }
    
    public virtual void OnHoleCardChanged(Battle battle, BattleEntity entity, int index, BasePokerCard from, BasePokerCard to)
    {
        
    }
    
    public virtual void OnCommunityCardChanged(Battle battle, int index, BasePokerCard from, BasePokerCard to)
    {
        
    }
    
    public virtual bool CanFlip()
    {
        return Battle.Player.Cost.Value >= ActualCost;
    }
    
    public virtual void OnFlip(BattleEntity flippedBy)
    {
        if (flippedBy == Battle.Player)
        {
            Battle.Player.Cost.Value -= ActualCost;
        }
    }
}