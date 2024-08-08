using XCardGame.Common;

namespace XCardGame;

public class Attack
{
    public Battle Battle;
    public Engage Engage;
    public BattleEntity Attacker;
    public BattleEntity Defender;
    public int Value;
    
    public Attack(Battle battle, BattleEntity attacker, BattleEntity defender, int value)
    {
        Battle = battle;
        Engage = Battle.RoundEngage;
        Attacker = attacker;
        Defender = defender;
        Value = value;
    }
    
    public void Apply()
    {
        Battle.GameMgr.BattleLog.Log(Utils._($"{Attacker}'s attack dealt {Value} damage to {Defender}."));
        Defender.TakeDamage(Value);
    }
}