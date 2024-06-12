using System.Collections.Generic;
using XCardGame.Scripts.Buffs;
using XCardGame.Scripts.Cards.SkillCards;
using XCardGame.Scripts.Common.Constants;
using XCardGame.Scripts.Effects.SkillEffects;
using XCardGame.Scripts.HandEvaluate;

namespace XCardGame.Scripts.GameLogic;

public class SkillResolver
{
    public GameMgr GameMgr;
    public Battle Battle;
    public Engage Engage;
    
    public SkillResolver(GameMgr gameMgr, Engage engage)
    {
        GameMgr = gameMgr;
        Battle = gameMgr.CurrentBattle;
        Engage = engage;
    }

    public void Resolve(BaseSkillCard skillCard, List<BaseSkillEffect> effects, CompletedHand hand, BattleEntity self, BattleEntity opponent)
    {
        foreach (var effect in effects)
        {
            effect.Resolve(this, hand, self, opponent);
        }
    }
}