using System.Collections.Generic;
using System.Linq;
using XCardGame.Scripts.Common;
using XCardGame.Scripts.Common.Constants;
using XCardGame.Scripts.Effects.SkillEffects;
using XCardGame.Scripts.GameLogic;
using XCardGame.Scripts.HandEvaluate;

namespace XCardGame.Scripts.Cards.SkillCards;

public class TwoHandedSkillCard: BaseSkillCard
{
    public TwoHandedSkillCard(Enums.CardSuit suit, Enums.CardRank rank, BattleEntity ownerEntity) : 
        base("Two handed", "Make an attack that scales more with power", "res://Sprites/Cards/two_handed.png", suit, rank, 
            null, ownerEntity)
    {
        Contents = new Dictionary<Enums.EngageRole, List<BaseSkillEffect>>()
        {
            {
                Enums.EngageRole.Attacker, new List<BaseSkillEffect>()
                {
                    new DamageSkillEffect(this, Enums.HandTier.TwoPairs, 0, 2),
                }
            }
        };
    }
}