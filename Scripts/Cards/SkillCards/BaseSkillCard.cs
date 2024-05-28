using System.Collections.Generic;
using XCardGame.Scripts.Common.Constants;
using XCardGame.Scripts.Effects;

namespace XCardGame.Scripts.Cards.SkillCards;

public class BaseSkillCard: BaseCard
{
    public Dictionary<Enums.HandTier, BaseEffect> Effects;
    
    public BaseSkillCard(string name, string description, string iconPath, Enums.CardSuit suit, Enums.CardRank rank,
        Dictionary<Enums.HandTier, BaseEffect> effects) : base(name, description, iconPath, suit, rank)
    {
        Effects = effects;
    }
    
}