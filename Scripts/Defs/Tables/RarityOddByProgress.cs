using System.Collections.Generic;

namespace XCardGame.Scripts.Defs.Tables;

public static class RarityOddByProgress
{
    public static Dictionary<int, List<int>> Content = new ()
    {
        { 1, new List<int> { 80, 15, 4, 1 } },
        { 2, new List<int> { 65, 25, 8, 2 } },
        { 3, new List<int> { 50, 35, 12, 3 } },
        { 4, new List<int> { 35, 40, 20, 5 } },
        { 5, new List<int> { 20, 40, 30, 10 } },
        { 6, new List<int> { 10, 30, 40, 20 } },
        { 7, new List<int> { 10, 20, 40, 30 } },
    };
}