using System.Collections.Generic;
using XCardGame.Scripts.Buffs;
using XCardGame.Scripts.Common.Constants;
using XCardGame.Scripts.Effects;
using XCardGame.Scripts.Effects.SkillEffects;
using XCardGame.Scripts.GameLogic;

namespace XCardGame.Scripts.Cards.SkillCards;

public class FeintSkillCard: BaseSkillCard
{
    public FeintSkillCard(Enums.CardSuit suit, Enums.CardRank rank, BattleEntity ownerEntity) : 
        base("Feint", "Grants vulnerable instead of dealing damage", "res://Sprites/feint", suit, 
            rank, null, ownerEntity)
    {

        var opponent = Battle.GetOpponentOf(ownerEntity);
        Contents = new Dictionary<Enums.EngageRole, List<BaseSkillEffect>>()
        {
            {
                Enums.EngageRole.Attacker, new List<BaseSkillEffect>()
                {
                    new BuffSkillEffect(opponent, this, Enums.HandTier.HighCard, new VulnerableDeBuff(1)),
                }
            }
        };
    }
}