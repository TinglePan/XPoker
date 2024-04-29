using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Godot;
using XCardGame.Scripts.Cards;
using XCardGame.Scripts.Cards.PokerCards;
using XCardGame.Scripts.Common.Constants;
using XCardGame.Scripts.HandEvaluate;

namespace XCardGame.Scripts.GameLogic;

public partial class BattleEntity: Node, ISetup
{
    public string DisplayName;
    public Deck Deck;
    public ObservableCollection<BasePokerCard> HoleCards;
    public int DealCardCount;
    public int FactionId;
    public int Morale;
    public int MaxMorale;

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
        DealCardCount = args.TryGetValue("startCardCount", out var value) ? (int)value : Configuration.DefaultDealCardCount;
        FactionId = args.TryGetValue("factionId", out value) ? (int)value : 0;
        MaxMorale = args.TryGetValue("maxMorale", out value) ? (int)value : 50;
        HoleCards = new ObservableCollection<BasePokerCard>();
    }
    
    public void Reset()
    {
        HoleCards.Clear();
    }

    public void Attack(BattleEntity target, CompletedHand hand, CompletedHand targetHand)
    {
        throw new NotImplementedException();
    }

    public override string ToString()
    {
        return DisplayName;
    }
}