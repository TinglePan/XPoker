using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using XCardGame.CardProperties;
using XCardGame.Common;
using XCardGame.Ui;

namespace XCardGame;

public class CopyPasteCard: BaseCard
{
    public class CopyPasteCardInputHandler : BaseCardSelectTargetInputHandlerWithConfirmConstraints
    {
        public CopyPasteCardInputHandler(GameMgr gameMgr, CardNode node) : base(gameMgr, node, 1)
        {
        }

        protected override IEnumerable<CardNode> GetValidSelectTargets()
        {
            return Helper.OriginateCard.GetProp<CopyPasteCardItemProp>().ValidCardContainers.SelectMany(x => x.CardNodes);
        }
    }
    
    public class CopyPasteCardItemProp : CardPropItem
    {
        public List<CardContainer> ValidCardContainers;
        
        public CopyPasteCardItemProp(BaseCard card, List<CardContainer> validContainers) : base(card)
        {
            ValidCardContainers = validContainers;
        }
        
        public override bool CanUse()
        {
            if (!base.CanUse()) return false;
            return Battle.CurrentState.Value == Battle.State.BeforeShowDown;
        }

        public override async Task Effect(List<CardNode> targets)
        {
            var tasks = new List<Task>();
            tasks.Add(base.Effect(targets));
            var selectedNode = targets[0];
            var copiedCard = new CopyCard(CardDefs.Copy, selectedNode.Card);
            tasks.Add(GameMgr.AwaitAndDisableInput(Battle.Dealer.CreateCardAndPutInto(copiedCard, selectedNode, Enums.CardFace.Up, Battle.ItemCardContainer)));
            await Task.WhenAll(tasks);
        }

        protected override BaseInputHandler GetInputHandler()
        {
            return new CopyPasteCardInputHandler(GameMgr, CardNode);
        }
    }
    
    public CopyPasteCard(CardDef def) : base(def)
    {
    }
    
    protected override CardPropItem CreateItemProp()
    {
        var validContainers = new List<CardContainer> { Battle.Player.HoleCardContainer, Battle.CommunityCardContainer, Battle.Enemy.HoleCardContainer };
        return new CopyPasteCardItemProp(this, validContainers);
    }
}