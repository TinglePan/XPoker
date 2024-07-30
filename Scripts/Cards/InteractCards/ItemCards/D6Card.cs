using System.Collections.Generic;
using System.Threading.Tasks;
using Godot;
using XCardGame.Common;
using XCardGame.Ui;

namespace XCardGame;

public class D6Card: BaseItemCard
{
    public List<CardContainer> CardContainers;
    public D6Card(ItemCardDef def): base(def)
    {
    }

    public override void Setup(object o)
    {
        base.Setup(o);
        CardContainers = new List<CardContainer>
        {
            Battle.CommunityCardContainer,
            Battle.Player.HoleCardContainer,
            Battle.Enemy.HoleCardContainer
        };
    }

    public override bool CanInteract(CardNode node)
    {
        return base.CanInteract(node) && Battle.CurrentState.Value == Battle.State.BeforeShowDown;
    }

    public override async void Use(CardNode node)
    {
        async Task Run()
        {
            var tasks = new List<Task>();
            foreach (var cardContainer in CardContainers)
            {
                foreach (var cardNode in cardContainer.CardNodes)
                {
                    if (cardNode.FaceDirection.Value != Enums.CardFace.Up) continue;
                    tasks.Add(Battle.Dealer.DealCardAndReplace(cardNode));
                    await Utils.Wait(node, Configuration.AnimateCardTransformInterval);
                }
            }
            await Task.WhenAll(tasks);
        }
        GD.Print("Use d6");
        await GameMgr.AwaitAndDisableInput(Run());
        base.Use(node);
    }
}