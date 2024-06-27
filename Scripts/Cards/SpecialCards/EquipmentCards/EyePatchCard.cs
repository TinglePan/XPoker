using XCardGame.Scripts.Common;
using XCardGame.Scripts.Common.Constants;
using XCardGame.Scripts.Defs;
using XCardGame.Scripts.Defs.Def.Card;
using XCardGame.Scripts.GameLogic;

namespace XCardGame.Scripts.Cards.AbilityCards;

public class EyePatchCard: BaseTapCard
{
    protected int Count;
    
    public EyePatchCard(InteractCardDef def): base(def)
    {
    }
    
    public override void OnStart(Battle battle)
    {
        base.OnStart(battle);
        if (IsFunctioning() && !AlreadyFunctioning)
        {
            Count = Utils.GetCardRankValue(Rank.Value);
            battle.FaceDownCommunityCardCount += Count;
            AlreadyFunctioning = true;
        }
    }
    
    public override void OnStop(Battle battle)
    {
        base.OnStop(battle);
        if (AlreadyFunctioning)
        {
            battle.FaceDownCommunityCardCount -= Count;
            AlreadyFunctioning = false;
        }
    }
}