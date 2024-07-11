using System.Collections.Generic;
using System.Threading.Tasks;
using XCardGame.Scripts.Cards.CardInputHandlers;
using XCardGame.Scripts.Common;
using XCardGame.Scripts.Common.Constants;
using XCardGame.Scripts.Defs.Def.Card;
using XCardGame.Scripts.Game;
using XCardGame.Scripts.InputHandling;
using XCardGame.Scripts.Ui;

namespace XCardGame.Scripts.Cards.InteractCards.ItemCards;

public class BalaTrollHandCard: BaseItemCard
{
    public class BalaTrollHandCardInputHandler : BaseItemCardSelectTargetInputHandler<BalaTrollHandCard>
    {
        public BalaTrollHandCardInputHandler(GameMgr gameMgr, CardNode node) : base(gameMgr, node)
        {
        }

        protected override IEnumerable<CardNode> GetValidSelectTargets()
        {
            foreach (var node in OriginateCard.PlayerCardContainer.ContentNodes)
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
                    selectedNode.IsSelected = false;
                    tasks.Add(Battle.Dealer.DealCardAndReplace(selectedNode));
                    await Utils.Wait(OriginateCardNode, Configuration.AnimateCardTransformInterval);
                }
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
    
    public CardContainer PlayerCardContainer; 
    
    public BalaTrollHandCard(ItemCardDef def): base(def)
    {
    }

    public override void Setup(Dictionary<string, object> args)
    {
        base.Setup(args);
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