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
            Enums.HandTier.TwoPairs, null,  ownerEntity)
    {
        Contents = new Dictionary<Enums.EngageRole, List<BaseSkillEffect>>()
        {
            {
                Enums.EngageRole.Attacker, new List<BaseSkillEffect>()
                {
                    new DamageSkillEffect(this, 0, 1),
                    new DamageSkillEffect(this, 0, 1),
                }
            }
        };
    }
}