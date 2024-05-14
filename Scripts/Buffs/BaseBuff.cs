using System.Collections.Generic;
using XCardGame.Scripts.Cards;
using XCardGame.Scripts.Common.DataBinding;
using XCardGame.Scripts.GameLogic;
using XCardGame.Scripts.HandEvaluate;
using XCardGame.Scripts.Ui;

namespace XCardGame.Scripts.Buffs;

public class BaseBuff: ILifeCycleTriggeredInBattle
{
    public string Name;
    public string Description;
    public string IconPath;

    public BattleEntity Entity;
    
    protected GameMgr GameMgr;
    protected Battle Battle;
    
    public BaseBuff(string name, string description, string iconPath, GameMgr gameMgr, BattleEntity entity)
    {
        Name = name;
        Description = description;
        IconPath = iconPath;
        Entity = entity;
        GameMgr = gameMgr;
        Battle = gameMgr.CurrentBattle;
    }

    public virtual void OnAppear(Battle battle)
    {
    }

    public virtual void OnDisappear(Battle battle)
    {
    }

    public void OnDisposal(Battle battle)
    {
    }
}