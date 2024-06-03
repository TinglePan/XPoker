using System.Collections.Generic;
using XCardGame.Scripts.Buffs;
using XCardGame.Scripts.Common.Constants;
using XCardGame.Scripts.Effects;
using XCardGame.Scripts.GameLogic;

namespace XCardGame.Scripts.Cards.SkillCards;

public class DualWieldSkillCard: BaseSkillCard
{
    public DualWieldSkillCard(Enums.CardSuit suit, Enums.CardRank rank, BattleEntity owner) : 
        base("Dual wield", "Attack twice", "res://Sprites/Cards/dual_wield.png", suit, rank, 
            Enums.HandTier.TwoPair, 0, -1, 1, null, null, null, owner)
    {
    }
}