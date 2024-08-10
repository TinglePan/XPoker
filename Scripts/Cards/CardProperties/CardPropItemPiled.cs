using XCardGame.Ui;

namespace XCardGame.CardProperties;

public class CardPropItemPiled: CardPropItem
{
    public CardPropPiled PiledProp => Card.GetProp<CardPropPiled>();
    
    public CardPropItemPiled(BaseCard card) : base(card)
    {
    }
    
    protected override BaseInputHandler GetInputHandler()
    {
        return new BaseCardSelectTargetInputHandlerWithConfirmConstraints(GameMgr, PiledProp.StubCardNode, getValidSelectTargetsFunc:GetValidSelectTargets);
    }
    
    
}