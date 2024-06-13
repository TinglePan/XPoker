using System.Collections.Generic;
using XCardGame.Scripts.Common.Constants;
using XCardGame.Scripts.Effects.SkillEffects;
using XCardGame.Scripts.GameLogic;

namespace XCardGame.Scripts.Cards.SkillCards;

public class SuckerPunchSkillCard: BaseSkillCard
{
    public SuckerPunchSkillCard(Enums.CardSuit suit, Enums.CardRank rank, BattleEntity ownerEntity) : 
        base("Sucker punch", "Attack as a defender", "res://Sprites/Cards/sucker_punch.png", suit, rank, 
            null, ownerEntity)
    {
        Contents = new Dictionary<Enums.EngageRole, List<BaseSkillEffect>>()
        {
            {
                Enums.EngageRole.Defender, new List<BaseSkillEffect>()
                {
                    new DamageSkillEffect(this, Enums.HandTier.TwoPairs, 0, 1),
                }
            }
        };
    }
}