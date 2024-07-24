using System.Collections.Generic;
using System.Threading.Tasks;
using Godot;
using XCardGame.Common;
using XCardGame.Ui;

namespace XCardGame;


public class KeepOutCard: BaseRuleCard
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

        public override async void OnStartEffect(Battle battle)
        {
            base.OnStartEffect(battle);
            var tasks = new List<Task>();
            var keepOutCard = (KeepOutCard)OriginateCard;
            foreach (var container in keepOutCard.CardContainers)
            {
                foreach (var cardNode in container.CardNodes)
                {
                    if (keepOutCard.Rule.Filter(cardNode.Card))
                    {
                        tasks.Add(cardNode.AnimateNegate(true, Configuration.NegateTweenTime));
                        NegatedCardNodes.Add(cardNode);
                    }
                }
            }
            battle.OnDealCard += OnDealCard;
            await Task.WhenAll(tasks);
        }
        
        public override async void OnStopEffect(Battle battle)
        {
            var tasks = new List<Task>();
            foreach (var cardNode in NegatedCardNodes)
            {
                if (GodotObject.IsInstanceValid(cardNode))
                {
                    tasks.Add(cardNode.AnimateNegate(false, Configuration.NegateTweenTime));
                }
            }
            NegatedCardNodes.Clear();
            battle.OnDealCard -= OnDealCard;
            await Task.WhenAll(tasks);
        }

        protected void OnDealCard(Battle battle, CardNode node)
        {
            var keepOutCard = (KeepOutCard)OriginateCard;
            var targetCard = node.Card;
            if (keepOutCard.Rule.Filter(targetCard))
            {
                targetCard.IsNegated.Value = true;
            }
        }
    }

    public BaseKeepOutCardRule Rule;
    public List<CardContainer> CardContainers;
    public KeepOutEffect Effect;
    
    public KeepOutCard(RuleCardDef def): base(def)
    {
        Rule = null;
    }
    
    public override void Setup(object o)
    {
        base.Setup(o);
        var args = (SetupArgs)o;
        CardContainers = new List<CardContainer>
        {
            Battle.CommunityCardContainer,
            Battle.Player.HoleCardContainer,
            Battle.Enemy.HoleCardContainer
        };
        Effect = new KeepOutEffect(Def.Name, Description(), this);
        Effect.Setup(new BaseEffect.SetupArgs
        {
            GameMgr = args.GameMgr,
            Battle = args.Battle
        });
    }

    public override void OnStartEffect(Battle battle)
    {
        base.OnStartEffect(battle);
        if (IsFunctioning() && !AlreadyFunctioning)
        {
            Battle.StartEffect(Effect);
            AlreadyFunctioning = true;
        }
    }

    public override void OnStopEffect(Battle battle)
    {
        base.OnStopEffect(battle);
        if (AlreadyFunctioning)
        {
            Battle.StopEffect(Effect);
            AlreadyFunctioning = false;
        }
    }
}