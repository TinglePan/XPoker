﻿using System.Collections.Generic;
using System.Linq;
using XCardGame.Scripts.Common;
using XCardGame.Scripts.Common.Constants;
using XCardGame.Scripts.Defs;
using XCardGame.Scripts.Effects.SkillEffects;
using XCardGame.Scripts.GameLogic;
using XCardGame.Scripts.HandEvaluate;

namespace XCardGame.Scripts.Cards.SkillCards;

public class TwoHandedSkillCard: BaseSkillCard
{
    public TwoHandedSkillCard(BaseCardDef def) : 
        base(def)
    {
        Def.Name = "Two handed";
        Def.DescriptionTemplate = "Make an attack that scales more with power";
        Def.IconPath = "res://Sprites/Cards/two_handed.png";
    }

    protected override void SetUpContents(Dictionary<string, object> args)
    {
        Contents = new Dictionary<Enums.EngageRole, List<BaseSkillEffect>>()
        {
            {
                Enums.EngageRole.Attacker, new List<BaseSkillEffect>()
                {
                    new DamageSkillEffect(Battle, this, Enums.HandTier.TwoPairs, 0, 2),
                }
            }
        };
    }
}