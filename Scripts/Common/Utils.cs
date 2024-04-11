using System.Collections.Generic;
using System.Linq;
using Godot;

namespace XCardGame.Scripts.Common;

public static class Utils
{
    public static Node InstantiatePrefab(PackedScene prefab, Node parent)
    {
        var node = prefab.Instantiate<Node>();
        parent.AddChild(node);
        return node;
    }

    public static List<List<T>> GetCombinations<T>(List<T> source, int m)
    {
        void Helper(int start, List<T> pick, List<List<T>> picks)
        {
            if (pick.Count == m)
            {
                picks.Add(new List<T>(pick));
                return;
            }
        
            for (int i = start; i < source.Count; i++)
            {
                var element = source[i];
                pick.Add(element);
                Helper(i + 1, pick, picks);
                pick.Remove(element);
            }
        }
        var res = new List<List<T>>();
        Helper(0, new List<T>(), res);
        return res;
    }

    public static List<List<T>> GetCombinationsWithXFromA<T>(List<T> a, List<T> b, int m, int x)
    {
        var picksFromA = Utils.GetCombinations(a, x);
        var picksFromB = Utils.GetCombinations(b, m - x);
        var res = new List<List<T>>();
        foreach (var pickHoleCards in picksFromA)
        {
            foreach (var pickCommunityCards in picksFromB)
            {
                res.Add(pickHoleCards.Concat(pickCommunityCards).ToList());
            }
        }
        return res;
    }
    
    public static List<List<T>> GetCombinationsWithXToYFromA<T>(List<T> a, List<T> b, int m, int x, int y)
    {
        var res = new List<List<T>>();
        for (int i = x; i <= y; i++)
        {
            res.AddRange(GetCombinationsWithXFromA(a, b, m, i));
        }
        return res;
    }
}