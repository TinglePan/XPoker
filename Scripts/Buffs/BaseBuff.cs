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
    public ObservableProperty<string> IconPath;

    public BuffNode Node;
    public BattleEntity Entity;
    
    protected GameMgr GameMgr;
    protected Battle Battle;
    
    public BaseBuff(string name, string description, string iconPath, GameMgr gameMgr, BattleEntity entity)
    {
        Name = name;
        Description = description;
        IconPath = new ObservableProperty<string>(nameof(IconPath), this, iconPath);
        Entity = entity;
    }
    
    public virtual void Setup(Dictionary<string, object> args)
    {
        GameMgr = (GameMgr)args["gameMgr"];
        Node = (BuffNode)args["node"];
        Battle = GameMgr.CurrentBattle;
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