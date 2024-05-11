﻿using XCardGame.Scripts.Common.Constants;
using XCardGame.Scripts.Common.DataBinding;
using XCardGame.Scripts.GameLogic;

namespace XCardGame.Scripts.Cards;

public class BaseActivatableCard: BaseCard
{
    public int CoolDown;
    public ObservableProperty<int> CoolDownCounter;
    public bool IsQuick;
    public int Cost;
    public int ActualCost =>
        IsQuick ? Cost : Cost + (Battle.Player.Overload.TryGetValue(Suit.Value, out var overload) ? overload.Value : 0);
    
    public BaseActivatableCard(string name, string description, string iconPath, Enums.CardFace face,
        Enums.CardSuit suit, Enums.CardRank rank, int cost, int coolDown, bool isQuick = false, BattleEntity owner = null) : 
        base(name, description, iconPath, face, suit, rank, owner)
    {
        Cost = cost;
        CoolDown = coolDown;
        CoolDownCounter = new ObservableProperty<int>(nameof(CoolDownCounter), this, 0);
        IsQuick = isQuick;
        Battle.OnRoundEnd += OnRoundEnd;
    }
    
    ~BaseActivatableCard()
    {
        Battle.OnRoundEnd -= OnRoundEnd;
    }

    public virtual bool CanActivate()
    {
        return CoolDownCounter.Value == 0 && ActualCost <= Battle.Player.Cost.Value;
    }

    public virtual void Activate()
    {
        
    }

    public virtual void AfterEffect()
    {
        CoolDownCounter.Value = CoolDown;
        Battle.Player.Cost.Value -= ActualCost;
    }

    public virtual void AfterCanceled()
    {
        
    }

    public void OnRoundEnd(Battle battle)
    {
        if (CoolDownCounter.Value > 0)
        {
            CoolDownCounter.Value--;
        }
    }

}