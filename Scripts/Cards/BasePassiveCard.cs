using System.Collections.Generic;
using XCardGame.Scripts.Common.Constants;
using XCardGame.Scripts.GameLogic;

namespace XCardGame.Scripts.Cards;

public class BasePassiveCard: BaseSealableCard
{
    public BasePassiveCard(string name, string description, string iconPath, Enums.CardFace face, Enums.CardSuit suit, Enums.CardRank rank, int cost, int sealDuration, bool isQuick = true, BattleEntity owner = null) : base(name, description, iconPath, face, suit, rank, cost, sealDuration, isQuick, owner)
    {
    }

    public override void Setup(Dictionary<string, object> args)
    {
        base.Setup(args);
        Battle.OnRoundStart += OnRoundStart;
    }

    protected void OnRoundStart(Battle battle)
    {
        if (Face.Value == Enums.CardFace.Up && !IsNegated.Value)
        {
            OnAppear(battle);
        }
    }
}