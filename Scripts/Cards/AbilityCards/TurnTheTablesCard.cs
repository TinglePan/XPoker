using System.Collections.Generic;
using System.Linq;
using Godot;
using XCardGame.Scripts.Buffs;
using XCardGame.Scripts.Common.Constants;
using XCardGame.Scripts.GameLogic;
using XCardGame.Scripts.HandEvaluate;

namespace XCardGame.Scripts.Cards.AbilityCards;

public class TurnTheTablesCard: BaseSealableAbilityCard
{
    public BattleEntity Source;
    public BattleEntity Target;
    
    public TurnTheTablesCard(GameMgr gameMgr, Enums.CardFace face, Enums.CardSuit suit, BattleEntity target,
        BattleEntity source=null, BattleEntity owner=null) : base(gameMgr, "Turn The Tables", 
        "When you proceed a round with this card faced up, you showdown with your opponent's hole cards, and vice versa.", 
        face, suit, "res://Sprites/Cards/turn_the_tables.png", 1, 0, true, owner)
    {
        source ??= owner;
        source ??= Battle.Player;
        Source = source;
        Target = target;
    }

    public override void BeforeEngage(Battle battle)
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