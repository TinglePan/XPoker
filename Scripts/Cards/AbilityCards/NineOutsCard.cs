using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using XCardGame.Scripts.Cards.PokerCards;
using XCardGame.Scripts.Common.Constants;
using XCardGame.Scripts.GameLogic;
using XCardGame.Scripts.HandEvaluate;

namespace XCardGame.Scripts.Cards.AbilityCards;

public class NineOutsCard: BaseSealableAbilityCard
{
    public NineOutsCard(GameMgr gameMgr, Enums.CardFace face, Enums.CardSuit suit, BattleEntity owner=null) : base(gameMgr, "Nine Outs", 
        "Each face-down 9 which contributes to a winning hand adds 9 more damage to attack triggered by that hand.", 
        face, suit, "res://Sprites/Cards/nine_outs.png", 1, 0, true, owner)
    {
    }

    public override void BeforeEngage(Battle battle)
    {
        
    }
    
    public override void BeforeApplyDamage(Battle battle, AttackObj obj)
    {
        if (obj.IsWinByOuts())
        {
            foreach (var card in battle.CommunityCards)
            {
                if (card is BasePokerCard pokerCard && card.Face.Value == Enums.CardFace.Down &&
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