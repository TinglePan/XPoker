using XCardGame.Ui;

namespace XCardGame.CardProperties;

public class BaseCardProp
{
    public BaseCard Card;
    public CardNode CardNode => Card.Node<CardNode>();

    public BaseCardProp(BaseCard card)
    {
        Card = card;
    }
}