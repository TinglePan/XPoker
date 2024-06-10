using System.Collections.Generic;
using XCardGame.Scripts.Common;
using XCardGame.Scripts.Common.Constants;
using XCardGame.Scripts.GameLogic;

namespace XCardGame.Scripts.Cards.SkillCards;

public class TwoHandedSkillCard: BaseSkillCard
{
    
    public TwoHandedSkillCard(Enums.CardSuit suit, Enums.CardRank rank, BattleEntity owner) : 
        base("Two handed", "Adds up the ranks of each pair to attack damage", "res://Sprites/Cards/two_handed.png", suit, rank, 
            Enums.HandTier.TwoPairs, 0, -1, 0, null, null, null, owner)
    {
    }

    protected override void InnerBeforeDamageDealt(Attack attack)
    {
        base.InnerBeforeDamageDealt(attack);
        if (attack.Source == Owner)
        {
            var hand = attack.SourceHand;
            HashSet<Enums.CardRank> ranks = new HashSet<Enums.CardRank>();
            foreach (var card in hand.PrimaryCards)
            {
                ranks.Add(card.Rank.Value);
            }
            var extraDamage = 0;
            foreach (var rank in ranks)
            {
                extraDamage += Utils.GetCardRankValue(rank);
            }
            attack.ExtraDamages.Add((extraDamage, Name));
        }
    }
}