using System.Collections.Generic;
using System.Threading.Tasks;
using XCardGame.TimingInterfaces;
using XCardGame.Ui;

namespace XCardGame.CardProperties;

public class CardPropPiled: BaseCardProp, IEnterField, ILeaveField
{
    public int CardCount;
    
    public CardPropPiled(BaseCard card, int cardCount) : base(card)
    {
        CardCount = cardCount;
    }

    public async void OnEnterField()
    {
        var tasks = new List<Task>();
        for (int i = 0;i < CardCount; i++)
        {
            tasks.Add(Card.Battle.Dealer.DrawCardToPile(((PiledCardNode)CardNode).CardPile));
        }
        await Task.WhenAll(tasks);
    }

    public void OnLeaveField()
    {
        throw new System.NotImplementedException();
    }
}