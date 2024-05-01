using System;
using Godot;
using XCardGame.Scripts.Common.Constants;
using XCardGame.Scripts.Common.DataBinding;
using XCardGame.Scripts.GameLogic;

namespace XCardGame.Scripts.Cards.AbilityCards;

public class BaseActiveAbilityCard: BaseAbilityCard
{
    public Action AfterEffect;
    public int Cost;
    public int CoolDown;
    public ObservableProperty<int> CurrentCoolDown;
    public bool IsQuickAbility;
    
    public int ActualCost => IsQuickAbility ? Cost: Cost + Player.Fatigue;

    protected Battle Battle;
    protected PlayerBattleEntity Player;
    
    public BaseActiveAbilityCard(GameMgr gameMgr, string name, string description, Enums.CardFace face,
        BattleEntity owner, string iconPath, int cost, int coolDown, bool isQuickAbility=false) : base(gameMgr, name, description, face, owner, iconPath)
    {
        Cost = cost;
        Battle = gameMgr.CurrentBattle;
        Battle.OnNewRound += OnNewRound;
        CoolDown = coolDown;
        CurrentCoolDown = new ObservableProperty<int>(nameof(CurrentCoolDown), this, 0);
        IsQuickAbility = isQuickAbility;
        Player = (PlayerBattleEntity)Owner;
        AfterEffect += AfterEffectHandler;
    }

    public override bool CanActivate()
    {
        return CurrentCoolDown.Value == 0 && ActualCost <= Player.Cost.Value;
    }

    protected void OnNewRound()
    {
        CurrentCoolDown.Value = Mathf.Max(0, CurrentCoolDown.Value - 1);
    }

    protected void AfterEffectHandler()
    {
        CurrentCoolDown.Value = CoolDown;
        Player.Cost.Value -= ActualCost;
    }
}