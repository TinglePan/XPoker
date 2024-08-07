using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot;
using XCardGame.CardProperties;
using XCardGame.Common;
using XCardGame.Ui;

namespace XCardGame;


public class KeepOutCard: BaseCard
{
    public class KeepOutEffect: BaseFieldEffect
    {
        public HashSet<Enums.CardRank> KeepOutRanks;
        public HashSet<Enums.CardSuit> KeepOutSuits;
        public KeepOutEffect(string name, string description, BaseCard originateCard, List<BaseCard> piledCards) : base(name, description, originateCard)
        {
            KeepOutRanks = new HashSet<Enums.CardRank>();
            KeepOutSuits = new HashSet<Enums.CardSuit>();
            foreach (var card in piledCards)
            {
                KeepOutRanks.Add(card.Rank.Value);
                KeepOutSuits.Add(card.Suit.Value);
            }
        }
    }
    
    public class KeepOutCardRuleProp: CardPropRuleRoundReTrigger
    {
        public PiledCardNode PiledCardNode;
        public KeepOutEffect FieldEffect;
        
        public KeepOutCardRuleProp(BaseCard card): base(card)
        {
            PiledCardNode = (PiledCardNode)CardNode;
        }
        
        public override bool CanUse()
        {
            if (!base.CanUse()) return false;
            return Battle.CurrentState.Value == Battle.State.BeforeShowDown;
        }
        
        public override void OnStartEffect()
        {
            if (!IsEffectActive)
            {
                FieldEffect = new KeepOutEffect(Card.Def.Name, Card.Description(), Card, PiledCardNode.CardPile.Cards.ToList());
                Battle.FieldEffects.Add(FieldEffect);
                IsEffectActive = true;
            }
        }
        
        public override void OnStopEffect()
        {
            if (IsEffectActive && Battle.FieldEffects.Contains(FieldEffect))
            {
                Battle.FieldEffects.Remove(FieldEffect);
                IsEffectActive = false;
            }
        }
    }
    
    public KeepOutCard(CardDef def): base(def)
    {
    }
    
    protected override CardPropRule CreateRuleProp()
    {
        return new KeepOutCardRuleProp(this);
    }
}