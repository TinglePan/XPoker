using XCardGame.Scripts.Cards;
using XCardGame.Scripts.GameLogic;
using XCardGame.Scripts.HandEvaluate;

namespace XCardGame.Scripts.Effects;

public class BaseAttackEffect: BaseEffect
{
    public Attack Attack;

    public BaseAttackEffect(string name, string description, string iconPath, BaseCard createdByCard, Attack attack) : 
        base(name, description, iconPath, createdByCard)
    {
        Attack = attack;
    }
}