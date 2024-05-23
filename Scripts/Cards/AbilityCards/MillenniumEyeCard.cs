using System.Collections.Generic;
using System.Linq;
using XCardGame.Scripts.Common.Constants;
using XCardGame.Scripts.Effects;
using XCardGame.Scripts.GameLogic;
using XCardGame.Scripts.Nodes;
using CardContainer = XCardGame.Scripts.Nodes.CardContainer;

namespace XCardGame.Scripts.Cards.AbilityCards;

public class MillenniumEyeCard: BaseUseCard
{
    class MillenniumEyeEffect : BaseSingleTurnEffect
    {
        public List<BaseCard> RevealedCards;
        public MillenniumEyeEffect(string name, string description, string iconPath, BaseCard createdByCard) : base(name, description, iconPath, createdByCard)
        {
            RevealedCards = new List<BaseCard>();
        }

        public override void OnStart(Battle battle)
        {
            base.OnStart(battle);
            
            if (CreatedByCard is MillenniumEyeCard millenniumEyeCard)
            {
                foreach (var container in millenniumEyeCard.CardContainers)
                {
                    foreach (var card in container.Contents)
                    {
                        if (card.Node.FaceDirection == Enums.CardFace.Down && !card.Node.IsRevealed)
                        {
                            card.Node.TweenReveal(true, Configuration.RevealTweenTime);
                            RevealedCards.Add(card);
                        }
                    }
                }
            }
        }

        public override void OnStop(Battle battle)
        {
            base.OnStop(battle);
            foreach (var card in RevealedCards)
            {
                card.Node.TweenReveal(false, Configuration.RevealTweenTime);
            }
        }
    }
    
    public List<CardContainer> CardContainers;
    
    public MillenniumEyeCard(Enums.CardSuit suit, Enums.CardRank rank) : 
        base("Millennium Eye", "All knowing at the cost of all power.", "res://Sprites/Cards/millennium_eye.png", 
            suit, rank, 0, 0)
    {
    }
    
    public override void Setup(Dictionary<string, object> args)
    {
        base.Setup(args);
        CardContainers = GameMgr.UiMgr.GetNodes<CardContainer>("pokerCardContainer");
    }

    public override void Use()
    {
        var effect = new MillenniumEyeEffect(Name, Description, IconPath.Value, this);
        Battle.StartEffect(effect);
        Battle.Player.Energy.Value -= ActualCost();
        StartRecharge();
    }

    public override int ActualCost()
    {
        return Battle.Player.Energy.Value;
    }
}