using System;
using System.Collections.Generic;
using System.Linq;
using XCardGame.CardProperties;
using XCardGame.Common;
using XCardGame.Ui;

namespace XCardGame;

public class BaseCard: IContent, IComparable<BaseCard>, ICardUse, IStartStopEffect, IEnterField, ILeaveField, IRoundStart, IRoundEnd
{
    public class SetupArgs
    {
        public GameMgr GameMgr;
        public Battle Battle;
        public BattleEntity Owner;
        public BaseContentNode Node;
        public Enums.CardSuit Suit;
        public Enums.CardRank Rank;
    }
    // public string Name;
    // public string Description;
    // public string OriginalIconPath;
    // public Enums.CardSuit OriginalSuit;
    // public Enums.CardRank OriginalRank;
    // public int BasePrice;
    
    public HashSet<BaseContentNode> Nodes { get; }
    
    public GameMgr GameMgr;
    public Battle Battle;
    public BattleEntity Owner;

    public Action<BaseCard> OnEnterFieldCallback;
    public Action<BaseCard> OnLeaveFieldCallback;
    
    public CardDef Def;
    public Dictionary<Type, BaseCardProp> Props;
    public ObservableProperty<string> IconPath;
    public Enums.CardSuit OriginalSuit;
    public Enums.CardRank OriginalRank;
    public ObservableProperty<Enums.CardSuit> Suit;
    public ObservableProperty<Enums.CardRank> Rank;
    public ObservableProperty<bool> IsNegated;
    public DerivedObservableProperty<bool> IsEffective;

    public bool IsEffectActive => Props.Values.Any(prop => prop is IStartStopEffect { IsEffectActive: true });

    public BaseCard(CardDef def)
    {
        Def = def;
        Props = new Dictionary<Type, BaseCardProp>();
        OriginalRank = def.Rank;
        OriginalSuit = def.Suit;
        Nodes = new HashSet<BaseContentNode>();
        IconPath = new ObservableProperty<string>(nameof(IconPath), this, def.IconPath);
        Suit = new ObservableProperty<Enums.CardSuit>(nameof(Suit), this, def.Suit);
        Rank = new ObservableProperty<Enums.CardRank>(nameof(Rank), this, def.Rank);
        IsNegated = new ObservableProperty<bool>(nameof(IsNegated), this, false);
        IsNegated.DetailedValueChanged += OnToggleIsNegated;
        IsEffective = new DerivedObservableProperty<bool>(nameof(IsEffective), GetIsEffective, IsNegated);
    }

    public virtual void Setup(object o)
    {
        var args = (SetupArgs)o;
        GameMgr = args.GameMgr;
        Battle = args.Battle;
        Owner = args.Owner;
        Nodes.Add(args.Node);
        SetupProps();
        if (OriginalRank == Enums.CardRank.None)
        {
            OriginalRank = args.Rank != Enums.CardRank.None ? args.Rank : RandRank();
            Rank.Value = OriginalRank;
        }
        if (OriginalSuit == Enums.CardSuit.None)
        {
            OriginalSuit = args.Suit != Enums.CardSuit.None ? args.Suit : RandSuit();
            Suit.Value = OriginalSuit;
        }
    }

    public TContentNode Node<TContentNode>(bool strict = true) where TContentNode : BaseContentNode
    {
        return InterfaceContentBoilerPlates.Node<TContentNode>(this, strict);
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

    public virtual bool CanUse()
    {
        return GetProp<BaseCardPropUsable>()?.CanUse() ?? false;
    }

    public void Use()
    {
        GetProp<BaseCardPropUsable>()?.Use();
    }
    
    public void OnStartEffect()
    {
        if (Node<CardNode>() is { IsEffective.Value: true } && IsEffective.Value)
        {
            foreach (var prop in Props.Values)
            {
                if (prop is IStartStopEffect startStopEffect)
                {
                    startStopEffect.OnStartEffect();
                }
            }
        }
    }

    public void OnStopEffect()
    {
        foreach (var prop in Props.Values)
        {
            if (prop is IStartStopEffect { IsEffectActive: true } startStopEffect)
            {
                startStopEffect.OnStopEffect();
            }
        }
    }

    public void OnEnterField()
    {
        OnEnterFieldCallback?.Invoke(this);
        foreach (var prop in Props.Values)
        {
            if (prop is IEnterField enterField)
            {
                enterField.OnEnterField();
            }
        }
    }
    
    public void OnLeaveField()
    {
        Reset();
        OnLeaveFieldCallback?.Invoke(this);
        foreach (var prop in Props.Values)
        {
            if (prop is ILeaveField leaveField)
            {
                leaveField.OnLeaveField();
            }
        }
    }
    
    public void OnRoundStart()
    {
        foreach (var prop in Props.Values)
        {
            if (prop is IRoundStart roundStart)
            {
                roundStart.OnRoundStart();
            }
        }
    }
    
    public void OnRoundEnd()
    {
        foreach (var prop in Props.Values)
        {
            if (prop is IRoundEnd roundEnd)
            {
                roundEnd.OnRoundEnd();
            }
        }
    }
    
    public TProp GetProp<TProp>(bool strict = false) where TProp : BaseCardProp
    {
        foreach (var (type, prop) in Props)
        {
            if (strict)
            {
                if (prop is TProp typedProp)
                {
                    return typedProp;
                }
            }
            else
            {
                if (type.IsAssignableTo(typeof(TProp)))
                {
                    return (TProp)prop;
                }
            }
        }
        return null;
    }
    
    public List<TProp> GetProps<TProp>() where TProp : BaseCardProp
    {
        var res = new List<TProp>();
        foreach (var (type, prop) in Props)
        {
            if (type.IsAssignableTo(typeof(TProp)))
            {
                res.Add((TProp)prop);
            }
        }
        return res;
    }
    
    public virtual string Description()
    {
        return Def.DescriptionTemplate;
    }
    
    public override string ToString()
    {
        return $"{Def.Name}({Description()})";
    }

    public bool GetIsEffective()
    {
        if (IsNegated.Value) return false;
        return true;
    }

    public void ChangeRank(int delta)
    {
        if (Rank.Value == Enums.CardRank.None) return;
        var resultRankValue = Utils.GetCardRankValue(Rank.Value) + delta;
        var resultRank = Utils.GetCardRank(resultRankValue);
        Rank.Value = resultRank;
        foreach (var prop in Props.Values)
        {
            if (prop is ICardRankChange rankChange)
            {
                rankChange.OnCardRankChange();
            }
        }
    }

    public virtual void Resolve(BattleEntity entity)
    {
        if (!Battle.CheckResolveCard(this))
        {
            return;
        }
        if (entity.RoundRole.Value == Enums.EngageRole.Attacker)
        {
            Battle.RoundEngage.PendingEffects.Add(new AttackAgainstEntityEffect(this, entity, Battle.GetOpponentOf(entity),
                Utils.GetCardBlackJackValue(Rank.Value)));
        }
        else if (entity.RoundRole.Value == Enums.EngageRole.Defender)
        {
            Battle.RoundEngage.PendingEffects.Add(new DefendAgainstEntityEffect(this, entity, Battle.GetOpponentOf(entity),
                Utils.GetCardBlackJackValue(Rank.Value)));
        }
        GameMgr.BattleLog.Log($"Resolving {this}");
        // GD.Print($"Resolve of {this}, effect {effect}");
        
        foreach (var prop in Props.Values)
        {
            if (prop is ICardResolve resolve)
            {
                resolve.OnResolve();
            }
        }
    }

    public void Reset()
    {
        Rank.Value = OriginalRank;
        Suit.Value = OriginalSuit;
        IsNegated.Value = false;
    }

    protected virtual void SetupProps()
    {
        if (Def.IsPiled && GetProp<CardPropPiled>() == null)
        {
            var prop = CreatePiledProp();
            Props.Add(prop.GetType(), prop);
        }
        
        if (Def.IsUsable)
        {
            if (Def.IsInnate && GetProp<CardPropInnate>() == null)
            {
                Props.Add(typeof(CardPropInnate), new CardPropInnate(this));
            }
            if (Def.IsItem && GetProp<CardPropItem>() == null)
            {
                var prop = CreateItemProp();
                Props.Add(prop.GetType(), prop);
            }
            if (Def.IsRule && GetProp<CardPropRule>() == null)
            {
                var prop = CreateRuleProp();
                Props.Add(prop.GetType(), prop);
            }
        }
    }
    
    protected virtual CardPropPiled CreatePiledProp()
    {
        return new CardPropPiled(this, Def.PileCardCountMax);
    }
    
    protected virtual CardPropItem CreateItemProp()
    {
        return new CardPropItem(this);
    }
    
    protected virtual CardPropRule CreateRuleProp()
    {
        return new CardPropRule(this);
    }

    protected void OnToggleIsNegated(object sender, ValueChangedEventDetailedArgs<bool> args)
    {
        if (args.NewValue)
        {
            OnStopEffect();
        }
        else
        {
            OnStartEffect();
        }
    }

    protected Enums.CardRank RandRank()
    {
        var rankValue = GameMgr.Rand.Next(Utils.GetCardRankValue(Enums.CardRank.Ace),
            Utils.GetCardRankValue(Enums.CardRank.King) + 1);
        return Utils.GetCardRank(rankValue);
    }

    protected Enums.CardSuit RandSuit()
    {
        var suitValue = GameMgr.Rand.Next(1, 5);
        return (Enums.CardSuit)suitValue;
    }
}