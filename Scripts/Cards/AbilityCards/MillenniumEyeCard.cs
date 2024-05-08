using System.Collections.Generic;
using XCardGame.Scripts.Common.Constants;
using XCardGame.Scripts.GameLogic;
using XCardGame.Scripts.Ui;

namespace XCardGame.Scripts.Cards.AbilityCards;

public class MillenniumEyeCard: BaseActiveAbilityCard
{
    private List<CardContainer> _cardContainers;
    
    public MillenniumEyeCard(GameMgr gameMgr, Enums.CardFace face, BattleEntity owner, string iconPath, int cost, int coolDown, bool isQuickAbility = false) : base(gameMgr, "Millennium Eye", "I can see forever.", face, owner, iconPath, cost, coolDown, isQuickAbility)
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
        AfterEffect?.Invoke();
    }
}