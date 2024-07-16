using System.Collections.Generic;
using System.Threading.Tasks;
using Godot;
using XCardGame.Scripts.Common.Constants;
using XCardGame.Scripts.Defs.Def.Card;
using XCardGame.Scripts.Effects;
using XCardGame.Scripts.Effects.FieldEffects;
using XCardGame.Scripts.Game;
using XCardGame.Scripts.Ui;

namespace XCardGame.Scripts.Cards.InteractCards.RuleCards;


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

        public override async void OnStart(Battle battle)
        {
            base.OnStart(battle);
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
        
        public override async void OnStop(Battle battle)
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
    
    public override void Setup(SetupArgs args)
    {
        base.Setup(args);
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