using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using XCardGame.Scripts.Cards.CardInputHandlers;
using XCardGame.Scripts.Defs.Def.Card;
using XCardGame.Scripts.Ui;

namespace XCardGame.Scripts.Cards.InteractCards.ItemCards;

public class BaseTokenCardInputHandler<TCard> : BaseItemCardSelectTargetInputHandler<TCard> where TCard : BaseTokenCard<TCard, BaseTokenCardInputHandler<TCard>>
{
    public BaseTokenCardInputHandler(GameMgr gameMgr, CardNode originate, int selectTargetCountLimit = 1) : base(gameMgr, originate, selectTargetCountLimit)
    {
    }

    protected override IEnumerable<CardNode> GetValidSelectTargets()
    {
        return OriginateCard.GetValidTargetSelectTargets();
    }
    
    protected override async void Confirm()
    {
        if (SelectedNodes.Count == 1)
        {
            var fromNode = OriginateCardNode;
            var toNode = SelectedNodes[0];
            var fromContainer = (CardContainer)fromNode.CurrentContainer.Value;
            var toContainer = (CardContainer)toNode.CurrentContainer.Value;
            var toIndex = toContainer.ContentNodes.IndexOf(toNode);
            var tasks = new List<Task>();

            if (toNode.PreviousContainer == null)
            {
                tasks.Add(Battle.Dealer.AnimateDiscard(toNode));
            }
            // else
            // {
            //     tasks.Add(toContainer.MoveCardNodeToContainer(toNode, (CardContainer)toNode.PreviousContainer));
            // }
            tasks.Add(fromContainer.MoveCardNodeToContainer(fromNode, toContainer, toIndex));
            fromNode.IsSelected = false;
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

public abstract class BaseTokenCard<TCard, TInputHandler>: BaseItemCard where TInputHandler: BaseItemCardSelectTargetInputHandler<TCard> where TCard: BaseTokenCard<TCard, TInputHandler>
{
    
    public BaseTokenCard(ItemCardDef def) : base(def)
    {
        Debug.Assert(def.RankChangePerUse == 0);
    }

    public override void ChooseTargets(CardNode node)
    {
        var inputHandler = GetInputHandler();
        GameMgr.InputMgr.SwitchToInputHandler(inputHandler);
    }
    
    public IEnumerable<CardNode> GetValidTargetSelectTargets()
    {
        foreach (var cardContainer in GetValidTargetContainers())
        {
            foreach (var node in cardContainer.CardNodes)
            {
                if (FilterValidCardNodes(node))
                {
                    yield return node;
                }
            }
        }
    }

    protected virtual IEnumerable<CardContainer> GetValidTargetContainers()
    {
        yield return Battle.CommunityCardContainer;
        yield return Battle.Player.HoleCardContainer;
        yield return Battle.Enemy.HoleCardContainer;
    }

    protected virtual bool FilterValidCardNodes(CardNode node)
    {
        return true;
    }

    protected abstract TInputHandler GetInputHandler();
}