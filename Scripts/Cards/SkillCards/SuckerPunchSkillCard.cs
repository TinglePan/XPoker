using System.Collections.Generic;
using XCardGame.Scripts.Common.Constants;
using XCardGame.Scripts.Defs;
using XCardGame.Scripts.Effects.SkillEffects;
using XCardGame.Scripts.GameLogic;

namespace XCardGame.Scripts.Cards.SkillCards;

public class SuckerPunchSkillCard: BaseSkillCard
{
    public SuckerPunchSkillCard(BaseCardDef def): base(def)
    {
        Def.Name = "Sucker punch";
        Def.DescriptionTemplate = "Attack as a defender";
        Def.IconPath = "res://Sprites/Cards/sucker_punch.png";
    }

    protected override void SetUpContents(Dictionary<string, object> args)
    {
        Contents = new Dictionary<Enums.EngageRole, List<BaseSkillEffect>>()
        {
            {
                Enums.EngageRole.Defender, new List<BaseSkillEffect>()
                {
                    new DamageSkillEffect(Battle, this, Enums.HandTier.TwoPairs, 0, 1),
                }
            }
        };
    }
}