using System.Collections.Generic;
using System.Linq;
using Godot;
using XCardGame.Scripts.Buffs;
using XCardGame.Scripts.Common.Constants;
using XCardGame.Scripts.GameLogic;
using XCardGame.Scripts.HandEvaluate;

namespace XCardGame.Scripts.Cards.AbilityCards;

public class TurnTheTablesCard: BasePassiveCard
{
    public BattleEntity Source;
    public BattleEntity Target;
    
    public TurnTheTablesCard(Enums.CardFace face, Enums.CardSuit suit, Enums.CardRank rank, int cost = 1,
        int coolDown = 0, bool isQuick = true, BattleEntity owner = null) : base("Turn The Tables", 
        "When you proceed a round with this card faced up, you showdown with your opponent's hole cards, and vice versa.", 
        "res://Sprites/Cards/turn_the_tables.png", face, suit, rank, cost, coolDown, isQuick, owner)
    {
    }

    public override void Setup(Dictionary<string, object> args)
    {
        base.Setup(args);
        Source = args.TryGetValue("source", out var arg) ? (BattleEntity) arg : Battle.Player;
        Target = (BattleEntity)args["target"];
    }

    public override void OnAppear(Battle battle)
    {
        base.OnAppear(battle);
        Battle.BeforeEngage += BeforeEngage;
    }

    public override void OnDisappear(Battle battle)
    {
        base.OnDisappear(battle);
        Battle.BeforeEngage -= BeforeEngage;
    }

    private void BeforeEngage(Battle battle)
    {
        if (Face.Value == Enums.CardFace.Up)
        {
            if (battle.RoundHandStrengths.ContainsKey(Source) && battle.RoundHandStrengths.ContainsKey(Target))
            {
                (battle.RoundHandStrengths[Source], battle.RoundHandStrengths[Target]) = 
                    (battle.RoundHandStrengths[Target], battle.RoundHandStrengths[Source]);
            }
            if (battle.RoundHandStrengthsWithoutFaceDownCards.ContainsKey(Source) && 
                battle.RoundHandStrengthsWithoutFaceDownCards.ContainsKey(Target))
            {
                (battle.RoundHandStrengthsWithoutFaceDownCards[Source], 
                    battle.RoundHandStrengthsWithoutFaceDownCards[Target]) = 
                    (battle.RoundHandStrengthsWithoutFaceDownCards[Target], 
                        battle.RoundHandStrengthsWithoutFaceDownCards[Source]);
            }
            AfterEffect();
            Disposal();
        }
    }
}