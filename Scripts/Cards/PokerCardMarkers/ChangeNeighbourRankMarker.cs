using System;
using XCardGame.Scripts.Common.Constants;
using XCardGame.Scripts.GameLogic;

namespace XCardGame.Scripts.Cards.PokerCards;

public class ChangeNeighbourRankMarker: RevealTriggeredPokerCard
{
    
    public Enums.Direction1D TargetDirection;
    public Func<Enums.CardRank, Enums.CardRank, Enums.CardRank> RankChangeFunc;
    
    public ChangeNeighbourRankMarker(Enums.CardSuit cardSuit, Enums.CardFace face, Enums.CardRank rank,
        Enums.Direction1D targetDirection, Func<Enums.CardRank, Enums.CardRank, Enums.CardRank> rankChangeFunc,
        BattleEntity owner = null, bool suitAsSecondComparer = false) : base(cardSuit, face, rank, owner, suitAsSecondComparer)
    {
        TargetDirection = targetDirection;
        RankChangeFunc = rankChangeFunc;
    }
    
    public override void OnReveal()
    {
        var index = Node.Container.Cards.IndexOf(this);
        switch (TargetDirection)
        {
            case Enums.Direction1D.Left:
                ChangeNeighbourRank(index - 1);
                break;
            case Enums.Direction1D.Right:
                ChangeNeighbourRank(index + 1);
                break;
            case Enums.Direction1D.None:
                ChangeNeighbourRank(index - 1);
                ChangeNeighbourRank(index + 1);
                break;
        }
    }
    
    protected void ChangeNeighbourRank(int targetIndex)
    {
        if (targetIndex >= 0 && targetIndex < Node.Container.Cards.Count)
        {
            if (Node.Container.Cards[targetIndex] is PokerCard targetCard)
            {
                targetCard.Rank.Value = RankChangeFunc(targetCard.Rank.Value, Rank.Value);
            }
        }
    }
    
}