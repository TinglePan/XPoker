using System.Collections.Generic;
using XCardGame.Scripts.Buffs;
using XCardGame.Scripts.Common.Constants;
using XCardGame.Scripts.Defs;
using XCardGame.Scripts.Effects;
using XCardGame.Scripts.Effects.SkillEffects;
using XCardGame.Scripts.GameLogic;

namespace XCardGame.Scripts.Cards.SkillCards;

public class DualWieldSkillCard: BaseSkillCard
{
    public DualWieldSkillCard(SkillCardDef def): base(def)
    {
        Def.Name = "Dual wield";
        Def.DescriptionTemplate = "Attack twice";
        Def.IconPath = "res://Sprites/Cards/dual_wield.png";
    }

    protected override void SetUpContents(Dictionary<string, object> args)
    {
        Contents = new Dictionary<Enums.EngageRole, List<BaseSkillEffect>>()
        {
            {
                Enums.EngageRole.Attacker, new List<BaseSkillEffect>()
                {
                    new DamageSkillEffect(Battle, this, Enums.HandTier.TwoPairs, 0, 1),
                    new DamageSkillEffect(Battle, this, Enums.HandTier.TwoPairs, 0, 1),
                }
            }
        };
    }
}