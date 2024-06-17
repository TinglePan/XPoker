using System.Collections.Generic;
using System.Linq;
using Godot;
using XCardGame.Scripts.Buffs;
using XCardGame.Scripts.Common.Constants;
using XCardGame.Scripts.Defs;
using XCardGame.Scripts.Effects;
using XCardGame.Scripts.GameLogic;
using XCardGame.Scripts.HandEvaluate;
using XCardGame.Scripts.Nodes;

namespace XCardGame.Scripts.Cards.AbilityCards;

public class TurnTheTablesCard: BaseUseCard
{
    public CardContainer PlayerHoleCardContainer;
    public CardContainer EnemyHoleCardContainer;
    
    public TurnTheTablesCard(UseCardDef def) : base(def)
    {
    }

    public override void Setup(Dictionary<string, object> args)
    {
        base.Setup(args);
        PlayerHoleCardContainer = Battle.Player.HoleCardContainer;
        EnemyHoleCardContainer = Battle.Enemy.HoleCardContainer;
    }
    
    
    public override bool CanInteract()
    {
        return base.CanInteract() && Battle.CurrentState == Battle.State.BeforeShowDown;
    }

    public override void Use()
    {
        base.Use();
        var playerHoleCardNodes = new List<CardNode>(PlayerHoleCardContainer.ContentNodes);
        var enemyHoleCardNodes = new List<CardNode>(EnemyHoleCardContainer.ContentNodes);
        foreach (var cardNode in playerHoleCardNodes)
        {
            PlayerHoleCardContainer.ContentNodes.Remove(cardNode);
            EnemyHoleCardContainer.ContentNodes.Add(cardNode);
        }
        foreach (var cardNode in enemyHoleCardNodes)
        {
            EnemyHoleCardContainer.ContentNodes.Remove(cardNode);
            PlayerHoleCardContainer.ContentNodes.Add(cardNode);
        }
    }
}