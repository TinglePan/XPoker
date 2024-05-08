using XCardGame.Scripts.Common.DataBinding;
using XCardGame.Scripts.GameLogic;
using XCardGame.Scripts.HandEvaluate;

namespace XCardGame.Scripts.Buffs;

public class TurnTheTablesBuff: BaseTemporaryBuff
{
    public TurnTheTablesBuff(GameMgr gameMgr, BattleEntity entity, int duration=1) : base("Turn the tables", 
        "Replace your hole cards with your opponent's, and vice versa, for this round", 
        "res://Sprites/Icons/turn_the_tables.png", gameMgr, entity, duration)
    {
    }

    public override void OnInflicted(Battle battle)
    {
        battle.TurnedTables = true;
    }

    public override void OnExpired(Battle battle)
    {
        battle.TurnedTables = false;
    }
}