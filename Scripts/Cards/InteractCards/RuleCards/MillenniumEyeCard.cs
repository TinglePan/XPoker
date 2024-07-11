using System.Collections.Generic;
using Godot;
using XCardGame.Scripts.Common.Constants;
using XCardGame.Scripts.Defs.Def.Card;
using XCardGame.Scripts.Game;
using XCardGame.Scripts.Ui;

namespace XCardGame.Scripts.Cards.InteractCards.RuleCards;

public class MillenniumEyeCard: BaseRuleCard
{
    public List<CardContainer> CardContainers;
    public List<CardNode> RevealedCardNodes;
    
    public MillenniumEyeCard(InteractCardDef def): base(def)
    {
        RevealedCardNodes = new List<CardNode>();
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
    }

    public override void OnStart(Battle battle)
    {
        base.OnStart(battle);
        if (IsFunctioning() && !AlreadyFunctioning)
        {
            foreach (var container in CardContainers)
            {
                foreach (var cardNode in container.ContentNodes)
                {
                    if (cardNode.FaceDirection.Value == Enums.CardFace.Down && !cardNode.IsRevealed.Value)
                    {
                        cardNode.TweenReveal(true, Configuration.RevealTweenTime);
                        RevealedCardNodes.Add(cardNode);
                    }
                }
            }
            battle.Dealer.DealCardPile.TopCard.TweenReveal(true, Configuration.RevealTweenTime);
            RevealedCardNodes.Add(battle.Dealer.DealCardPile.TopCard);
            battle.OnDealCard += OnDealCard;
            AlreadyFunctioning = true;
        }
    }

    public override void OnStop(Battle battle)
    {
        base.OnStop(battle);
        if (AlreadyFunctioning)
        {
            foreach (var cardNode in RevealedCardNodes)
            {
                if (GodotObject.IsInstanceValid(cardNode))
                {
                    cardNode.TweenReveal(false, Configuration.RevealTweenTime);
                }
            }
            battle.OnDealCard -= OnDealCard;
            AlreadyFunctioning = false;
        }
    }
    
    protected void OnDealCard(Battle battle, CardNode node)
    {
        node.IsRevealed.Value = true;
    }
}