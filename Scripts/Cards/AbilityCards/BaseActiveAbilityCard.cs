using System;
using Godot;
using XCardGame.Scripts.Common.Constants;
using XCardGame.Scripts.Common.DataBinding;
using XCardGame.Scripts.GameLogic;

namespace XCardGame.Scripts.Cards.AbilityCards;

public class BaseActiveAbilityCard: BaseAbilityCard
{
    public Action AfterEffect;
    public Action AfterCanceled;
    public int Cost;
    public int CoolDown;
    public ObservableProperty<int> CurrentCoolDown;
    public bool IsQuickAbility;
    
    public int ActualCost => IsQuickAbility ? Cost: Cost + Battle.Player.Fatigue;

    
    public BaseActiveAbilityCard(GameMgr gameMgr, string name, string description, Enums.CardFace face,
        BattleEntity owner, string iconPath, int cost, int coolDown, bool isQuickAbility=false) : base(gameMgr, name, description, face, owner, iconPath)
    {
        Cost = cost;
        Battle.OnNewRound += OnNewRound;
        CoolDown = coolDown;
        CurrentCoolDown = new ObservableProperty<int>(nameof(CurrentCoolDown), this, 0);
        IsQuickAbility = isQuickAbility;
        AfterEffect += AfterEffectHandler;
    }

    public override bool CanActivate()
    {
        return CurrentCoolDown.Value == 0 && ActualCost <= Battle.Player.Cost.Value;
    }

    public override void Activate()
    {
        base.Activate();
        AfterEffect?.Invoke();
    }

    protected void OnNewRound()
    {
        CurrentCoolDown.Value = Mathf.Max(0, CurrentCoolDown.Value - 1);
    }

    protected void AfterEffectHandler()
    {
        CurrentCoolDown.Value = CoolDown;
        Battle.Player.Cost.Value -= ActualCost;
    }
}