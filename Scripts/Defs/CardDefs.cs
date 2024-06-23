using System;
using System.Collections.Generic;
using System.Reflection;
using XCardGame.Scripts.Common.Constants;

namespace XCardGame.Scripts.Defs;

public class BaseCardDef
{
    public string Name;
    public string ConcreteClassPath;
    public string DescriptionTemplate;
    public string IconPath;
    public Enums.CardSuit Suit;
    public Enums.CardRank Rank;
    public int BasePrice;
    public int Rarity;
}

public class SkillCardDef : BaseCardDef
{
    public int Cost;
}

public class AbilityCardDef: BaseCardDef
{
    public int Cost;
}

public class TapCardDef: AbilityCardDef
{
    public int TappedCost;
}

public class UseCardDef: AbilityCardDef
{
    public int RankChangePerUse;
}

public static class CardDefs
{
    public static UseCardDef D6 = new ()
    {
        Name = "D6",
        ConcreteClassPath = "AbilityCards.D6Card",
        DescriptionTemplate = "Reroll your destiny.",
        IconPath = "res://Sprites/Cards/d6.png",
        RankChangePerUse = -1,
        Cost = 5,
        BasePrice = 5,
        Rarity = 1,
        Rank = Enums.CardRank.Six,
        Suit = Enums.CardSuit.Diamonds
    };
    
    public static TapCardDef Capitalism = new ()
    {
        Name = "Capitalism",
        ConcreteClassPath = "AbilityCards.CapitalismCard",
        DescriptionTemplate = "Add hole cards dealt each round.",
        IconPath = "res://Sprites/Cards/capitalism.png",
        TappedCost = 2,
        Cost = 0,
        BasePrice = 5,
        Rarity = 1,
        Rank = Enums.CardRank.Ace,
        Suit = Enums.CardSuit.Diamonds
    };
    
    public static TapCardDef Socialism = new ()
    {
        Name = "Socialism",
        ConcreteClassPath = "AbilityCards.SocialismCard",
        DescriptionTemplate = "Add community cards dealt each round.",
        IconPath = "res://Sprites/Cards/socialism.png",
        TappedCost = 2,
        Cost = 0,
        BasePrice = 5,
        Rarity = 1,
        Rank = Enums.CardRank.Ace,
        Suit = Enums.CardSuit.Clubs
    };
    
    public static UseCardDef MagicalHat = new ()
    {
        Name = "Magical hat",
        ConcreteClassPath = "AbilityCards.MagicalHatCard",
        DescriptionTemplate = "Swap two cards in hole card area or community card area.",
        IconPath = "res://Sprites/Cards/magical_hat.png",
        RankChangePerUse = -1,
        Cost = 5,
        BasePrice = 5,
        Rarity = 1,
        Rank = Enums.CardRank.Three,
        Suit = Enums.CardSuit.Spades
    };
    
    public static UseCardDef Balatroll = new ()
    {
        Name = "Balatroll",
        ConcreteClassPath = "AbilityCards.BalatrollCard",
        DescriptionTemplate = "Discard and redraw.",
        IconPath = "res://Sprites/Cards/balatroll.png",
        RankChangePerUse = -1,
        Cost = 4,
        BasePrice = 4,
        Rarity = 1,
        Rank = Enums.CardRank.Three,
        Suit = Enums.CardSuit.Hearts
    };
    
    public static TapCardDef BigShield = new ()
    {
        Name = "Big shield",
        ConcreteClassPath = "AbilityCards.BigShieldCard",
        DescriptionTemplate = "Deal and receive no damage for this round",
        IconPath = "res://Sprites/Cards/big_shield.png",
        TappedCost = 0,
        Cost = 5,
        BasePrice = 5,
        Rarity = 1,
        Rank = Enums.CardRank.King,
        Suit = Enums.CardSuit.Hearts
    };
    
    public static TapCardDef KeepOut = new ()
    {
        Name = "Keep out",
        ConcreteClassPath = "AbilityCards.KeepOutCard",
        DescriptionTemplate = "Certain cards do not count, the rule alters each round.",
        IconPath = "res://Sprites/Cards/keep_out.png",
        TappedCost = 2,
        Cost = 0,
        BasePrice = 5,
        Rarity = 1,
        Rank = Enums.CardRank.King,
        Suit = Enums.CardSuit.Spades
    };
    
    public static UseCardDef MillenniumEye = new ()
    {
        Name = "Millennium eye",
        ConcreteClassPath = "AbilityCards.MillenniumEyeCard",
        DescriptionTemplate = "All knowing at the cost of all power.",
        IconPath = "res://Sprites/Cards/millennium_eye.png",
        RankChangePerUse = -1,
        Cost = 5,
        BasePrice = 5,
        Rarity = 1,
        Rank = Enums.CardRank.Ace,
        Suit = Enums.CardSuit.Spades
    };
    
    public static TapCardDef TheTieBreaker = new ()
    {
        Name = "The tie breaker",
        ConcreteClassPath = "AbilityCards.TheTieBreakerCard",
        DescriptionTemplate = "Card suit is used to break a tie, suit order from high to low: Spades, Hearts, Clubs, Diamonds",
        IconPath = "res://Sprites/Cards/the_tie_breaker.png",
        TappedCost = 2,
        Cost = 0,
        BasePrice = 5,
        Rarity = 1,
        Rank = Enums.CardRank.Ace,
        Suit = Enums.CardSuit.Spades
    };
    
    public static UseCardDef TurnTheTables = new ()
    {
        Name = "Turn the tables",
        ConcreteClassPath = "AbilityCards.TurnTheTablesCard",
        DescriptionTemplate = "Swap your hole cards with your opponents.",
        IconPath = "res://Sprites/Cards/turn_the_tables.png",
        RankChangePerUse = -10,
        Cost = 5,
        BasePrice = 5,
        Rarity = 1,
        Rank = Enums.CardRank.Ten,
        Suit = Enums.CardSuit.Spades,
    };
    
    public static TapCardDef EyePatch = new ()
    {
        Name = "Eye patch",
        ConcreteClassPath = "AbilityCards.EyePatchCard",
        DescriptionTemplate = "Add face-down community cards.",
        IconPath = "res://Sprites/Cards/eye_patch.png",
        TappedCost = 4,
        Cost = 0,
        BasePrice = 0,
        Rarity = 1,
        Rank = Enums.CardRank.Jack,
        Suit = Enums.CardSuit.Spades
    };
    
    public static TapCardDef Xom = new ()
    {
        Name = "Xom",
        ConcreteClassPath = "AbilityCards.XomCard",
        DescriptionTemplate = "Random effects that change every turn.",
        IconPath = "res://Sprites/Cards/xom.png",
        TappedCost = 5,
        Cost = 0,
        BasePrice = 0,
        Rarity = 1,
        Rank = Enums.CardRank.Joker,
        Suit = Enums.CardSuit.Joker
    };
    
    public static TapCardDef HandShake = new ()
    {
        Name = "Hand shake",
        ConcreteClassPath = "AbilityCards.HandShakeCard",
        DescriptionTemplate = "You will be dealt cards in your opponent's deck and vice versa.",
        IconPath = "res://Sprites/Cards/hand_shake.png",
        TappedCost = 2,
        Cost = 0,
        BasePrice = 0,
        Rarity = 1,
        Rank = Enums.CardRank.Five,
        Suit = Enums.CardSuit.Hearts
    };

    
    // Skills
    public static SkillCardDef Feint = new ()
    {
        Name = "Feint",
        ConcreteClassPath = "SkillCards.FeintCard",
        DescriptionTemplate = "Grants vulnerable instead of dealing damage",
        IconPath = "res://Sprites/Cards/feint.png",
        Rarity = 1,
        Cost = 5,
        BasePrice = 5,
        Rank = Enums.CardRank.Ace,
        Suit = Enums.CardSuit.Spades
    };
    
    public static SkillCardDef DualWield = new ()
    {
        Name = "Dual wield",
        ConcreteClassPath = "SkillCards.DualWieldCard",
        DescriptionTemplate = "Attack twice",
        IconPath = "res://Sprites/Cards/dual_wield.png",
        Rarity = 1,
        Cost = 5,
        BasePrice = 5,
        Rank = Enums.CardRank.Two,
        Suit = Enums.CardSuit.Clubs
    };
    
    public static SkillCardDef Riposte = new ()
    {
        Name = "Riposte",
        ConcreteClassPath = "SkillCards.RiposteCard",
        DescriptionTemplate = "Negate the next incoming attack, then counter attack",
        IconPath = "res://Sprites/Cards/riposte.png",
        Rarity = 1,
        Cost = 5,
        BasePrice = 5,
        Rank = Enums.CardRank.Eight,
        Suit = Enums.CardSuit.Hearts
    };
    
    public static SkillCardDef SuckerPunch = new ()
    {
        Name = "Sucker punch",
        ConcreteClassPath = "SkillCards.SuckerPunchCard",
        DescriptionTemplate = "Attack as a defender",
        IconPath = "res://Sprites/Cards/sucker_punch.png",
        Rarity = 1,
        Cost = 5,
        BasePrice = 5,
        Rank = Enums.CardRank.Seven,
        Suit = Enums.CardSuit.Clubs
    };
    
    public static SkillCardDef TwoHanded = new ()
    {
        Name = "Two handed",
        ConcreteClassPath = "SkillCards.TwoHandedCard",
        DescriptionTemplate = "Make an attack that scales more with power",
        IconPath = "res://Sprites/Cards/two_handed.png",
        Rarity = 1,
        Cost = 5,
        BasePrice = 5,
        Rank = Enums.CardRank.Two,
        Suit = Enums.CardSuit.Clubs
    };
        
    public static List<BaseCardDef> All()
    {
        Type cards = typeof(CardDefs);
        var res = new List<BaseCardDef>();
        FieldInfo[] staticFields = cards.GetFields(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
        foreach (FieldInfo field in staticFields)
        {
            var value = field.GetValue(null);
            var cardDef = (BaseCardDef)value;
            res.Add(cardDef);
        }
        return res;
    }
}