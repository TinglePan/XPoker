using System;
using System.Collections.Generic;
using Godot;
using XCardGame.Scripts.Common;
using XCardGame.Scripts.Common.Constants;
using XCardGame.Scripts.Common.DataBinding;
using XCardGame.Scripts.Defs;
using XCardGame.Scripts.Defs.Def.Card;
using XCardGame.Scripts.Effects;
using XCardGame.Scripts.Effects.SkillEffects;
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
    public ObservableProperty<bool> IsNegated;
    
    public BaseCard(BaseCardDef def)
    {
        Def = def;
        Nodes = new HashSet<BaseContentNode<BaseCard>>();
        IconPath = new ObservableProperty<string>(nameof(IconPath), this, def.IconPath);
        Suit = new ObservableProperty<Enums.CardSuit>(nameof(Suit), this, def.Suit);
        Rank = new ObservableProperty<Enums.CardRank>(nameof(Rank), this, def.Rank);
        IsNegated = new ObservableProperty<bool>(nameof(IsNegated), this, false);
        IsNegated.DetailedValueChanged += OnToggleIsNegated;
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
    
    public override string ToString()
    {
        return $"{Def.Name}({GetDescription()})";
    }

    public virtual bool IsFunctioning()
    {
        var node = Node<CardNode>();
        if (IsNegated.Value) return false;
        if (node.FaceDirection.Value == Enums.CardFace.Down) return false;
        if (!node.WithCardEffect) return false;
        return true;
    }

    public virtual void ChangeRank(int delta)
    {
        var resultRankValue = Utils.GetCardRankValue(Rank.Value) + delta;
        var resultRank = Utils.GetCardRank(resultRankValue);
        Rank.Value = resultRank;
    }
    
    public virtual string GetDescription()
    {
        return Def.DescriptionTemplate;
    }

    public virtual void OnStart(Battle battle)
    {
        
    }

    public virtual void OnStop(Battle battle)
    {
        
    }

    public virtual void Resolve(Battle battle, Engage engage, Enums.EngageRole role)
    {
        BaseAgainstEntityEffect effect = null;
        if (role == Enums.EngageRole.Attacker)
        {
            effect = new AttackAgainstEntityEffect(this, Utils.GetCardRankValue(Rank.Value), 1);
        }
        else if (role == Enums.EngageRole.Defender)
        {
            effect = new DefendAgainstEntityEffect(this, Utils.GetCardRankValue(Rank.Value), 1);
        }
        effect?.Setup(new Dictionary<string, object>()
        {
            { "battle", battle },
            { "engage", engage }
        });
        effect?.Resolve();
    }

    protected void OnToggleIsNegated(object sender, ValueChangedEventDetailedArgs<bool> args)
    {
        if (args.NewValue)
        {
            OnStop(Battle);
        }
        else
        {
            OnStart(Battle);
        }
    }
}