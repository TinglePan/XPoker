using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Godot;

namespace XCardGame.Common;

public static class Utils
{
    public static string _(string text)
    {
        return text;
    }
    
    public static string GetPersonalPronoun(BattleEntity who, BattleEntity self, BattleEntity target, bool capitalize = false)
    {
        string res = who.Def.Name;
        if (who == self) res = _("user");
        if (who == target) res = _("target");
        if (capitalize) res = res.Capitalize();
        return res;
    }

    public static Vector2 AddUpSeparatedMultipliers(List<float> multipliers)
    {
        float positiveMultiplier = 1;
        float negativeMultiplier = 1;
        foreach (var multiplier in multipliers)
        {
            if (multiplier > 1)
            {
                positiveMultiplier += multiplier - 1;
            }
            else
            {
                negativeMultiplier *= multiplier;
            }
        }
        return new Vector2(positiveMultiplier, negativeMultiplier);
    }

    public static async Task Wait(Node node, float time)
    {
        Debug.Assert(GodotObject.IsInstanceValid(node));
        var timer = node.GetTree().CreateTimer(time);
        await node.ToSignal(timer, Timer.SignalName.Timeout);
    }
    
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
        // GD.Print($"GetCombinationsWithXFromA: {a.Count}/{b.Count}{m}/{x} -> {res.Count}");
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

    public static int RandOnWeight(List<int> weights, Random rand)
    {
        var totalWeight = weights.Sum();
        var randValue = rand.Next(0, totalWeight);
        for (int i = 0; i < weights.Count; i++)
        {
            if (randValue < weights[i])
            {
                return i;
            }
            randValue -= weights[i];
        }
        return -1;
    }

    public static List<T> RandMFrom<T>(List<T> source, int m, Random rand)
    {
        if (m < source.Count)
        {
            var tmp = new List<T>(source);
            var res = new List<T>();
            var n = tmp.Count;
            for (int i = 0; i < m; i++)
            {
                var randValue = rand.Next(n - i);
                var pick = tmp[randValue];
                res.Add(pick);
                (tmp[randValue], tmp[n - i - 1]) = (tmp[n - i - 1], tmp[randValue]);
            }
            return res;
        }
        else
        {
            return source;
        }
    }

    public static int RandOnThresholds(List<int> thresholds, Random rand)
    {
        int max = thresholds.Max();
        var randValue = rand.Next(0, max);
        for (int i = 0; i < thresholds.Count; i++)
        {
            if (randValue < thresholds[i])
            {
                return i;
            }
        }
        return thresholds.Count - 1;
    }

    public static int RandOnOdds(List<int> odds, Random rand)
    {
        return RandOnThresholds(Odds2Thresholds(odds), rand);
    }

    public static List<int> Odds2Thresholds(List<int> odds)
    {
        var res = new List<int>();
        var sum = 0;
        foreach (var odd in odds)
        {
            sum += odd;
            res.Add(sum);
        }
        return res;
    }

    public static string PrettyPrintCardSuit(Enums.CardSuit suit)
    {
        switch (suit)
        {
            case Enums.CardSuit.Clubs:
                return _("♣");
            case Enums.CardSuit.Diamonds:
                return _("♦");
            case Enums.CardSuit.Hearts:
                return _("♥");
            case Enums.CardSuit.Spades:
                return _("♠");
        }
        return "";
    }

    public static string PrettyPrintHandTier(Enums.HandTier tier)
    {
        switch (tier)
        {
            case Enums.HandTier.HighCard:
                return "High Card";
            case Enums.HandTier.Pair:
                return "Pair";
            case Enums.HandTier.TwoPairs:
                return "Two Pairs";
            case Enums.HandTier.ThreeOfAKind:
                return "Three of a Kind";
            case Enums.HandTier.Straight:
                return "Straight";
            case Enums.HandTier.Flush:
                return "Flush";
            case Enums.HandTier.FullHouse:
                return "Full House";
            case Enums.HandTier.Quads:
                return "Quads";
            case Enums.HandTier.StraightFlush:
                return "Straight Flush"; 
            case Enums.HandTier.RoyalFlush:
                return "Royal Flush";
        }
        return "Unknown";
    }

    public static string GetPercentageString(float value)
    {
        return _($"{Mathf.RoundToInt(value * 100)}%");
    }

    public static Enums.CardColor GetCardSuitColor(Enums.CardSuit suit)
    {
        return suit switch
        {
            Enums.CardSuit.Spades => Enums.CardColor.Black,
            Enums.CardSuit.Clubs => Enums.CardColor.Black,
            Enums.CardSuit.Hearts => Enums.CardColor.Red,
            Enums.CardSuit.Diamonds => Enums.CardColor.Red,
            _ => Enums.CardColor.None
        };
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

    public static int[] GetCardBlackJackValues(Enums.CardRank rank)
    {
        switch (rank)
        {
            case Enums.CardRank.Ace:
                return new [] { 1, 11 };
            case Enums.CardRank.Two:
            case Enums.CardRank.Three:
            case Enums.CardRank.Four:
            case Enums.CardRank.Five:
            case Enums.CardRank.Six:
            case Enums.CardRank.Seven:
            case Enums.CardRank.Eight:
            case Enums.CardRank.Nine:
            case Enums.CardRank.Ten:
                return new[] { GetCardRankValue(rank) };
            case Enums.CardRank.Jack:
            case Enums.CardRank.Queen:
            case Enums.CardRank.King:
            case Enums.CardRank.Joker:
                return new[] { 10 };
            default:
                return null;
        }
    }

    public static int GetCardBlackJackValue(Enums.CardRank rank)
    {
        
        switch (rank)
        {
            case Enums.CardRank.Ace:
                return 11;
            case Enums.CardRank.Two:
            case Enums.CardRank.Three:
            case Enums.CardRank.Four:
            case Enums.CardRank.Five:
            case Enums.CardRank.Six:
            case Enums.CardRank.Seven:
            case Enums.CardRank.Eight:
            case Enums.CardRank.Nine:
            case Enums.CardRank.Ten:
                return GetCardRankValue(rank);
            case Enums.CardRank.Jack:
            case Enums.CardRank.Queen:
            case Enums.CardRank.King:
                return 10;
            case Enums.CardRank.Joker:
                return 20;
            default:
                return 0;
        }
    }
    
    
    public static Enums.CardRank GetCardRank(int value)
    {
        switch (value)
        {
            case <= 1:
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
            default:
                return Enums.CardRank.King;
        }
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