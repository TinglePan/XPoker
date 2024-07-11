using System.Collections.Generic;
using XCardGame.Scripts.Cards.CardInputHandlers;
using XCardGame.Scripts.Defs.Def.Card;
using XCardGame.Scripts.Game;
using XCardGame.Scripts.InputHandling;
using XCardGame.Scripts.Ui;
using CardNode = XCardGame.Scripts.Ui.CardNode;

namespace XCardGame.Scripts.Cards.InteractCards.ItemCards;

public class MagicalHatCard: BaseItemCard
{
    public class MagicalHatCardInputHandler : BaseItemCardSelectTargetInputHandler<MagicalHatCard>
    {
        public MagicalHatCardInputHandler(GameMgr gameMgr, CardNode node) : base(gameMgr, node, 2)
        {
        }

        protected override IEnumerable<CardNode> GetValidSelectTargets()
        {
            foreach (var cardContainer in OriginateCard.CardContainers)
            {
                foreach (var node in cardContainer.ContentNodes)
                {
                    yield return node;
                }
            }
        }

        protected override void Confirm()
        {
            if (SelectedNodes.Count == 2)
            {
                var fromNode = SelectedNodes[0];
                var toNode = SelectedNodes[1];
                var fromContainer = fromNode.Container.Value;
                var toContainer = toNode.Container.Value;
                var fromIndex = fromContainer.ContentNodes.IndexOf(fromNode);
                var toIndex = toContainer.ContentNodes.IndexOf(toNode);
                (toContainer.ContentNodes[toIndex], fromContainer.ContentNodes[fromIndex]) = (fromContainer.ContentNodes[fromIndex], toContainer.ContentNodes[toIndex]);
                fromNode.IsSelected = false;
                toNode.IsSelected = false;
                SelectedNodes.Clear();
                OriginateCard.Use(OriginateCardNode);
                GameMgr.InputMgr.QuitCurrentInputHandler();
            }
            else
            {
                // TODO: Hint on invalid confirm
            }
        }
    }
    
    public List<CardContainer> CardContainers;

    public MagicalHatCard(ItemCardDef def) : base(def)
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

    public override bool CanInteract(CardNode node)
    {
        return base.CanInteract(node) && Battle.CurrentState == Battle.State.BeforeShowDown;
    }
    
    public override void ChooseTargets(CardNode node)
    {
        var inputHandler = new MagicalHatCardInputHandler(GameMgr, node);
        GameMgr.InputMgr.SwitchToInputHandler(inputHandler);
    }
}