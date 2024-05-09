using System.Collections.Generic;
using XCardGame.Scripts.Common.Constants;
using XCardGame.Scripts.GameLogic;
using XCardGame.Scripts.Ui;

namespace XCardGame.Scripts.Cards.AbilityCards;

public class MillenniumEyeCard: BaseActivatableAbilityCard
{
    private List<CardContainer> _cardContainers;
    
    public MillenniumEyeCard(GameMgr gameMgr, Enums.CardFace face, Enums.CardSuit suit, BattleEntity owner=null) : 
        base(gameMgr, "Millennium Eye", "I can see forever.", face, suit, 
            "res://Sprites/Cards/millennium_eye.png", 1, 2, false, owner)
    {
        _cardContainers = gameMgr.UiMgr.GetNodes<CardContainer>("pokerCardContainer");
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