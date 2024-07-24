using System.Collections.Generic;
using System.Threading.Tasks;
using XCardGame.Common;
using XCardGame.Ui;

namespace XCardGame;

public class BalaTrollHandCard: BaseItemCard
{
    public class BalaTrollHandCardInputHandler : BaseItemCardSelectTargetInputHandler<BalaTrollHandCard>
    {
        public BalaTrollHandCardInputHandler(GameMgr gameMgr, CardNode node) : base(gameMgr, node)
        {
        }

        protected override IEnumerable<CardNode> GetValidSelectTargets()
        {
            foreach (var node in OriginateCard.PlayerCardContainer.CardNodes)
            {
                yield return node;
            }
        }

        protected override async void Confirm()
        {
            var tasks = new List<Task>();
            if (SelectedNodes.Count > 0)
            {
                foreach (var selectedNode in SelectedNodes)
                {
                    tasks.Add(Battle.Dealer.DealCardAndReplace(selectedNode));
                    await Utils.Wait(OriginateCardNode, Configuration.AnimateCardTransformInterval);
                }
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
    
    public CardContainer PlayerCardContainer; 
    
    public BalaTrollHandCard(ItemCardDef def): base(def)
    {
    }

    public override void Setup(object o)
    {
        base.Setup(o);
        PlayerCardContainer = Battle.Player.HoleCardContainer;
    }
    
    public override bool CanInteract(CardNode node)
    {
        return base.CanInteract(node) && Battle.CurrentState == Battle.State.BeforeShowDown;
    }

    public override void ChooseTargets(CardNode node)
    {
        var inputHandler = new BalaTrollHandCardInputHandler(GameMgr, node);
        GameMgr.InputMgr.SwitchToInputHandler(inputHandler);
    }
}