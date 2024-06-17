using XCardGame.Scripts.Common;
using XCardGame.Scripts.Common.Constants;
using XCardGame.Scripts.Defs;
using XCardGame.Scripts.GameLogic;

namespace XCardGame.Scripts.Cards.AbilityCards;

public class EyePatchCard: BaseTapCard
{

    public int Count;
    
    public EyePatchCard(TapCardDef def): base(def)
    {
    }
    
    public override void OnStart(Battle battle)
    {
        base.OnStart(battle);
        Count = Utils.GetCardRankValue(Rank.Value);
        battle.FaceDownCommunityCardCount += Count;
    }
    
    public override void OnStop(Battle battle)
    {
        base.OnStop(battle);
        battle.FaceDownCommunityCardCount -= Count;
    }
}