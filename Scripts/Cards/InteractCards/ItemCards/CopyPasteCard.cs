using System.Collections.Generic;
using System.Threading.Tasks;
using XCardGame.Scripts.Cards.CardInputHandlers;
using XCardGame.Scripts.Common;
using XCardGame.Scripts.Common.Constants;
using XCardGame.Scripts.Defs;
using XCardGame.Scripts.Defs.Def.Card;
using XCardGame.Scripts.Game;
using XCardGame.Scripts.Ui;

namespace XCardGame.Scripts.Cards.InteractCards.ItemCards;

public class CopyPasteCard: BaseItemCard
{
    public class CopyCard : BaseTokenCard<CopyCard, BaseTokenCardInputHandler<CopyCard>>
    {
        protected static ItemCardDef CreateDefFromCopiedCard(ItemCardDef def, BaseCard card)
        {
            var res = new ItemCardDef()
            {
                Name = def.Name,
                DescriptionTemplate = def.DescriptionTemplate,
                ConcreteClassPath = def.ConcreteClassPath,
                Rarity = def.Rarity,
                IconPath = def.IconPath,
                Cost = def.Cost,
                RankChangePerUse = def.RankChangePerUse,
            };
            def.Rank = card.Def.Rank;
            def.Suit = card.Def.Suit;
            return res;
        }
        
        public CopyCard(ItemCardDef def, BaseCard target) : base(CreateDefFromCopiedCard(def, target))
        {
            
        }

        protected override BaseTokenCardInputHandler<CopyCard> GetInputHandler()
        {
            var cardNode = Node<CardNode>();
            return new BaseTokenCardInputHandler<CopyCard>(GameMgr, cardNode);
        }
    }
    
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
                selectedNode.IsSelected = false;
                var copiedCard = new CopyCard(CardDefs.Copy, selectedNode.Card);
                tasks.Add(GameMgr.AwaitAndDisableProceed(Battle.Dealer.CreateCardAndPutInto(copiedCard, selectedNode, Enums.CardFace.Up, Battle.CommunityCardContainer)));
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
    
    public CopyPasteCard(ItemCardDef def) : base(def)
    {
    }

    public override void Setup(SetupArgs args)
    {
        base.Setup(args);
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
        var inputHandler = new BalaTrollHandCard.BalaTrollHandCardInputHandler(GameMgr, node);
        GameMgr.InputMgr.SwitchToInputHandler(inputHandler);
    }
}