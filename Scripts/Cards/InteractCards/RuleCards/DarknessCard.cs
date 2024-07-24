using XCardGame.Common;

namespace XCardGame;

public class DarknessCard: BaseRuleCard
{
    protected int Count;
    
    public DarknessCard(RuleCardDef def): base(def)
    {
    }
    
    public override void OnStartEffect(Battle battle)
    {
        base.OnStartEffect(battle);
        if (IsFunctioning() && !AlreadyFunctioning)
        {
            Count = Utils.GetCardRankValue(Rank.Value);
            battle.FaceDownCommunityCardCount += Count;
            AlreadyFunctioning = true;
        }
    }
    
    public override void OnStopEffect(Battle battle)
    {
        base.OnStopEffect(battle);
        if (AlreadyFunctioning)
        {
            battle.FaceDownCommunityCardCount -= Count;
            AlreadyFunctioning = false;
        }
    }
}