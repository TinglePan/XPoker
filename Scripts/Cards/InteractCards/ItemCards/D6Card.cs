using System.Collections.Generic;
using System.Threading.Tasks;
using XCardGame.Scripts.Common;
using XCardGame.Scripts.Common.Constants;
using XCardGame.Scripts.Defs.Def.Card;
using XCardGame.Scripts.Game;
using XCardGame.Scripts.Ui;

namespace XCardGame.Scripts.Cards.InteractCards.ItemCards;

public class D6Card: BaseItemCard
{
    public List<CardContainer> CardContainers;
    public D6Card(ItemCardDef def): base(def)
    {
    }

    public override void Setup(Dictionary<string, object> args)
    {
        base.Setup(args);
        CardContainers = new List<CardContainer>
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

    public override async void Use(CardNode node)
    {
        base.Use(node);
        var tasks = new List<Task>();
        foreach (var cardContainer in CardContainers)
        {
            var cardNodes = new List<CardNode>(cardContainer.ContentNodes);
            foreach (var cardNode in cardNodes)
            {
                if (cardNode.FaceDirection.Value != Enums.CardFace.Up) continue;
                tasks.Add(Battle.Dealer.DealCardAndReplace(cardNode));
                await Utils.Wait(node, Configuration.AnimateCardTransformInterval);
            }
        }
        await Task.WhenAll(tasks);
    }
}