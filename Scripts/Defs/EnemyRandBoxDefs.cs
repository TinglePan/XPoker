using System;
using System.Collections.Generic;
using System.Numerics;
using System.Reflection;
using XCardGame.Scripts.Common;
using XCardGame.Scripts.Defs.Def;
using XCardGame.Scripts.Defs.Def.BattleEntity;

namespace XCardGame.Scripts.Defs;

public static class EnemyRandBoxDefs
{
    public static readonly EnemyRandBoxDef StandardEnemyRandBoxDef = new EnemyRandBoxDef
    {
        ProgressRange = new Vector2(0, 4),
        RandBox = new RandBox<BattleEntityDef>(80)
        {
            ContentWeights = new List<(BattleEntityDef, int)>
            {
                (BattleEntityDefs.TallBoyBattleEntityDef, 100),
            }
        }
    };
    
    public static readonly EnemyRandBoxDef MidGameEnemyRandBoxDef = new EnemyRandBoxDef
    {
        ProgressRange = new Vector2(3, 6),
        RandBox = new RandBox<BattleEntityDef>(100)
        {
            ContentWeights = new List<(BattleEntityDef, int)>
            {
                (BattleEntityDefs.TallBoyBattleEntityDef, 100),
                (BattleEntityDefs.AssassinBattleEntityDef, 100),
                (BattleEntityDefs.NinjaBattleEntityDef, 100),
                (BattleEntityDefs.ManInBlackBattleEntityDef, 100),
            }
        }
    };
    
    public static readonly EnemyRandBoxDef LateGameEnemyRandBoxDef = new EnemyRandBoxDef
    {
        ProgressRange = new Vector2(5, 7),
        RandBox = new RandBox<BattleEntityDef>(50)
        {
            ContentWeights = new List<(BattleEntityDef, int)>
            {
                (BattleEntityDefs.AssassinBattleEntityDef, 100),
                (BattleEntityDefs.NinjaBattleEntityDef, 100),
                (BattleEntityDefs.ManInBlackBattleEntityDef, 100),
            }
        }
    };
    
    public static List<EnemyRandBoxDef> All()
    {
        Type y = typeof(EnemyRandBoxDefs);
        var res = new List<EnemyRandBoxDef>();
        FieldInfo[] staticFields = y.GetFields(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
        foreach (FieldInfo field in staticFields)
        {
            var value = field.GetValue(null);
            var boxDef = (EnemyRandBoxDef)value;
            res.Add(boxDef);
        }
        return res;
    }
    
}