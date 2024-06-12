using System.Collections.Generic;
using XCardGame.Scripts.Buffs;
using XCardGame.Scripts.Common;
using XCardGame.Scripts.Common.Constants;
using XCardGame.Scripts.Effects;
using XCardGame.Scripts.Effects.SkillEffects;
using XCardGame.Scripts.GameLogic;
using XCardGame.Scripts.HandEvaluate;

namespace XCardGame.Scripts.Cards.SkillCards;

public class RiposteSkillCard: BaseSkillCard
{
    
    public class RiposteSkillEffect: BuffSkillEffect
    {
        public RiposteSkillEffect(BattleEntity target, BaseCard createdByCard) : base(target, createdByCard)
        {
        }

        public override BaseBuff PrepareBuff(CompletedHand hand, BattleEntity self, BattleEntity opponent)
        {
            var power = self.GetPower(hand.Tier);
            return new RiposteBuff(power);
        }
    }
    
    public RiposteSkillCard(Enums.CardSuit suit, Enums.CardRank rank, BattleEntity ownerEntity) : 
        base("Riposte", "Negate the next incoming attack, then counter attacks", "res://Sprites/riposte.png", suit, 
            rank, Enums.HandTier.HighCard, null, ownerEntity)
    {
        IsExpanding = true;
        var opponent = Battle.GetOpponentOf(ownerEntity);
        Contents = new Dictionary<Enums.EngageRole, List<BaseSkillEffect>>()
        {
            {
                Enums.EngageRole.Attacker, new List<BaseSkillEffect>()
                {
                    new RiposteSkillEffect(opponent, this),
                }
            }
        };
    }
}