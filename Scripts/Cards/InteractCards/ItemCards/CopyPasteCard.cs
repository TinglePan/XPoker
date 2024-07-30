using System.Collections.Generic;
using System.Threading.Tasks;
using XCardGame.Common;
using XCardGame.Ui;

namespace XCardGame;

public class CopyPasteCard: BaseItemCard
{
    public class CopyPasteCardInputHandler : BaseItemCardSelectTargetInputHandler<CopyPasteCard>
    {
        public CopyPasteCardInputHandler(GameMgr gameMgr, CardNode node) : base(gameMgr, node, 1)
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
            var tasks = new List<Task>();
            if (SelectedNodes.Count == 1)
            {
                var selectedNode = SelectedNodes[0];
                var copiedCard = new CopyCard(CardDefs.Copy, selectedNode.Card);
                tasks.Add(GameMgr.AwaitAndDisableInput(Battle.Dealer.CreateCardAndPutInto(copiedCard, selectedNode, Enums.CardFace.Up, Battle.ItemCardContainer)));
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
    
    public CopyPasteCard(ItemCardDef def) : base(def)
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
        return base.CanInteract(node) && Battle.CurrentState.Value == Battle.State.BeforeShowDown;
    }

    public override void ChooseTargets(CardNode node)
    {
        var inputHandler = new CopyPasteCardInputHandler(GameMgr, node);
        GameMgr.InputMgr.SwitchToInputHandler(inputHandler);
    }
}