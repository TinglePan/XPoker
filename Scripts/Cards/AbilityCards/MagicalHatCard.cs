using System;
using System.Collections.Generic;
using Godot;
using XCardGame.Scripts.Common.Constants;
using XCardGame.Scripts.Defs;
using XCardGame.Scripts.GameLogic;
using XCardGame.Scripts.InputHandling;
using XCardGame.Scripts.Nodes;
using CardContainer = XCardGame.Scripts.Nodes.CardContainer;
using CardNode = XCardGame.Scripts.Nodes.CardNode;

namespace XCardGame.Scripts.Cards.AbilityCards;

public class MagicalHatCard: BaseUseCard
{
    public class MagicalHatCardInputHandler : BaseInteractCardInputHandler<MagicalHatCard>
    {
        public MagicalHatCardInputHandler(GameMgr gameMgr, Battle battle, MagicalHatCard card) : base(gameMgr, battle, card)
        {
        }

        protected override IEnumerable<CardNode> GetValidSelectTargets()
        {
            foreach (var cardContainer in Card.CardContainers)
            {
                foreach (var node in cardContainer.ContentNodes)
                {
                    yield return node;
                }
            }
        }

        protected override void Confirm()
        {
            if (SelectedCardNodes.Count == 2)
            {
                var fromNode = SelectedCardNodes[0];
                var toNode = SelectedCardNodes[1];
                var fromContainer = fromNode.Container;
                var toContainer = toNode.Container;
                var fromIndex = fromContainer.ContentNodes.IndexOf(fromNode);
                var toIndex = toContainer.ContentNodes.IndexOf(toNode);
                (toContainer.ContentNodes[toIndex], fromContainer.ContentNodes[fromIndex]) = (fromContainer.ContentNodes[fromIndex], toContainer.ContentNodes[toIndex]);
                fromNode.IsSelected = false;
                toNode.IsSelected = false;
                SelectedCardNodes.Clear();
                Card.Use();
                GameMgr.InputMgr.QuitCurrentInputHandler();
            }
            else
            {
                // TODO: Hint on invalid confirm
            }
        }
    }
    
    public List<CardContainer> CardContainers;
    
    public MagicalHatCard(UseCardDef def) : base(def)
    {
        Def.Name = "Magical hat";
        Def.DescriptionTemplate = "Swap two cards in hole card area or community card area.";
        Def.IconPath = "res://Sprites/Cards/magical_hat.png";
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
    
    public override void ChooseTargets()
    {
        var inputHandler = new MagicalHatCardInputHandler(GameMgr, Battle, this);
        GameMgr.InputMgr.SwitchToInputHandler(inputHandler);
    }
}