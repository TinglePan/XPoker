using XCardGame.Scripts.Common.Constants;
using XCardGame.Scripts.Defs;
using XCardGame.Scripts.GameLogic;

namespace XCardGame.Scripts.Cards.AbilityCards;

public class BaseAbilityCard: BaseCard
{

    public readonly AbilityCardDef AbilityCardDef;
    
    public BaseAbilityCard(AbilityCardDef def): base(def)
    {
        AbilityCardDef = def;
    }
}