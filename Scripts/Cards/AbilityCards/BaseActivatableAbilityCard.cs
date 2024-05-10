using System;
using Godot;
using XCardGame.Scripts.Common.Constants;
using XCardGame.Scripts.Common.DataBinding;
using XCardGame.Scripts.GameLogic;

namespace XCardGame.Scripts.Cards.AbilityCards;

public class BaseActivatableAbilityCard: BaseAbilityCard, IActivatableCard, IWithCost
{
    public int CoolDown;
    public ObservableProperty<int> CoolDownCounter;
    public bool IsQuick { get; }

    public int Cost { get; }

    public int ActualCost =>
        IsQuick ? Cost : Cost + (Battle.Player.Overload.TryGetValue(Suit.Value, out var overload) ? overload.Value : 0);
    
    public BaseActivatableAbilityCard(GameMgr gameMgr, string name, string description, Enums.CardFace face,
        Enums.CardSuit suit, string iconPath, int cost, int coolDown, bool isQuick=false, BattleEntity owner=null) : 
        base(gameMgr, name, description, face, suit, iconPath, owner)
    {
        Cost = cost;
        CoolDown = coolDown;
        CoolDownCounter = new ObservableProperty<int>(nameof(CoolDownCounter), this, 0);
        IsQuick = isQuick;
    }

    public virtual bool CanActivate()
    {
        return CoolDownCounter.Value == 0 && ActualCost <= Battle.Player.Cost.Value;
    }

    public virtual void Activate()
    {
        
    }

    public override void AfterEffect()
    {
        base.AfterEffect();
        CoolDownCounter.Value = CoolDown;
        Battle.Player.Cost.Value -= ActualCost;
    }

    public virtual void AfterCanceled()
    {
        
    }

    public override void OnRoundEnd(Battle battle)
    {
        if (CoolDownCounter.Value > 0)
        {
            CoolDownCounter.Value--;
        }
    }

}