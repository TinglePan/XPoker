using System.Collections.Generic;
using XCardGame.CardProperties;
using XCardGame.Ui;

namespace XCardGame;

public class BaseCardReplaceInputHandler : BaseCardSelectTargetInputHandlerWithConfirmConstraints
{
    public BaseCardReplaceInputHandler(GameMgr gameMgr, CardNode node) : base(gameMgr, node,
        selectTargetCountLimit: 1)
    {
    }
        
    protected override IEnumerable<CardNode> GetValidSelectTargets()
    {
        return Helper.OriginateCard.GetProp<CardPropItemReplace>().GetValidSelectTargets();
    }
}