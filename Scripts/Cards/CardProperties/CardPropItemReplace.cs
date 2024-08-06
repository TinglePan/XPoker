using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using XCardGame.Ui;

namespace XCardGame.CardProperties;

public class CardPropItemReplace: CardPropItem
{
    public CardPropItemReplace(BaseCard card) : base(card)
    {
        Debug.Assert(RankChangePerUse == 0, "Replace card should not have RankChangePerUse other than 0");
    }
    
    public override async Task Effect(List<CardNode> targets)
    {
        var fromNode = CardNode;
        var toNode = targets[0];
        var fromContainer = (CardContainer)fromNode.CurrentContainer.Value;
        var toContainer = (CardContainer)toNode.CurrentContainer.Value;
        var toIndex = toContainer.ContentNodes.IndexOf(toNode);
        var tasks = new List<Task>
        {
            base.Effect(targets),
            toNode.AnimateLeaveField(),
            fromContainer.MoveCardNodeToContainer(fromNode, toContainer, toIndex)
        };
        await Task.WhenAll(tasks);
    }
    
    public virtual List<CardNode> GetValidSelectTargets()
    {
        return Card.Battle.CommunityCardContainer.CardNodes;
    }
    
    protected override BaseCardReplaceInputHandler GetInputHandler()
    {
        return new BaseCardReplaceInputHandler(GameMgr, CardNode);
    }
}