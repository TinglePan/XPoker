using System.Collections.Generic;
using XCardGame.Scripts.Common.Constants;
using XCardGame.Scripts.GameLogic;
using XCardGame.Scripts.Ui;

namespace XCardGame.Scripts.Cards.AbilityCards;

public class MillenniumEyeCard: BaseActivatableCard
{
    private List<CardContainer> _cardContainers;
    
    public MillenniumEyeCard(Enums.CardFace face, Enums.CardSuit suit, Enums.CardRank rank, int cost = 1,
        int coolDown = 2, bool isQuick = false, BattleEntity owner = null) : 
        base("Millennium Eye", "I can see forever.", "res://Sprites/Cards/millennium_eye.png", 
            face, suit, rank, cost, coolDown, isQuick, owner)
    {
    }
    
    public override void Setup(Dictionary<string, object> args)
    {
        base.Setup(args);
        _cardContainers = GameMgr.UiMgr.GetNodes<CardContainer>("pokerCardContainer");
    }
    
    
    public override void Activate()
    {
        float delay = 0f;
        foreach (var cardContainer in _cardContainers)
        {
            foreach (var card in cardContainer.Cards)
            {
                if (card.Face.Value == Enums.CardFace.Up) continue;
                delay += Configuration.RevealDelayPerCard;
                card.Node.Reveal(Configuration.RevealDuration, delay);
            }
        }
        AfterEffect();
    }
}