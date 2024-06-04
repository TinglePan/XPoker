using System.Collections.Generic;
using System.Linq;
using Godot;
using XCardGame.Scripts.Common.Constants;

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
        var picksFromA = GetCombinations(a, x);
        var picksFromB = GetCombinations(b, m - x);
        var res = new List<List<T>>();
        foreach (var aPicks in picksFromA)
        {
            foreach (var bPicks in picksFromB)
            {
                res.Add(aPicks.Concat(bPicks).ToList());
            }
        }
        GD.Print($"GetCombinationsWithXFromA: {a.Count}/{b.Count}{m}/{x} -> {res.Count}");
        return res;
    }
    
    public static List<List<T>> 
        GetCombinationsWithXToYFromA<T>(List<T> a, List<T> b, int m, int x, int y)
    {
        // Profile.StartWatch("get comb");
        var res = new List<List<T>>();
        for (int i = x; i <= y; i++)
        {
            res.AddRange(GetCombinationsWithXFromA(a, b, m, i));
        }
        // Profile.EndWatch("get comb", true, 0);
        // GD.Print($"combination count: {res.Count}");
        return res;
    }

    public static string PrettyPrintCardSuit(Enums.CardSuit suit)
    {
        switch (suit)
        {
            case Enums.CardSuit.Clubs:
                return "♣";
            case Enums.CardSuit.Diamonds:
                return "♦";
            case Enums.CardSuit.Hearts:
                return "♥";
            case Enums.CardSuit.Spades:
                return "♠";
        }
        return "";
    }

    public static string GetPercentageString(float value)
    {
        return $"{Mathf.RoundToInt(value * 100)}%";
    }

    public static int GetCardRankValue(Enums.CardRank rank)
    {
        switch (rank)
        {
            case Enums.CardRank.Ace:
                return 1;
            case Enums.CardRank.Two:
            case Enums.CardRank.Three:
            case Enums.CardRank.Four:
            case Enums.CardRank.Five:
            case Enums.CardRank.Six:
            case Enums.CardRank.Seven:
            case Enums.CardRank.Eight:
            case Enums.CardRank.Nine:
            case Enums.CardRank.Ten:
            case Enums.CardRank.Jack:
            case Enums.CardRank.Queen:
            case Enums.CardRank.King:
                return (int)rank;
        }
        return 0;
    }
    
    public static Enums.CardRank GetCardRank(int value)
    {
        switch (value)
        {
            case 1:
                return Enums.CardRank.Ace;
            case 2:
                return Enums.CardRank.Two;
            case 3:
                return Enums.CardRank.Three;
            case 4:
                return Enums.CardRank.Four;
            case 5:
                return Enums.CardRank.Five;
            case 6:
                return Enums.CardRank.Six;
            case 7:
                return Enums.CardRank.Seven;
            case 8:
                return Enums.CardRank.Eight;
            case 9:
                return Enums.CardRank.Nine;
            case 10:
                return Enums.CardRank.Ten;
            case 11:
                return Enums.CardRank.Jack;
            case 12:
                return Enums.CardRank.Queen;
            case 13:
                return Enums.CardRank.King;
        }
        return Enums.CardRank.None;
    }

    public static Texture2D GetCardSuitTexture(Enums.CardSuit suit)
    {
        var texturePath = GetCardTexturePath(suit);
        return texturePath != null ? ResourceCache.Instance.Load<Texture2D>(texturePath) : null;
    }
    
    public static string GetCardTexturePath(Enums.CardSuit suit)
    {
        switch (suit)
        {
            case Enums.CardSuit.Clubs:
                return "res://Sprites/Suits/clubs.png";
            case Enums.CardSuit.Diamonds:
                return "res://Sprites/Suits/diamonds.png";
            case Enums.CardSuit.Hearts:
                return "res://Sprites/Suits/hearts.png";
            case Enums.CardSuit.Spades:
                return "res://Sprites/Suits/spades.png";
        }
        return null;
    }

    public static string PrettyPrintCardRank(Enums.CardRank rank)
    {
        switch (rank)
        {
            case Enums.CardRank.Two:
            case Enums.CardRank.Three:
            case Enums.CardRank.Four:
            case Enums.CardRank.Five:
            case Enums.CardRank.Six:
            case Enums.CardRank.Seven:
            case Enums.CardRank.Eight:
            case Enums.CardRank.Nine:
            case Enums.CardRank.Ten:
                return ((int)rank).ToString();
            case Enums.CardRank.Jack:
                return "J";
            case Enums.CardRank.Queen:
                return "Q";
            case Enums.CardRank.King:
                return "K";
            case Enums.CardRank.Ace:
                return "A";
            default:
                return "_";
        }
    }
    
    public static int RoundToNearest(int number, int n)
    {
        return (int)Mathf.Round((float)number / n) * n;
    }
}