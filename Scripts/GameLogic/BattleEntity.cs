using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Godot;
using XCardGame.Scripts.Cards;
using XCardGame.Scripts.Cards.PokerCards;
using XCardGame.Scripts.Common.Constants;
using XCardGame.Scripts.Common.DataBinding;
using XCardGame.Scripts.HandEvaluate;

namespace XCardGame.Scripts.GameLogic;

public partial class BattleEntity: Node, ISetup
{
    public string DisplayName;
    public Deck Deck;
    public ObservableCollection<BaseCard> HoleCards;
    public int DealCardCount;
    public int FactionId;
    public ObservableProperty<int> Morale;
    public ObservableProperty<int> MaxMorale;
    public Dictionary<Enums.HandTier, int> DamageTable;

    private GameMgr _gameMgr;
    private Battle _battle;

    public override void _Ready()
    {
        _gameMgr = GetNode<GameMgr>("/root/GameMgr");
    }

    public virtual void Setup(Dictionary<string, object> args)
    {
        DisplayName = (string)args["name"];
        _battle = (Battle)args["battle"];
        Deck = (Deck)args["deck"];
        foreach (var card in Deck.CardList)
        {
            card.Owner = this;
        }
        DealCardCount = args.TryGetValue("startCardCount", out var value) ? (int)value : Configuration.DefaultDealCardCount;
        FactionId = args.TryGetValue("factionId", out value) ? (int)value : 0;
        var maxMorale = args.TryGetValue("maxMorale", out value) ? (int)value : 50;
        Morale = new ObservableProperty<int>(nameof(Morale), this, maxMorale);
        MaxMorale = new ObservableProperty<int>(nameof(MaxMorale), this, maxMorale);
        DamageTable = (Dictionary<Enums.HandTier, int>)args["damageTable"];
        HoleCards = new ObservableCollection<BaseCard>();
    }
    
    public virtual void Reset()
    {
        Morale.Value = MaxMorale.Value;
        RoundReset();
    }
    
    public void RoundReset()
    {
        HoleCards.Clear();
    }

    public void Attack(BattleEntity target, CompletedHand hand, CompletedHand targetHand)
    {
        var tier = hand.Tier;
        var damage = DamageTable[tier];
        target.TakeDamage(damage, this);
    }

    public void TakeDamage(int damage, BattleEntity source)
    {
        Morale.Value = Mathf.Clamp(Morale.Value - damage, 0, MaxMorale.Value);
        if (Morale.Value == 0)
        {
            _battle.OnEntityDefeated(this);
        }
    }

    public override string ToString()
    {
        return DisplayName;
    }
}