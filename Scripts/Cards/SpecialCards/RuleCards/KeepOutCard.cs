using System;
using System.Collections.Generic;
using Godot;
using XCardGame.Scripts.Common.Constants;
using XCardGame.Scripts.Defs;
using XCardGame.Scripts.Defs.Def.Card;
using XCardGame.Scripts.Effects;
using XCardGame.Scripts.GameLogic;
using XCardGame.Scripts.Nodes;

namespace XCardGame.Scripts.Cards.AbilityCards;


public class KeepOutCard: BaseTapCard
{
    
    public abstract class BaseKeepOutCardRule
    {
        public string Description;
        public abstract bool Filter(BaseCard card);
        
        protected BaseKeepOutCardRule(string description)
        {
            Description = description;
        }
    }
    
    public class FilterBySuitRule: BaseKeepOutCardRule
    {
        public Enums.CardSuit Suit;
        
        public FilterBySuitRule(Enums.CardSuit suit, string description): base(description)
        {
            Suit = suit;
            Description = $"Cards with suit {Suit}.";
        }

        public override bool Filter(BaseCard card)
        {
            return card.Suit.Value == Suit;
        }
        
    }
    
    public class FilterByRankRule: BaseKeepOutCardRule
    {
        public HashSet<Enums.CardRank> Ranks;
        public FilterByRankRule(HashSet<Enums.CardRank> ranks, string description): base(description)
        {
            Ranks = ranks;
        }

        public override bool Filter(BaseCard card)
        {
            return Ranks.Contains(card.Rank.Value);
        }
    }
    
    public class KeepOutEffect: BaseFieldEffect
    {
        public List<CardNode> NegatedCardNodes;
        public KeepOutEffect(string name, string description, BaseCard originateCard) : base(name, description, originateCard)
        {
            NegatedCardNodes = new List<CardNode>();
        }

        public override void OnStart(Battle battle)
        {
            base.OnStart(battle);
            var keepOutCard = (KeepOutCard)OriginateCard;
            foreach (var container in keepOutCard.CardContainers)
            {
                foreach (var cardNode in container.ContentNodes)
                {
                    if (keepOutCard.Rule.Filter(cardNode.Content.Value))
                    {
                        cardNode.TweenNegate(true, Configuration.NegateTweenTime);
                        NegatedCardNodes.Add(cardNode);
                    }
                }
            }
            battle.OnDealCard += OnDealCard;
        }
        
        public override void OnStop(Battle battle)
        {
            foreach (var cardNode in NegatedCardNodes)
            {
                if (GodotObject.IsInstanceValid(cardNode))
                {
                    cardNode.TweenNegate(false, Configuration.NegateTweenTime);
                }
            }
            NegatedCardNodes.Clear();
            battle.OnDealCard -= OnDealCard;
        }

        protected void OnDealCard(Battle battle, CardNode node)
        {
            var keepOutCard = (KeepOutCard)OriginateCard;
            var targetCard = node.Content.Value;
            if (keepOutCard.Rule.Filter(targetCard))
            {
                targetCard.IsNegated.Value = true;
            }
        }
    }

    public BaseKeepOutCardRule Rule;
    public List<CardContainer> CardContainers;
    public KeepOutEffect Effect;
    
    public KeepOutCard(InteractCardDef def): base(def)
    {
        Rule = null;
    }
    
    public override void Setup(Dictionary<string, object> args)
    {
        base.Setup(args);
        CardContainers = new List<CardContainer>
        {
            Battle.CommunityCardContainer,
            Battle.Player.HoleCardContainer,
            Battle.Enemy.HoleCardContainer
        };
        Effect = new KeepOutEffect(Def.Name, GetDescription(), this);
        Effect.Setup(args);
    }

    public override void OnStart(Battle battle)
    {
        base.OnStart(battle);
        if (IsFunctioning() && !AlreadyFunctioning)
        {
            Battle.StartEffect(Effect);
            AlreadyFunctioning = true;
        }
    }

    public override void OnStop(Battle battle)
    {
        base.OnStop(battle);
        if (AlreadyFunctioning)
        {
            Battle.StopEffect(Effect);
            AlreadyFunctioning = false;
        }
    }
}