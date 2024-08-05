using XCardGame.Common;
using XCardGame.TimingInterfaces;

namespace XCardGame.CardProperties;

public class CardPropUnstable: BaseCardProp, IRoundStart
{
    public CardPropUnstable(BaseCard card) : base(card)
    {
    }

    public async void OnRoundStart()
    {
        if (!(CardNode.IsTapped.Value ^ Card.Def.UnstableWhenTapped))
        {
            await CardNode.AnimateTap(!CardNode.IsTapped.Value, Configuration.TapTweenTime);
            Card.Reset();
        }
    }
}