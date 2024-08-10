using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot;
using XCardGame.Common;
using XCardGame.Ui;

namespace XCardGame.CardProperties;

public class CardPropPiled: BaseCardProp, IEnterField, ILeaveField
{
    public int CardCount;
    
    public bool IsOpened;
    public Battle Battle => Card.Battle;
    public PiledCardNode PiledCardNode => (PiledCardNode)CardNode;
    public CardNode StubCardNode;
    
    public CardPropPiled(BaseCard card, int cardCount) : base(card)
    {
        CardCount = cardCount;
    }

    public virtual async Task Open()
    {
        IsOpened = true;

        Battle.OpenedPiledCardContainer.TweenContainerPosition = true;
        if (Battle.OpenedPiledCardContainer.CardContainers[0].Contents.Count != 0)
        {
            var currOpenedPiledCardNode = (CardNode)Battle.OpenedPiledCardContainer.CardContainers[0].ContentNodes[0];
            var currOpenedPiledCard = (OpenedPiledCard)currOpenedPiledCardNode.Card;
            await currOpenedPiledCard.PiledCard.GetProp<CardPropPiled>().Close();
            GD.Print("old opened piled card closed");
        }
        var tasks = new List<Task>();
        Battle.OpenedPiledCardContainer.TweenContainerPosition = false;
        foreach (var card in PiledCardNode.CardPile.Cards.ToList())
        {
            tasks.Add(Battle.Dealer.DealDedicatedCardIntoContainer(card, PiledCardNode.CardPile, Battle.OpenedPiledCardContainer.CardContainers[1], collectUsable:false));
        }
        await Task.WhenAll(tasks);
        var stubCard = new OpenedPiledCard(Card);
        StubCardNode = PiledCardNode.CardPile.CreateCardNodeOnPile(stubCard);
        StubCardNode.CurrentContainer.Value = CardNode.CurrentContainer.Value;
        // StubCardNode.PrintPosition = true;
        await Battle.Dealer.DealDedicatedCardNodeIntoContainer(StubCardNode,
            Battle.OpenedPiledCardContainer.CardContainers[0], collectUsable: false);
        Battle.OpenedPiledCardContainer.TweenContainerPosition = true;
    }

    public virtual async Task Close()
    {
        IsOpened = false;
        var tasks = new List<Task>();
        Battle.OpenedPiledCardContainer.TweenContainerPosition = true;
        foreach (var cardNode in Battle.OpenedPiledCardContainer.CardContainers[1].ContentNodes.ToList())
        {
            tasks.Add(Battle.Dealer.DealDedicatedCardNodeToPile((CardNode)cardNode, PiledCardNode.CardPile));
        }
        tasks.Add(StubCardNode.AnimateLeaveField());
        await Task.WhenAll(tasks);
    }

    public virtual void OnEnterField()
    {
    }

    public async void OnLeaveField()
    {
        if (IsOpened)
        {
            await Close();
        }
    }
}