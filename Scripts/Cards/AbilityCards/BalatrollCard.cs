using System.Collections.Generic;
using Godot;
using XCardGame.Scripts.Common;
using XCardGame.Scripts.Common.Constants;
using XCardGame.Scripts.Defs;
using XCardGame.Scripts.GameLogic;
using XCardGame.Scripts.InputHandling;
using XCardGame.Scripts.Nodes;

namespace XCardGame.Scripts.Cards.AbilityCards;

public class BalatrollCard: BaseUseCard
{
    public class BalatrollCardInputHandler : BaseInteractCardInputHandler<BalatrollCard>
    {
        public BalatrollCardInputHandler(GameMgr gameMgr, Battle battle, BalatrollCard card) : base(gameMgr, battle, card)
        {
        }

        protected override IEnumerable<CardNode> GetValidSelectTargets()
        {
            foreach (var node in Card.PlayerCardContainer.ContentNodes)
            {
                yield return node;
            }
        }

        protected override async void Confirm()
        {
            if (SelectedCardNodes.Count != 0)
            {
                foreach (var selectedCardNode in SelectedCardNodes)
                {
                    selectedCardNode.IsSelected = false;
                    await Card.Battle.Dealer.DealCardAndReplace(selectedCardNode, Configuration.AnimateCardTransformInterval);
                }
                SelectedCardNodes.Clear();
                Card.Use();
                GameMgr.InputMgr.QuitCurrentInputHandler();
            }
            else
            {
                // TODO: Hint on invalid confirm
            }
        }
    }
    
    public CardContainer PlayerCardContainer; 
    
    public BalatrollCard(UseCardDef def): base(def)
    {
    }

    public override void Setup(Dictionary<string, object> args)
    {
        base.Setup(args);
        PlayerCardContainer = Battle.Player.HoleCardContainer;
    }
    
    public override bool CanInteract()
    {
        return base.CanInteract() && Battle.CurrentState == Battle.State.BeforeShowDown;
    }

    public override void ChooseTargets()
    {
        var inputHandler =
            new BalatrollCardInputHandler(GameMgr, Battle, this);
        GameMgr.InputMgr.SwitchToInputHandler(inputHandler);
    }
}