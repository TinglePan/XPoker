using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Linq;
using XCardGame.Scripts.Common.Constants;
using XCardGame.Scripts.Effects;
using XCardGame.Scripts.GameLogic;
using XCardGame.Scripts.HandEvaluate;

namespace XCardGame.Scripts.Cards.AbilityCards;

public class NineOutsCard: BaseTapCard
{

    class NineOutsEffect: BaseSingleTurnEffect
    {
        public NineOutsEffect(string name, string description, string iconPath, BaseCard createdByCard) : base(name, description, iconPath, createdByCard)
        {
        }

        public override void OnStart(Battle battle)
        {
            base.OnStart(battle);
            battle.BeforeApplyDamage += BeforeApplyDamage;
        }
        
        public override void OnStop(Battle battle)
        {
            base.OnStop(battle);
            battle.BeforeApplyDamage -= BeforeApplyDamage;
        }

        protected void BeforeApplyDamage(Battle battle, Attack attack)
        {
            if (attack.IsWinByOuts())
            {
                int counter = 0;
                foreach (var card in battle.CommunityCardContainer.Contents)
                {
                    if (card is MarkerCard pokerCard && card.Node.FaceDirection == Enums.CardFace.Down &&
                        pokerCard.Rank.Value == Enums.CardRank.Nine)
                    {
                        counter++;
                        switch (counter)
                        {
                            case 1:
                                attack.ExtraDamages.TryAdd($"{Name} 1", 9);
                                break;
                            case 2:
                                attack.ExtraMultipliers.TryAdd("{Name} 2", 9);
                                break;
                            case 3:
                                attack.ExtraDamages.TryAdd("{Name} 3", 999);
                                break;
                        }
                    }
                }
            }
        }
    }
    
    public NineOutsCard(Enums.CardSuit suit, Enums.CardRank rank, int tapCost, int unTapCost) : base("Nine Outs", 
        "Outs are face-down cards that contributes to a winning hand. The first out of 9 adds 9 more damage to resulting attack. The second out of 9 times the raw damage of resulting attack by 9. The third out of 9 is an instant death.",
        "res://Sprites/Cards/nine_outs.png", suit, rank, tapCost, unTapCost)
    {
        
    }
    
    public override void Setup(Dictionary<string, object> args)
    {
        base.Setup(args);
        Effect = new NineOutsEffect(Name, Description, IconPath.Value, this);
    }

    public override void OnStop(Battle battle)
    {
        base.OnStop(battle);
        battle.StopEffect(Effect);
    }
}