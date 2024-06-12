using System;
using System.Collections.Generic;
using XCardGame.Scripts.Common.Constants;
using XCardGame.Scripts.Effects;
using XCardGame.Scripts.GameLogic;
using XCardGame.Scripts.Nodes;
using CardContainer = XCardGame.Scripts.Nodes.CardContainer;

namespace XCardGame.Scripts.Cards.AbilityCards;


public class BlockOutCard: BaseTapCard
{
    
    public abstract class BaseAbsenceCardRule
    {
        public string Description;
        public abstract bool Filter(BaseCard card);
        
        protected BaseAbsenceCardRule(string description)
        {
            Description = description;
        }
    }
    
    public class FilterBySuitRule: BaseAbsenceCardRule
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
    
    public class FilterByRankRule: BaseAbsenceCardRule
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
    
    public class AbsenceEffect: BaseSingleTurnEffect
    {
        public List<CardNode> NegatedCardNodes;
        public AbsenceEffect(string name, string description, BaseCard createdByCard) : base(name, description, createdByCard)
        {
            NegatedCardNodes = new List<CardNode>();
        }

        public override void OnStart(Battle battle)
        {
            base.OnStart(battle);
            if (CreatedByCard is BlockOutCard absenceCard)
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

    public BaseAbsenceCardRule Rule;
    public List<CardContainer> CardContainers;
    
    public BlockOutCard(Enums.CardSuit suit, Enums.CardRank rank, int tappedCost, int unTappedCost, BaseAbsenceCardRule rule): 
        base("Absence", "Certain cards do not count, the rule alters each round.", "res://Sprites/Cards/absence.png", suit, rank, tappedCost, unTappedCost)
    {
        Rule = rule;
    }
    
    public override void Setup(Dictionary<string, object> args)
    {
        base.Setup(args);
        CardContainers = GameMgr.SceneMgr.GetNodes<CardContainer>("markerCardContainer"); 
        Effect = new AbsenceEffect(Name, Description, this);
    }
}