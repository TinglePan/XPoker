using System.Collections.Generic;
using System.Threading.Tasks;
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
            foreach (var cardContainer in OriginateCard.ValidTargetContainers)
            {
                foreach (var node in cardContainer.CardNodes)
                {
                    yield return node;
                }
            }
        }

        protected override async void Confirm()
        {
            if (SelectedNodes.Count == 2)
            {
                var tasks = new List<Task>();
                var fromNode = SelectedNodes[0];
                var toNode = SelectedNodes[1];
                var fromContainer = (CardContainer)fromNode.CurrentContainer.Value;
                var toContainer = (CardContainer)toNode.CurrentContainer.Value;
                var fromIndex = fromContainer.ContentNodes.IndexOf(fromNode);
                var toIndex = toContainer.ContentNodes.IndexOf(toNode);
                tasks.Add(fromContainer.MoveCardNodeToContainer(fromNode, toContainer, toIndex));
                tasks.Add(toContainer.MoveCardNodeToContainer(toNode, fromContainer, fromIndex));
                OriginateCard.Use(OriginateCardNode);
                GameMgr.InputMgr.QuitCurrentInputHandler();
                SelectedNodes.Clear();
                await Task.WhenAll(tasks);
            }
            else
            {
                // TODO: Hint on invalid confirm
            }
        }
    }
    
    public List<CardContainer> ValidTargetContainers;

    public MagicalHatCard(ItemCardDef def) : base(def)
    {
    }
    
    public override void Setup(object o)
    {
        base.Setup(o);
        ValidTargetContainers = new List<CardContainer>
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