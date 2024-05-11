using XCardGame.Scripts.Common.Constants;
using XCardGame.Scripts.GameLogic;

namespace XCardGame.Scripts.Cards;

public class BaseStateCard: BaseSealableCard
{
    public BaseStateCard(string name, string description, string iconPath, Enums.CardFace face, Enums.CardSuit suit, Enums.CardRank rank, int cost, int sealDuration, bool isQuick = true, BattleEntity owner = null) : base(name, description, iconPath, face, suit, rank, cost, sealDuration, isQuick, owner)
    {
    }
}