using System.Collections.Generic;
using XCardGame.Scripts.Buffs;
using XCardGame.Scripts.Common;
using XCardGame.Scripts.Common.Constants;
using XCardGame.Scripts.Defs;
using XCardGame.Scripts.Effects;
using XCardGame.Scripts.Effects.SkillEffects;
using XCardGame.Scripts.GameLogic;
using XCardGame.Scripts.HandEvaluate;

namespace XCardGame.Scripts.Cards.SkillCards;

public class RiposteSkillCard: BaseSkillCard
{
    public class RiposteSkillEffect: BuffSkillEffect
    {
        public RiposteSkillEffect(BattleEntity target, Battle battle, BaseCard createdByCard, Enums.HandTier triggerHandTier) : base(target, battle, createdByCard, triggerHandTier, expanding:0)
        {
        }

        public override BaseBuff PrepareBuff(CompletedHand hand, BattleEntity self, BattleEntity opponent)
        {
            var power = self.GetPower(hand.Tier);
            return new RiposteBuff(power);
        }
    }
    
    public RiposteSkillCard(BaseCardDef def): base(def)
    {
        Def.Name = "Riposte";
        Def.DescriptionTemplate = "Negate the next incoming attack, then counter attack";
        Def.IconPath = "res://Sprites/Cards/riposte.png";
    }

    protected override void SetUpContents(Dictionary<string, object> args)
    {
        var opponent = Battle.GetOpponentOf(OwnerEntity);
        Contents = new Dictionary<Enums.EngageRole, List<BaseSkillEffect>>()
        {
            {
                Enums.EngageRole.Attacker, new List<BaseSkillEffect>()
                {
                    new RiposteSkillEffect(opponent, Battle, this, Enums.HandTier.HighCard),
                }
            }
        };
    }
}