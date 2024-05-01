using XCardGame.Scripts.Common.Constants;
using XCardGame.Scripts.GameLogic;

namespace XCardGame.Scripts.Cards.AbilityCards;

public class BasePassiveAbilityCard: BaseAbilityCard
{
    public BasePassiveAbilityCard(GameMgr gameMgr, string name, string description, Enums.CardFace face, BattleEntity owner, string iconPath) : base(gameMgr, name, description, face, owner, iconPath)
    {
    }
}