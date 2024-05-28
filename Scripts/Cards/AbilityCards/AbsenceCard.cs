using System;
using System.Collections.Generic;
using XCardGame.Scripts.Common.Constants;
using XCardGame.Scripts.Effects;
using XCardGame.Scripts.GameLogic;
using XCardGame.Scripts.Nodes;
using CardContainer = XCardGame.Scripts.Nodes.CardContainer;

namespace XCardGame.Scripts.Cards.AbilityCards;


public class AbsenceCard: BaseTapCard
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
        public List<BaseCard> NegatedCards;
        public AbsenceEffect(string name, string description, string iconPath, BaseCard createdByCard) : base(name, description, iconPath, createdByCard)
        {
            NegatedCards = new List<BaseCard>();
        }

        public override void OnStart(Battle battle)
        {
            base.OnStart(battle);
            if (CreatedByCard is AbsenceCard absenceCard)
            {
                foreach (var container in absenceCard.CardContainers)
                {
                    foreach (var card in container.Contents)
                    {
                        if (absenceCard.Rule.Filter(card))
                        {
                            card.Node.TweenNegate(true, Configuration.NegateTweenTime);
                            NegatedCards.Add(card);
                        }
                    }
                }
            }
        }
        
        public override void OnStop(Battle battle)
        {
            foreach (var card in NegatedCards)
            {
                card.Node.TweenNegate(false, Configuration.NegateTweenTime);
            }
            NegatedCards.Clear();
        }
    }

    public BaseAbsenceCardRule Rule;
    public List<CardContainer> CardContainers;
    
    public AbsenceCard(Enums.CardSuit suit, Enums.CardRank rank, int tapCost, int unTapCost, BaseAbsenceCardRule rule): 
        base("Absence", "Certain cards do not count, the rule alters each round.", "res://Sprites/Cards/absence.png", suit, rank, tapCost, unTapCost)
    {
        Rule = rule;
    }
    
    public override void Setup(Dictionary<string, object> args)
    {
        base.Setup(args);
        CardContainers = GameMgr.SceneMgr.GetNodes<CardContainer>("markerCardContainer"); 
        Effect = new AbsenceEffect(Name, Description, IconPath.Value, this);
    }
}