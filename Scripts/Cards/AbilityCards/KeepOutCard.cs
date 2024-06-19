using System;
using System.Collections.Generic;
using XCardGame.Scripts.Common.Constants;
using XCardGame.Scripts.Defs;
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
    
    public class KeepOutEffect: BaseSingleTurnEffect
    {
        public List<CardNode> NegatedCardNodes;
        public KeepOutEffect(string name, string description, Battle battle, BaseCard createdByCard) : base(name, description, battle, createdByCard)
        {
            NegatedCardNodes = new List<CardNode>();
        }

        public override void OnStart(Battle battle)
        {
            base.OnStart(battle);
            if (CreatedByCard is KeepOutCard absenceCard)
            {
                foreach (var container in absenceCard.CardContainers)
                {
                    foreach (var cardNode in container.ContentNodes)
                    {
                        if (absenceCard.Rule.Filter(cardNode.Content.Value))
                        {
                            cardNode.TweenNegate(true, Configuration.NegateTweenTime);
                            NegatedCardNodes.Add(cardNode);
                        }
                    }
                }
            }
        }
        
        public override void OnStop(Battle battle)
        {
            foreach (var cardNode in NegatedCardNodes)
            {
                cardNode.TweenNegate(false, Configuration.NegateTweenTime);
            }
            NegatedCardNodes.Clear();
        }
    }

    public BaseKeepOutCardRule Rule;
    public List<CardContainer> CardContainers;
    
    public KeepOutCard(TapCardDef def, BaseKeepOutCardRule rule): base(def)
    {
        Rule = rule;
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
        Effect = new KeepOutEffect(Def.Name, GetDescription(), Battle, this);
    }
}