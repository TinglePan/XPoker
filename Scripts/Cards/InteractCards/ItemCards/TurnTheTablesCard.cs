using System.Collections.Generic;
using XCardGame.Scripts.Defs.Def.Card;
using XCardGame.Scripts.Game;
using XCardGame.Scripts.Ui;

namespace XCardGame.Scripts.Cards.InteractCards.ItemCards;

public class TurnTheTablesCard: BaseItemCard
{
    public CardContainer PlayerHoleCardContainer;
    public CardContainer EnemyHoleCardContainer;
    
    public TurnTheTablesCard(InteractCardDef def) : base(def)
    {
    }

    public override void Setup(Dictionary<string, object> args)
    {
        base.Setup(args);
        PlayerHoleCardContainer = Battle.Player.HoleCardContainer;
        EnemyHoleCardContainer = Battle.Enemy.HoleCardContainer;
    }
    
    
    public override bool CanInteract(CardNode node)
    {
        return base.CanInteract(node) && Battle.CurrentState == Battle.State.BeforeShowDown;
    }

    public override void Use(CardNode node)
    {
        base.Use(node);
        var playerHoleCardNodes = new List<CardNode>(PlayerHoleCardContainer.ContentNodes);
        var enemyHoleCardNodes = new List<CardNode>(EnemyHoleCardContainer.ContentNodes);
        foreach (var cardNode in playerHoleCardNodes)
        {
            PlayerHoleCardContainer.ContentNodes.Remove(cardNode);
        }
        foreach (var cardNode in enemyHoleCardNodes)
        {
            EnemyHoleCardContainer.ContentNodes.Remove(cardNode);
        }
        foreach (var cardNode in playerHoleCardNodes)
        {
            EnemyHoleCardContainer.ContentNodes.Add(cardNode);
        }
        foreach (var cardNode in enemyHoleCardNodes)
        {
            PlayerHoleCardContainer.ContentNodes.Add(cardNode);
        }
    }
}