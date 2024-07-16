﻿using System;
using System.Collections.Generic;
using Godot;
using XCardGame.Scripts.Cards.InteractCards.RuleCards;
using XCardGame.Scripts.Common;
using XCardGame.Scripts.Common.Constants;
using XCardGame.Scripts.Common.DataBinding;
using XCardGame.Scripts.Defs;
using XCardGame.Scripts.Defs.Def.Card;
using XCardGame.Scripts.Effects;
using XCardGame.Scripts.Effects.AgainstEntityEffects;
using XCardGame.Scripts.Game;
using XCardGame.Scripts.Ui;
using Battle = XCardGame.Scripts.Game.Battle;
using BattleEntity = XCardGame.Scripts.Game.BattleEntity;
using CardNode = XCardGame.Scripts.Ui.CardNode;

namespace XCardGame.Scripts.Cards;

public class BaseCard: ILifeCycleTriggeredInBattle, IContent, IComparable<BaseCard>
{
    public class SetupArgs
    {
        public GameMgr GameMgr;
        public Battle Battle;
        public BaseContentNode Node;
    }
    // public string Name;
    // public string Description;
    // public string OriginalIconPath;
    // public Enums.CardSuit OriginalSuit;
    // public Enums.CardRank OriginalRank;
    // public int BasePrice;
    
    public GameMgr GameMgr;
    public Battle Battle;
    public BattleEntity OwnerEntity;
    
    public BaseCardDef Def;
    public HashSet<BaseContentNode> Nodes { get; private set; }
    public ObservableProperty<string> IconPath;
    public ObservableProperty<Enums.CardSuit> Suit;
    public ObservableProperty<Enums.CardRank> Rank;
    public ObservableProperty<bool> IsNegated;
    
    public BaseCard(BaseCardDef def)
    {
        Def = def;
        Nodes = new HashSet<BaseContentNode>();
        IconPath = new ObservableProperty<string>(nameof(IconPath), this, def.IconPath);
        Suit = new ObservableProperty<Enums.CardSuit>(nameof(Suit), this, def.Suit);
        Rank = new ObservableProperty<Enums.CardRank>(nameof(Rank), this, def.Rank);
        IsNegated = new ObservableProperty<bool>(nameof(IsNegated), this, false);
        IsNegated.DetailedValueChanged += OnToggleIsNegated;
    }

    public TContentNode Node<TContentNode>() where TContentNode : BaseContentNode
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

    public virtual void Setup(SetupArgs args)
    {
        GameMgr = args.GameMgr;
        Battle = args.Battle;
        Nodes.Add(args.Node);
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
        return $"{Def.Name}({Description()})";
    }

    public virtual bool IsFunctioning()
    {
        var node = Node<CardNode>();
        if (IsNegated.Value) return false;
        if (node.FaceDirection.Value == Enums.CardFace.Down) return false;
        if (node.OnlyDisplay) return false;
        return true;
    }

    public virtual void ChangeRank(int delta)
    {
        var resultRankValue = Utils.GetCardRankValue(Rank.Value) + delta;
        var resultRank = Utils.GetCardRank(resultRankValue);
        Rank.Value = resultRank;
    }
    
    public virtual string Description()
    {
        return Def.DescriptionTemplate;
    }

    public virtual void OnStart(Battle battle)
    {
        
    }

    public virtual void OnStop(Battle battle)
    {
        
    }

    public virtual void Resolve(Battle battle, Engage engage, BattleEntity entity)
    {
        BaseAgainstEntityEffect effect = null;
        bool applyHeartsRule = false;
        if (entity.RoundRole.Value == Enums.EngageRole.Attacker)
        {
            effect = new AttackAgainstEntityEffect(this, entity, battle.GetOpponentOf(entity),
                Utils.GetCardRankValue(Rank.Value), 1);
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
                Utils.GetCardRankValue(Rank.Value), 1);
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
        Battle.GameMgr.BattleLog.Log($"Resolving {this}");
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
            effect?.Resolve();
        }
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