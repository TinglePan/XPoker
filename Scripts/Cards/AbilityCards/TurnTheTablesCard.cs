using System.Collections.Generic;
using System.Linq;
using Godot;
using XCardGame.Scripts.Buffs;
using XCardGame.Scripts.Common.Constants;
using XCardGame.Scripts.Effects;
using XCardGame.Scripts.GameLogic;
using XCardGame.Scripts.HandEvaluate;

namespace XCardGame.Scripts.Cards.AbilityCards;

public class TurnTheTablesCard: BaseTapCard
{
    class TurnTheTablesEffect: BaseSingleTurnEffect
    {
        public TurnTheTablesEffect(string name, string description, string iconPath, BaseCard createdByCard) : base(name, description, iconPath, createdByCard)
        {
        }

        public override void OnStart(Battle battle)
        {
            CreatedByCard.Battle.BeforeEngage += BeforeEngage;
        }
        
        public override void OnStop(Battle battle)
        {
            CreatedByCard.Battle.BeforeEngage -= BeforeEngage;
        }

        protected void BeforeEngage(Battle battle)
        {
            var card = (TurnTheTablesCard)CreatedByCard;
            if (battle.RoundHandStrengths.ContainsKey(card.Source) && battle.RoundHandStrengths.ContainsKey(card.Target))
            {
                (battle.RoundHandStrengths[card.Source], battle.RoundHandStrengths[card.Target]) = 
                    (battle.RoundHandStrengths[card.Target], battle.RoundHandStrengths[card.Source]);
            }
        }
    }
    
    public BattleEntity Source;
    public BattleEntity Target;
    
    public TurnTheTablesCard(Enums.CardSuit suit, Enums.CardRank rank, int tappedCost, int unTappedCost) : base("Turn The Tables", 
        "When you proceed a round with this card faced up, you showdown with your opponent's hole cards, and vice versa.", 
        "res://Sprites/Cards/turn_the_tables.png", suit, rank, tappedCost, unTappedCost)
    {
    }

    public override void Setup(Dictionary<string, object> args)
    {
        base.Setup(args);
        Source = Battle.Player;
        Target = Battle.Entities.FirstOrDefault(entity => entity != Source);
        Effect = new TurnTheTablesEffect(Name, Description, IconPath.Value, this);
    }
}