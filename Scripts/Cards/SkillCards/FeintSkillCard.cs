using System.Collections.Generic;
using XCardGame.Scripts.Buffs;
using XCardGame.Scripts.Common.Constants;
using XCardGame.Scripts.Effects;
using XCardGame.Scripts.GameLogic;

namespace XCardGame.Scripts.Cards.SkillCards;

public class FeintSkillCard: BaseSkillCard
{
    public FeintSkillCard(Enums.CardSuit suit, Enums.CardRank rank, BattleEntity owner) : 
        base("Feint", "Grants vulnerable instead of dealing damage", "res://Sprites/feint", suit, 
            rank, Enums.HandTier.HighCard, 0, 0, 0, null, null, null, owner)
    {
        BuffOpponent = new List<BaseBuff>()
        {
            new VulnerableDeBuff(1, Battle.GetOpponentOf(owner), owner, this)
        };
    }
}