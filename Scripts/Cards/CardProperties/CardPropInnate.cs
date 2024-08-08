namespace XCardGame.CardProperties;

public class CardPropInnate: BaseCardProp
{
    protected Battle Battle => Card.Battle;
    
    public CardPropInnate(BaseCard card) : base(card)
    {
    }

    // NOTE: Dealing innate cards is currently done manually in battle process 
    // public async void OnBattleStart()
    // {
    //     if (Card.Def.IsInnate)
    //     {
    //         if (Card.Def.IsItem && Battle.ItemCardContainer.ContentNodes.Count < Battle.Player.ItemPocketSize.Value)
    //         {
    //             await Battle.Dealer.DealDedicatedCardIntoContainer(Card, Battle.ItemCardContainer);
    //         } else if (Card.Def.IsRule)
    //         {
    //             await Battle.Dealer.DealDedicatedCardIntoContainer(Card, Battle.RuleCardContainer);
    //         }
    //     }
    // }
}