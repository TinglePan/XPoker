using System.Collections.Generic;
using System.Linq;
using Godot;

namespace XCardGame.Scripts.Common;

public static class Profile
{
    
    private static Dictionary<string, long> _watched = new ();
    private static Dictionary<string, List<long>> _res = new();

    public static void Reset(string tag)
    {
        _watched.Remove(tag);
        _res.Remove(tag);
    }
    
    public static void StartWatch(string tag)
    {
        _watched[tag] = (long)Time.GetTicksMsec();
    }

    public static void EndWatch(string tag, bool printOnEnd = false, int printEvery = 0)
    {
        if (!_watched.ContainsKey(tag)) return;
        var start = _watched[tag];
        var end = (long)Time.GetTicksMsec();
        var diff = end - start;
        if (!_res.ContainsKey(tag))
        {
            _res[tag] = new List<long>();
        }
        _res[tag].Add(diff);
        if (printOnEnd)
        {
            if (printEvery > 0)
            {
                PrintResultsEvery(tag, printEvery);
            }
            else
            {
                PrintResults(tag);
            }
        }
    }
    
    public static void PrintResults(string tag)
    {
        if (!_res.ContainsKey(tag)) return;
        var res = _res[tag];
        var avg = res.Average();
        var min = res.Min();
        var max = res.Max();
        GD.Print($"Profile: {tag} - avg: {avg}ms of {res.Count} samples, min: {min}ms, max: {max}ms");
    }

    public static void PrintResultsEvery(string tag, int n)
    {
        if (!_res.ContainsKey(tag)) return;
        var res = _res[tag];
        if (res.Count % n == 0) PrintResults(tag);
    }
}