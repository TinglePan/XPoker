using System.Collections.Generic;
using XCardGame.Scripts.Buffs;
using XCardGame.Scripts.Common.Constants;
using XCardGame.Scripts.Defs;
using XCardGame.Scripts.Effects;
using XCardGame.Scripts.Effects.SkillEffects;
using XCardGame.Scripts.GameLogic;

namespace XCardGame.Scripts.Cards.SkillCards;

public class FeintSkillCard: BaseSkillCard
{
    public FeintSkillCard(SkillCardDef def): base(def)
    {
    }

    protected override void SetUpContents(Dictionary<string, object> args)
    {
        var opponent = Battle.GetOpponentOf(OwnerEntity);
        Contents = new Dictionary<Enums.EngageRole, List<BaseSkillEffect>>()
        {
            {
                Enums.EngageRole.Attacker, new List<BaseSkillEffect>()
                {
                    new BuffSkillEffect(opponent, Battle, this, Enums.HandTier.HighCard, new VulnerableDeBuff(1)),
                }
            }
        };
    }
}