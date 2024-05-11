using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Godot;
using XCardGame.Scripts.Buffs;
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
    public int ShowDownHoleCardCountMin;
    public int ShowDownHoleCardCountMax;
    public int FactionId;
    public ObservableProperty<int> Morale;
    public ObservableProperty<int> MaxMorale;
    public Dictionary<Enums.HandTier, int> DamageTable;
    public ObservableProperty<int> Level;
    public ObservableCollection<BaseBuff> Buffs;
    public int CrossTierThreshold;

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
        ShowDownHoleCardCountMin = args.TryGetValue("showDownHoleCardCountMin", out value) ? (int)value : Configuration.DefaultRequiredHoleCardCountMin;
        ShowDownHoleCardCountMax = args.TryGetValue("showDownHoleCardCountMax", out value) ? (int)value : Configuration.DefaultRequiredHoleCardCountMax;
        FactionId = args.TryGetValue("factionId", out value) ? (int)value : 0;
        var maxMorale = args.TryGetValue("maxMorale", out value) ? (int)value : Configuration.DefaultMorale;
        Morale = new ObservableProperty<int>(nameof(Morale), this, maxMorale);
        MaxMorale = new ObservableProperty<int>(nameof(MaxMorale), this, maxMorale);
        DamageTable = (Dictionary<Enums.HandTier, int>)args["damageTable"];
        Level = new ObservableProperty<int>(nameof(Level), this, args.TryGetValue("level", out value) ? (int)value : 1);
        CrossTierThreshold = args.TryGetValue("crossTierThreshold", out value) ? (int)value : Configuration.DefaultCrossTierThreshold;
        HoleCards = new ObservableCollection<BaseCard>();
        Buffs = new ObservableCollection<BaseBuff>();
    }
    
    public virtual void Reset()
    {
        Morale.Value = MaxMorale.Value;
        RoundReset();
    }
    
    public virtual void RoundReset()
    {
        HoleCards.Clear();
    }

    public void Attack(BattleEntity target, CompletedHand hand, CompletedHand targetHand)
    {
        var tier = hand.Tier;
        var damage = DamageTable[tier];
        target.TakeDamage(damage, this);
        if (tier - targetHand.Tier >= CrossTierThreshold)
        {
            InflictBuffOn(new CrossTierDeBuff(_gameMgr, target, tier - targetHand.Tier, 1), target);
        }
    }
    
    public void InflictBuffOn(BaseBuff buff, BattleEntity target)
    {
        target.Buffs.Add(buff);
        buff.OnAppearInField(_battle);
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