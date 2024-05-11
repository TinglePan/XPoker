using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using XCardGame.Scripts.Common.Constants;
using XCardGame.Scripts.GameLogic;
using XCardGame.Scripts.HandEvaluate;

namespace XCardGame.Scripts.Cards.AbilityCards;

public class NineOutsCard: BaseSealableCard
{
    public NineOutsCard(Enums.CardFace face, Enums.CardSuit suit, Enums.CardRank rank, int cost = 1,
        int coolDown = 0, bool isQuick = true, BattleEntity owner = null) : base("Nine Outs", 
        "Each face-down 9 which contributes to a winning hand adds 9 more damage to attack triggered by that hand.",
        "res://Sprites/Cards/nine_outs.png", face, suit, rank, cost, coolDown, isQuick, owner)
    {
        
    }

    public override void Setup(Dictionary<string, object> args)
    {
        base.Setup(args);
        Battle.BeforeApplyDamage += BeforeApplyDamage;
    }
    
    ~NineOutsCard()
    {
        Battle.BeforeApplyDamage -= BeforeApplyDamage;
    }

    private void BeforeApplyDamage(Battle battle, AttackObj obj)
    {
        if (obj.IsWinByOuts())
        {
            foreach (var card in battle.CommunityCards)
            {
                if (card is PokerCard pokerCard && card.Face.Value == Enums.CardFace.Down &&
                    pokerCard.Rank.Value == Enums.CardRank.Nine)
                {
                    obj.ExtraDamages.TryAdd(Name, 0);
                    obj.ExtraDamages[Name] += 9;
                }
            }
            if (obj.ExtraDamages.ContainsKey(Name))
            {
                AfterEffect();
            }
        }
    }
}