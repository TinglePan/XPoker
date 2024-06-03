using System.Collections.Generic;
using XCardGame.Scripts.Buffs;
using XCardGame.Scripts.Common;
using XCardGame.Scripts.Common.Constants;
using XCardGame.Scripts.Effects;
using XCardGame.Scripts.GameLogic;

namespace XCardGame.Scripts.Cards.SkillCards;

public class RiposteSkillCard: BaseSkillCard
{
    public RiposteSkillCard(Enums.CardSuit suit, Enums.CardRank rank, BattleEntity owner, Enums.HandTier tier) : 
        base("Riposte", "Grants vulnerable instead of dealing damage", "res://Sprites/feint", suit, 
            rank, tier, 0, 0, 0, null, null, null, owner)
    {
        BuffSelf = new List<BaseBuff>()
        {
            new RiposteBuff(TriggerHandTier, Utils.GetCardRankValue(Rank.Value), 1, 
                Battle.GetOpponentOf(owner), owner, this)
        };
    }
}