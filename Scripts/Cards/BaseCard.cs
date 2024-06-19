using System;
using System.Collections.Generic;
using Godot;
using XCardGame.Scripts.Common.Constants;
using XCardGame.Scripts.Common.DataBinding;
using XCardGame.Scripts.Defs;
using XCardGame.Scripts.GameLogic;
using XCardGame.Scripts.Nodes;
using CardNode = XCardGame.Scripts.Nodes.CardNode;

namespace XCardGame.Scripts.Cards;

public class BaseCard: ISetup, ILifeCycleTriggeredInBattle, IContent<BaseCard>, IComparable<BaseCard>
{
    // public string Name;
    // public string Description;
    // public string OriginalIconPath;
    // public Enums.CardSuit OriginalSuit;
    // public Enums.CardRank OriginalRank;
    // public int BasePrice;

    public BaseCardDef Def;
    
    public GameMgr GameMgr;
    public Battle Battle;
    public BattleEntity OwnerEntity;
    public HashSet<BaseContentNode<BaseCard>> Nodes { get; private set; }
    public bool HasSetup { get; set; }
    public ObservableProperty<string> IconPath;
    public ObservableProperty<Enums.CardSuit> Suit;
    public ObservableProperty<Enums.CardRank> Rank;
    public bool IsTapped;
    public bool IsNegated;
    
    public Enums.CardColor CardColor => Suit.Value switch
    {
        Enums.CardSuit.Spades => Enums.CardColor.Black,
        Enums.CardSuit.Clubs => Enums.CardColor.Black,
        Enums.CardSuit.Hearts => Enums.CardColor.Red,
        Enums.CardSuit.Diamonds => Enums.CardColor.Red,
        _ => Enums.CardColor.None
    };
    
    public BaseCard(BaseCardDef def)
    {
        Def = def;
        Nodes = new HashSet<BaseContentNode<BaseCard>>();
        IconPath = new ObservableProperty<string>(nameof(IconPath), this, def.IconPath);
        Suit = new ObservableProperty<Enums.CardSuit>(nameof(Suit), this, def.Suit);
        Rank = new ObservableProperty<Enums.CardRank>(nameof(Rank), this, def.Rank);
        IsTapped = false;
        IsNegated = false;
    }

    public TContentNode Node<TContentNode>() where TContentNode : BaseContentNode<TContentNode, BaseCard>
    {
        foreach (var node in Nodes)
        {
            if (node is TContentNode contentNode)
            {
                return contentNode;
            }
        }
        return null;
    }

    public virtual void Setup(Dictionary<string, object> args)
    {
        GameMgr = (GameMgr)args["gameMgr"];
        Battle = (Battle)args["battle"];
        Nodes.Add((CardNode)args["node"]);
    }

    public void EnsureSetup()
    {
        if (!HasSetup)
        {
            GD.PrintErr($"{this} not setup yet");
        }
    }
    
    public override string ToString()
    {
        return $"{Def.Name}({GetDescription()})";
    }

    public virtual void OnStart(Battle battle)
    {
        
    }

    public virtual void OnStop(Battle battle)
    {
        
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

    public virtual string GetDescription()
    {
        return Def.DescriptionTemplate;
    }
    
    public virtual async void ToggleTap()
    {
        var targetTapValue = !IsTapped;
        var node = Node<CardNode>();
        node.TweenTap(targetTapValue, Configuration.TapTweenTime);
    }
}