using System;
using System.Collections.Generic;
using Godot;
using XCardGame.Scripts.Common.Constants;
using XCardGame.Scripts.Common.DataBinding;
using XCardGame.Scripts.GameLogic;
using XCardGame.Scripts.Nodes;
using CardNode = XCardGame.Scripts.Nodes.CardNode;

namespace XCardGame.Scripts.Cards;

public class BaseCard: ISetup, ILifeCycleTriggeredInBattle, IContent<CardNode, BaseCard>, IComparable<BaseCard>
{
    public CardNode Node { get; set; }
    public GameMgr GameMgr;
    public Battle Battle;
    public BattleEntity Owner;
    public bool HasSetup { get; set; }
    
    public string Name;
    public string Description;
    public ObservableProperty<string> IconPath;
    public Enums.CardSuit OriginalSuit;
    public ObservableProperty<Enums.CardSuit> Suit;
    public Enums.CardRank OriginalRank;
    public ObservableProperty<Enums.CardRank> Rank;
    
    public Enums.CardColor CardColor => Suit.Value switch
    {
        Enums.CardSuit.Spades => Enums.CardColor.Black,
        Enums.CardSuit.Clubs => Enums.CardColor.Black,
        Enums.CardSuit.Hearts => Enums.CardColor.Red,
        Enums.CardSuit.Diamonds => Enums.CardColor.Red,
        _ => Enums.CardColor.None
    };
    
    public BaseCard(string name, string description, string iconPath, Enums.CardSuit suit = Enums.CardSuit.None,
        Enums.CardRank rank = Enums.CardRank.None, BattleEntity owner = null)
    {
        Name = name;
        Description = description;
        IconPath = new ObservableProperty<string>(nameof(IconPath), this, iconPath);
        OriginalSuit = suit;
        Suit = new ObservableProperty<Enums.CardSuit>(nameof(Suit), this, suit);
        OriginalRank = rank;
        Rank = new ObservableProperty<Enums.CardRank>(nameof(Rank), this, rank);
        Owner = owner;
    }

    public override string ToString()
    {
        return $"{Name}({Description})";
    }

    public virtual void Setup(Dictionary<string, object> args)
    {
        GameMgr = (GameMgr)args["gameMgr"];
        Battle = (Battle)args["battle"];
        Node = (CardNode)args["node"];
    }

    public void EnsureSetup()
    {
        if (!HasSetup)
        {
            GD.PrintErr($"{this} not setup yet");
        }
    }

    public virtual void OnStart(Battle battle)
    {
        
    }

    public virtual void OnStop(Battle battle)
    {
        
    }
    
    public virtual void OnDisposal(Battle battle)
    {
        OnStop(battle);
    }
    
    public int CompareTo(BaseCard other)
    {
        return CompareTo(other, false);
    }

    public int CompareTo(BaseCard other, bool isSuitSecondComparer)
    {
        var res = Rank.Value.CompareTo(other.Rank.Value);
        if (res == 0 && isSuitSecondComparer)
        {
            res = Suit.Value.CompareTo(other.Suit.Value);
        }
        return res;
    }
}