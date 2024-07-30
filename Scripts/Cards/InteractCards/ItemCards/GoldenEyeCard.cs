using System.Collections.Generic;
using System.Threading.Tasks;
using XCardGame.Common;
using XCardGame.Ui;

namespace XCardGame;

public class GoldenEyeCard: BaseItemCard
{
    public List<CardContainer> CardContainers;
    // public List<CardNode> RevealedCardNodes;
    
    public GoldenEyeCard(ItemCardDef def): base(def)
    {
        // RevealedCardNodes = new List<CardNode>();
    }
    
    public override void Setup(object o)
    {
        base.Setup(o);
        CardContainers = new List<CardContainer>
        {
            Battle.CommunityCardContainer,
            Battle.Enemy.HoleCardContainer
        };
    }
    
    
    public override bool CanInteract(CardNode node)
    {
        return base.CanInteract(node) && Battle.CurrentState.Value == Battle.State.BeforeShowDown;
    }

    public override async void Use(CardNode node)
    {
        base.Use(node);
        var tasks = new List<Task>();
        foreach (var cardContainer in CardContainers)
        {
            foreach (var cardNode in cardContainer.CardNodes)
            {
                if (cardNode.FaceDirection.Value == Enums.CardFace.Up) continue;
                tasks.Add(GameMgr.AwaitAndDisableInput(cardNode.AnimateReveal(true, Configuration.RevealTweenTime)));
                await Utils.Wait(node, Configuration.AnimateCardTransformInterval);
            }
        }
        await Task.WhenAll(tasks);
    }
}