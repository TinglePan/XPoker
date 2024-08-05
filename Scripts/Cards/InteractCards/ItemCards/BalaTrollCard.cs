using System.Collections.Generic;
using System.Threading.Tasks;
using XCardGame.CardProperties;
using XCardGame.Common;
using XCardGame.Ui;

namespace XCardGame;

public class BalaTrollCard: BaseCard
{
    public class BalaTrollCardInputHandler : BaseCardSelectTargetInputHandlerWithConfirmConstraints
    {
        public BalaTrollCardInputHandler(GameMgr gameMgr, CardNode node) : base(gameMgr, node)
        {
        }

        protected override IEnumerable<CardNode> GetValidSelectTargets()
        {
            return Helper.OriginateCard.GetProp<BalaTrollCardItemProp>().PlayerHoleCardContainer.CardNodes;
        }
    }

    public class BalaTrollCardItemProp : CardPropItem
    {
        public CardContainer PlayerHoleCardContainer; 
        public BalaTrollCardItemProp(BaseCard card, CardContainer playerHoleCardContainer) : base(card)
        {
            PlayerHoleCardContainer = playerHoleCardContainer;
        }
        
        public override bool CanUse()
        {
            if (!base.CanUse()) return false;
            return Battle.CurrentState.Value == Battle.State.BeforeShowDown;
        }

        public override async Task Effect(List<CardNode> targets)
        {
            var tasks = new List<Task>();
            var tasks2 = new List<Task>();
            tasks.Add(base.Effect(targets));
            foreach (var selectedNode in targets)
            {
                var sourceContainer = selectedNode.CurrentContainer.Value;
                selectedNode.IsSelected = false;
                tasks.Add(selectedNode.AnimateLeaveField());
                tasks2.Add(Battle.Dealer.DrawCardIntoContainer((CardContainer)sourceContainer));
                await Utils.Wait(CardNode, Configuration.AnimateCardTransformInterval);
            }
            await Task.WhenAll(tasks);
            await Task.WhenAll(tasks2);
        }

        protected override BaseInputHandler GetInputHandler()
        {
            return new BalaTrollCardInputHandler(GameMgr, CardNode);
        }
    }
    
    public BalaTrollCard(CardDef def): base(def)
    {
    }
    
    protected override CardPropItem CreateItemProp()
    {
        return new BalaTrollCardItemProp(this, Battle.Player.HoleCardContainer);
    }
}