using System.Collections.Generic;

namespace XCardGame.Scripts.Defs;

public static class Thresholds
{
    public static Dictionary<int, List<int>> RarityThresholdAtProgress = new ()
    {
        { 1, new List<int> { 80, 95, 99, 100 } },
        { 2, new List<int> { 65, 85, 95, 100 } },
        
        { 3, new List<int> { 45, 80, 90, 100 } },
        { 4, new List<int> { 20, 65, 85, 100 } },
        { 5, new List<int> { 20, 50, 80, 100 } },
        
        { 6, new List<int> { 20, 45, 80, 100 } },
        { 7, new List<int> { 10, 35, 80, 100 } },
    };
}