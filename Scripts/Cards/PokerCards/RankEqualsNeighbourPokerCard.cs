using System.Diagnostics;
using XCardGame.Scripts.Common.Constants;
using XCardGame.Scripts.GameLogic;

namespace XCardGame.Scripts.Cards.PokerCards;

public class RankEqualsNeighbourPokerCard: RevealTriggeredPokerCard
{
    
    public Enums.Direction1D TargetDirection;
    
    public RankEqualsNeighbourPokerCard(Enums.CardSuit cardSuit, Enums.CardFace face, Enums.CardRank rank,
        Enums.Direction1D targetDirection, BattleEntity owner = null, bool suitAsSecondComparer = false) : 
        base(cardSuit, face, rank, owner, suitAsSecondComparer)
    {
        TargetDirection = targetDirection;
    }

    public override void OnReveal()
    {
        Debug.Assert(Battle != null);
        var index = Node.Container.Cards.IndexOf(this);
        int targetIndex = -1;
        switch (TargetDirection)
        {
            case Enums.Direction1D.Left:
                targetIndex = index - 1;
                break;
            case Enums.Direction1D.Right:
                targetIndex = index + 1;
                break;
        }
        if (targetIndex >= 0 && targetIndex < Node.Container.Cards.Count)
        {
            if (Node.Container.Cards[targetIndex] is BasePokerCard targetCard)
            {
                Rank.Value = targetCard.Rank.Value;
            }
        }
    }
}