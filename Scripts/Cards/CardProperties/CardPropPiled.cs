using System.Collections.Generic;
using System.Threading.Tasks;
using XCardGame.Common;
using XCardGame.TimingInterfaces;
using XCardGame.Ui;

namespace XCardGame.CardProperties;

public class CardPropPiled: BaseCardProp, IEnterField, ILeaveField
{
    public int CardCount;
    public bool IsOpened;
    public Battle Battle => Card.Battle;
    public PiledCardNode PiledCardNode => (PiledCardNode)CardNode;
    
    public CardPropPiled(BaseCard card, int cardCount) : base(card)
    {
        CardCount = cardCount;
    }

    public virtual async Task Open()
    {
        IsOpened = true;
        if (Battle.OpenedPiledCardContainer.CardContainers[0].Contents.Count != 0)
        {
            var currOpenedPiledCardNode = (CardNode)Battle.OpenedPiledCardContainer.CardContainers[0].ContentNodes[0];
            await currOpenedPiledCardNode.Card.GetProp<CardPropPiled>().Close();
        }
        var tasks = new List<Task>();
        foreach (var card in PiledCardNode.CardPile.Cards)
        {
            tasks.Add(Battle.Dealer.DealDedicatedCardIntoContainer(card, PiledCardNode.CardPile, Battle.OpenedPiledCardContainer.CardContainers[1], collectUsable:false));
        }
        var stubCardNode = PiledCardNode.CardPile.CreateCardNodeOnPile(Card);
        tasks.Add(Battle.Dealer.DealDedicatedCardNodeIntoContainer(stubCardNode, Battle.OpenedPiledCardContainer.CardContainers[0], collectUsable:false));
        await Task.WhenAll(tasks);
    }

    public virtual async Task Close()
    {
        IsOpened = false;
        var tasks = new List<Task>();
        foreach (var cardNode in Battle.OpenedPiledCardContainer.CardContainers[1].ContentNodes)
        {
            tasks.Add(Battle.Dealer.DealDedicatedCardNodeToPile((CardNode)cardNode, PiledCardNode.CardPile));
        }
        tasks.Add(CardNode.AnimateExhaust(Configuration.ExhaustAnimationTime));
        await Task.WhenAll(tasks);
    }

    public async void OnEnterField()
    {
        var tasks = new List<Task>();
        for (int i = 0;i < CardCount; i++)
        {
            tasks.Add(Battle.Dealer.DrawCardToPile(PiledCardNode.CardPile));
        }
        await Task.WhenAll(tasks);
    }

    public async void OnLeaveField()
    {
        var tasks = new List<Task>();
        foreach (var card in PiledCardNode.CardPile.Cards)
        {
            tasks.Add(PiledCardNode.Battle.Dealer.DealDedicatedCardToPile(card, PiledCardNode.CardPile,
                Battle.Dealer.DiscardCardPile));
        }
        for (int i = 0;i < CardCount; i++)
        {
            tasks.Add(Battle.Dealer.DrawCardToPile(PiledCardNode.CardPile));
        }
        if (IsOpened)
        {
            tasks.Add(Close());
        }
        await Task.WhenAll(tasks);
    }
}