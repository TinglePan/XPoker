using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using XCardGame.Scripts.Cards.CardInputHandlers;
using XCardGame.Scripts.Common.Constants;
using XCardGame.Scripts.Defs.Def.Card;
using XCardGame.Scripts.Ui;

namespace XCardGame.Scripts.Cards.InteractCards;

public abstract class BaseTokenCard<TCard, TInputHandler>: BaseItemCard where TInputHandler: BaseItemCardSelectTargetInputHandler<TCard> where TCard: BaseTokenCard<TCard, TInputHandler>
{
    public class BaseTokenCardInputHandler : BaseItemCardSelectTargetInputHandler<TCard>
    {
        public BaseTokenCardInputHandler(GameMgr gameMgr, CardNode originate, int selectTargetCountLimit = 1) : base(gameMgr, originate, selectTargetCountLimit)
        {
        }

        protected override IEnumerable<CardNode> GetValidSelectTargets()
        {
            foreach (var cardContainer in OriginateCard.ValidTargetContainers)
            {
                foreach (var node in cardContainer.ContentNodes)
                {
                    yield return node;
                }
            }
        }
        
        protected override async void Confirm()
        {
            if (SelectedNodes.Count == 1)
            {
                var fromNode = OriginateCardNode;
                var toNode = SelectedNodes[0];
                var fromContainer = (CardContainer)fromNode.Container.Value;
                var toContainer = (CardContainer)toNode.Container.Value;
                var toIndex = toContainer.ContentNodes.IndexOf(toNode);
                var tasks = new List<Task>();

                if (toNode.PreviousContainer == null)
                {
                    tasks.Add(Battle.Dealer.AnimateDiscard(toNode));
                }
                else
                {
                    tasks.Add(toContainer.MoveCardNodeToContainer(toNode, (CardContainer)toNode.PreviousContainer));
                }
                toNode.IsSelected = false;
                tasks.Add(fromContainer.MoveCardNodeToContainer(fromNode, toContainer, toIndex));
                fromNode.IsSelected = false;
                SelectedNodes.Clear();
                OriginateCard.Use(OriginateCardNode);
                GameMgr.InputMgr.QuitCurrentInputHandler();
                await Task.WhenAll(tasks);
            }
            else
            {
                // TODO: Hint on invalid confirm
            }
        }
    }
    
    public List<CardContainer> ValidTargetContainers;
    
    public BaseTokenCard(InteractCardDef def) : base(def)
    {
        Debug.Assert(def.RankChangePerUse == 0);
    }
    
    public override void Setup(Dictionary<string, object> args)
    {
        base.Setup(args);
        ValidTargetContainers = new List<CardContainer>
        {
            Battle.CommunityCardContainer,
        };
    }

    public override void ChooseTargets(CardNode node)
    {
        var inputHandler = GetInputHandler();
        GameMgr.InputMgr.SwitchToInputHandler(inputHandler);
    }

    protected abstract TInputHandler GetInputHandler();
}