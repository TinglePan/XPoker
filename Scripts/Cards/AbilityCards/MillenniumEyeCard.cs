using System.Collections.Generic;
using System.Linq;
using XCardGame.Scripts.Common.Constants;
using XCardGame.Scripts.Defs;
using XCardGame.Scripts.Effects;
using XCardGame.Scripts.GameLogic;
using XCardGame.Scripts.Nodes;
using CardContainer = XCardGame.Scripts.Nodes.CardContainer;

namespace XCardGame.Scripts.Cards.AbilityCards;

public class MillenniumEyeCard: BaseUseCard
{
    class MillenniumEyeEffect : BaseSingleTurnEffect
    {
        public List<CardNode> RevealedCardNodes;
        public MillenniumEyeEffect(string name, string description, Battle battle, BaseCard createdByCard) : base(name, description, battle, createdByCard)
        {
            RevealedCardNodes = new List<CardNode>();
        }

        public override void OnStart(Battle battle)
        {
            base.OnStart(battle);
            
            if (CreatedByCard is MillenniumEyeCard millenniumEyeCard)
            {
                foreach (var container in millenniumEyeCard.CardContainers)
                {
                    foreach (var cardNode in container.ContentNodes)
                    {
                        if (cardNode.FaceDirection.Value == Enums.CardFace.Down && !cardNode.IsRevealed)
                        {
                            cardNode.TweenReveal(true, Configuration.RevealTweenTime);
                            RevealedCardNodes.Add(cardNode);
                        }
                    }
                }
            }
        }

        public override void OnStop(Battle battle)
        {
            base.OnStop(battle);
            foreach (var cardNode in RevealedCardNodes)
            {
                cardNode.TweenReveal(false, Configuration.RevealTweenTime);
            }
        }
    }
    
    public List<CardContainer> CardContainers;
    
    public MillenniumEyeCard(UseCardDef def): base(def)
    {
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

    public override bool CanInteract()
    {
        return base.CanInteract() && Battle.CurrentState == Battle.State.BeforeShowDown;
    }

    public override void Use()
    {
        var effect = new MillenniumEyeEffect(Def.Name, GetDescription(), Battle, this);
        Battle.StartEffect(effect);
        StartRecharge();
    }
}