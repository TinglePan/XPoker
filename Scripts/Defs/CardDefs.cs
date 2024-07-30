using System;
using System.Collections.Generic;
using System.Reflection;
using XCardGame.Common;

namespace XCardGame;

public static class CardDefs
{
    public static ItemCardDef D6 = new ()
    {
        Name = Utils._("D6"),
        ConcreteClassPath = "D6Card",
        DescriptionTemplate = Utils._("Randomized destiny."),
        IconPath = "res://Sprites/Cards/d6.png",
        BasePrice = 5,
        Rarity = 1,
        Rank = Enums.CardRank.Six,
        
        ExcludeFromRewards = true,
        IsInnate = true,
        Cost = 1,
        RankChangePerUse = -1,
    };

    public static ItemCardDef LesserD6 = new()
    {
        Name = Utils._("Lesser d6"),
        ConcreteClassPath = "D6Card",
        DescriptionTemplate = Utils._("Randomized destiny."),
        IconPath = "res://Sprites/Cards/d6.png",
        BasePrice = 0,
        Rarity = 1,
        Rank = Enums.CardRank.Six,

        ExcludeFromRewards = true,
        Cost = 1,
        RankChangePerUse = -99,
    };
    
    public static ItemCardDef MagicalHat = new ()
    {
        Name = Utils._("Magical hat"),
        ConcreteClassPath = "MagicalHatCard",
        DescriptionTemplate = Utils._("Swap two cards."),
        IconPath = "res://Sprites/Cards/magical_hat.png",
        BasePrice = 5,
        Rarity = 1,
        Rank = Enums.CardRank.Four,
        
        ExcludeFromRewards = true,
        IsInnate = true,
        Cost = 1,
        RankChangePerUse = -1,
    };
    
    public static ItemCardDef LesserMagicalHat = new ()
    {
        Name = Utils._("Lesser magical hat"),
        ConcreteClassPath = "MagicalHatCard",
        DescriptionTemplate = Utils._("Swap two cards."),
        IconPath = "res://Sprites/Cards/magical_hat.png",
        BasePrice = 0,
        Rarity = 1,
        Rank = Enums.CardRank.Ace,
        
        ExcludeFromRewards = true,
        Cost = 1,
        RankChangePerUse = -99,
    };
    
    public static ItemCardDef BalaTrollHand = new ()
    {
        Name = Utils._("BalaTroll Hand"),
        ConcreteClassPath = "BalaTrollHandCard",
        DescriptionTemplate = Utils._("Discard and redraw, like what you would do in balatro."),
        IconPath = "res://Sprites/Cards/balatroll_hand.png",
        BasePrice = 5,
        Rarity = 1,
        Rank = Enums.CardRank.Four,
        
        ExcludeFromRewards = true,
        IsInnate = true,
        Cost = 1,
        RankChangePerUse = -1,
    };

    public static ItemCardDef LesserBalaTrollHand = new()
    {
        Name = Utils._("Lesser balaTroll Hand"),
        ConcreteClassPath = "BalaTrollHandCard",
        DescriptionTemplate = Utils._("Discard and redraw, like what you would do in balatro."),
        IconPath = "res://Sprites/Cards/balatroll_hand.png",
        BasePrice = 0,
        Rarity = 1,
        Rank = Enums.CardRank.Ace,

        ExcludeFromRewards = true,
        Cost = 1,
        RankChangePerUse = -99,
    };

    public static ItemCardDef CopyPaste = new()
    {
        Name = Utils._("Copy & paste"),
        ConcreteClassPath = "CopyPasteCard",
        DescriptionTemplate =
            Utils._("Create a copy of a selected card."),
        IconPath = "res://Sprites/Cards/copy_paste.png",
        BasePrice = 5,
        Rarity = 1,
        Rank = Enums.CardRank.Three,
        
        ExcludeFromRewards = true,
        IsInnate = true,
        Cost = 1,
        RankChangePerUse = -1,
    };

    public static ItemCardDef LesserCopyPaste = new()
    {

        Name = Utils._("Lesser copy & paste"),
        ConcreteClassPath = "CopyPasteCard",
        DescriptionTemplate =
            Utils._("Create a copy of a selected card."),
        IconPath = "res://Sprites/Cards/copy_paste.png",
        BasePrice = 0,
        Rarity = 1,
        Rank = Enums.CardRank.Ace,
        
        ExcludeFromRewards = true,
        Cost = 1,
        RankChangePerUse = -99,
    };

    public static ItemCardDef Copy = new()
    {
        Name = Utils._("Copy"),
        ConcreteClassPath = "CopyCard",
        DescriptionTemplate =
            Utils._("A copy. You can replace a card with it when needed. Exhaust."),
        IconPath = "res://Sprites/Cards/copy.png",
        Rarity = 1,

        ExcludeFromRewards = true,
        IsExhaust = true,
        Cost = 1,
        RankChangePerUse = 0,
    };
    
    public static ItemCardDef GoldenEye = new ()
    {
        Name = Utils._("Golden eye"),
        ConcreteClassPath = "GoldenEyeCard",
        DescriptionTemplate = Utils._("This trinket reveals the face-down cards."),
        IconPath = "res://Sprites/Cards/golden_eye.png",
        BasePrice = 5,
        Rarity = 1,
        
        Cost = 2,
        RankChangePerUse = -99,
    };
    
    public static RuleCardDef Darkness = new ()
    {
        Name = Utils._("Darkness"),
        ConcreteClassPath = "DarknessCard",
        DescriptionTemplate = Utils._("Add face-down community cards."),
        IconPath = "res://Sprites/Cards/darkness.png",
        Rarity = 1,
        
        Cost = 1,
        AutoUnSeal = true,
    };
    
    public static RuleCardDef KeepOut = new ()
    {
        Name = Utils._("Keep out"),
        ConcreteClassPath = "KeepOutCard",
        DescriptionTemplate = Utils._("Certain cards do not count. Current rule:\n{}"),
        IconPath = "res://Sprites/Cards/keep_out.png",
        Rarity = 1,
        
        Cost = 1,
        AutoUnSeal = true,
    };
    
    public static RuleCardDef TheTieBreaker = new ()
    {
        Name = Utils._("The tie breaker"),
        ConcreteClassPath = "TheTieBreakerCard",
        DescriptionTemplate = Utils._("Card suit is used to break a tie. Current suit order:\n{}"),
        IconPath = "res://Sprites/Cards/the_tie_breaker.png",
        Rarity = 1,
        
        Cost = 1,
        AutoUnSeal = true,
    };
    
    public static RuleCardDef Xom = new ()
    {
        Name = Utils._("Xom"),
        ConcreteClassPath = "XomCard",
        DescriptionTemplate = Utils._("Random effects that change every turn. Current effects:\n{}"),
        IconPath = "res://Sprites/Cards/xom.png",
        Rarity = 1,
        Rank = Enums.CardRank.Joker,
        Suit = Enums.CardSuit.Joker,
        
        Cost = 1,
        AutoUnSeal = true,
    };
    
    public static RuleCardDef Separation = new ()
    {
        Name = Utils._("Separation"),
        ConcreteClassPath = "SeparationCard",
        DescriptionTemplate = Utils._("You will not be dealt cards from your opponent's deck, vice versa."),
        IconPath = "res://Sprites/Cards/hand_shake.png",
        Rarity = 1,
        
    };
    
    public static RuleCardDef SpadesRule = new ()
    {
        Name = Utils._("Spades rule"),
        ConcreteClassPath = "SpadesRuleCard",
        DescriptionTemplate = Utils._("When attack, spades resolve with 50% life leech."),
        IconPath = "res://Sprites/Cards/spades_rule.png",
        Rarity = 1,
        
        Cost = 1,
        AutoUnSeal = true,
    };
    
    public static RuleCardDef HeartsRule = new ()
    {
        Name = Utils._("Hearts rule"),
        ConcreteClassPath = "HeartsRuleCard",
        DescriptionTemplate = Utils._("When defend, hearts resolve with life recovery of 50% card rank value."),
        IconPath = "res://Sprites/Cards/hearts_rule.png",
        Rarity = 1,
        
        Cost = 1,
        AutoUnSeal = true,
    };
    
    public static RuleCardDef ClubsRule = new ()
    {
        Name = Utils._("Clubs rule"),
        ConcreteClassPath = "ClubsRuleCard",
        DescriptionTemplate = Utils._("When attack, clubs resolve with double card rank value."),
        IconPath = "res://Sprites/Cards/clubs_rule.png",
        Rarity = 1,
        
        Cost = 1,
        AutoUnSeal = true,
    };
    
    public static RuleCardDef DiamondsRule = new ()
    {
        Name = Utils._("Diamonds rule"),
        ConcreteClassPath = "DiamondsRuleCard",
        DescriptionTemplate = Utils._("When defend, diamonds are the only suit that matters."),
        IconPath = "res://Sprites/Cards/diamonds_rule.png",
        Rarity = 1,
        
        Cost = 1,
        AutoUnSeal = true,
    };
    
    public static RuleCardDef NerfFlush = new ()
    {
        Name = Utils._("Nerf flush"),
        ConcreteClassPath = "NerfFlushCard",
        DescriptionTemplate = Utils._("Reduce Flush's hand tier by 1."),
        IconPath = "res://Sprites/Cards/nerf_flush.png",
        Rarity = 1,
        
        Cost = 1,
        AutoUnSeal = true,
    };
    
    public static RuleCardDef ShortDeckRule = new ()
    {
        Name = Utils._("Short deck rule"),
        ConcreteClassPath = "ShortDeckRuleCard",
        DescriptionTemplate = Utils._("Straight connects Ace and 6 "),
        IconPath = "res://Sprites/Cards/short_deck_rule.png",
        Rarity = 1,
        
        Cost = 1,
        AutoUnSeal = true,
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