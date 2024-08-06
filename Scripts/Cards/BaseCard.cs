﻿using System;
using System.Collections.Generic;
using System.Linq;
using XCardGame.CardProperties;
using XCardGame.Common;
using XCardGame.TimingInterfaces;
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
        SetupProps();
        GameMgr = args.GameMgr;
        Battle = args.Battle;
        Owner = args.Owner;
        Nodes.Add(args.Node);
        
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

    public virtual void Resolve(Battle battle, Engage engage, BattleEntity entity)
    {
        BaseAgainstEntityEffect effect = null;
        bool applyHeartsRule = false;
        if (entity.RoundRole.Value == Enums.EngageRole.Attacker)
        {
            effect = new AttackAgainstEntityEffect(this, entity, battle.GetOpponentOf(entity),
                Utils.GetCardBlackJackValue(Rank.Value), 1);
            foreach (var ruleCard in battle.RuleCardContainer.Cards)
            {
                if (ruleCard is SpadesRuleCard)
                {
                    ((AttackAgainstEntityEffect)effect).Leech = 0.5f;
                } else if (ruleCard is ClubsRuleCard)
                {
                    ((AttackAgainstEntityEffect)effect).RawValue *= 2;
                }
            }
        }
        else if (entity.RoundRole.Value == Enums.EngageRole.Defender)
        {
            effect = new DefendAgainstEntityEffect(this, entity, battle.GetOpponentOf(entity),
                Utils.GetCardBlackJackValue(Rank.Value), 1);
            foreach (var ruleCard in battle.RuleCardContainer.Cards)
            {
                if (ruleCard is HeartsRuleCard)
                {
                    applyHeartsRule = true;
                } else if (ruleCard is DiamondsRuleCard)
                {
                    ((DefendAgainstEntityEffect)effect).RawValue *= 2;
                }
            }
        }
        effect?.Setup(new BaseEffect.SetupArgs
        {
            GameMgr = GameMgr,
            Battle = battle,
        });
        GameMgr.BattleLog.Log($"Resolving {this}");
        // GD.Print($"Resolve of {this}, effect {effect}");
        effect?.Resolve();

        if (applyHeartsRule)
        {
            effect = new HealEffect(this, entity, entity,
                Utils.GetCardRankValue(Rank.Value) / 2, 0);
            effect.Setup(new BaseEffect.SetupArgs
            {
                GameMgr = GameMgr,
                Battle = battle,
            });
            effect.Resolve();
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
        if (Def.IsPiled)
        {
            Props.Add(typeof(CardPropPiled), CreatePiledProp());
        }
        
        if (Def.IsUsable)
        {
            if (Def.IsInnate)
            {
                Props.Add(typeof(CardPropInnate), new CardPropInnate(this));
            }
            if (Def.IsItem)
            {
                Props.Add(typeof(CardPropItem), CreateItemProp());
            }
            if (Def.IsRule)
            {
                Props.Add(typeof(CardPropRule), CreateRuleProp());
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