using System.Collections.Generic;
using XCardGame.Scripts.Buffs;
using XCardGame.Scripts.Common.Constants;
using XCardGame.Scripts.Effects;
using XCardGame.Scripts.Effects.SkillEffects;
using XCardGame.Scripts.GameLogic;

namespace XCardGame.Scripts.Cards.SkillCards;

public class DualWieldSkillCard: BaseSkillCard
{
    public DualWieldSkillCard(Enums.CardSuit suit, Enums.CardRank rank, BattleEntity ownerEntity) : 
        base("Dual wield", "Attack twice", "res://Sprites/Cards/dual_wield.png", suit, rank, 
            null,  ownerEntity)
    {
        Contents = new Dictionary<Enums.EngageRole, List<BaseSkillEffect>>()
        {
            {
                Enums.EngageRole.Attacker, new List<BaseSkillEffect>()
                {
                    new DamageSkillEffect(this, Enums.HandTier.TwoPairs, 0, 1),
                    new DamageSkillEffect(this, Enums.HandTier.TwoPairs, 0, 1),
                }
            }
        };
    }
}