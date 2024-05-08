using XCardGame.Scripts.Buffs;
using XCardGame.Scripts.Common.Constants;
using XCardGame.Scripts.GameLogic;

namespace XCardGame.Scripts.Cards.AbilityCards;

public class TurnTheTablesCard: BaseActiveAbilityCard
{
    public TurnTheTablesCard(GameMgr gameMgr, Enums.CardFace face, BattleEntity owner, int cost, int coolDown,
        bool isQuickAbility = false) : base(gameMgr, "Turn The Tables", 
        "Replace your hole cards with your opponent's, and vice versa.", face, owner,
        "res://Sprites/Cards/turn_the_tables.png", cost, coolDown, isQuickAbility)
    {
    }
    
    public override void Activate()
    {
        BattleEntity player = Battle.Player;
        player.InflictBuffOn(new TurnTheTablesBuff(GameMgr, player), player);
        AfterEffect?.Invoke();
    }
}