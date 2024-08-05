using XCardGame.TimingInterfaces;

namespace XCardGame.CardProperties;

public class CardPropRuleRoundReTrigger: CardPropRule, IRoundStart, IRoundEnd
{
    public CardPropRuleRoundReTrigger(BaseCard card) : base(card)
    {
    }

    public void OnRoundStart()
    {
        OnStartEffect();
    }

    public void OnRoundEnd()
    {
        OnStopEffect();
    }
}