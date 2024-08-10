using System;
using System.Collections.Generic;
using System.Reflection;
using XCardGame.Common;

namespace XCardGame;

public static class CardDefs
{
    public static CardDef D6 = new ()
    {
        Name = Utils._("D6"),
        ConcreteClassPath = "D6Card",
        DescriptionTemplate = Utils._("Randomized destiny."),
        IconPath = "res://Sprites/Cards/d6.png",
        Rank = Enums.CardRank.Six,
        
        IsItem = true,
        ExcludeFromShop = true, 
        ExcludeFromRewards = true,
        IsInnate = true,
        Cost = 1,
        RankChangePerUse = -1,
    };

    public static CardDef LesserD6 = new()
    {
        Name = Utils._("Lesser d6"),
        ConcreteClassPath = "D6Card",
        DescriptionTemplate = Utils._("Randomized destiny."),
        IconPath = "res://Sprites/Cards/d6.png",
        Rank = Enums.CardRank.Six,

        IsItem = true,
        ExcludeFromShop = true, 
        ExcludeFromRewards = true,
        Cost = 1,
        RankChangePerUse = -99,
    };
    
    public static CardDef MagicalHat = new ()
    {
        Name = Utils._("Magical hat"),
        ConcreteClassPath = "MagicalHatCard",
        DescriptionTemplate = Utils._("Swap two cards."),
        IconPath = "res://Sprites/Cards/magical_hat.png",
        Rank = Enums.CardRank.Four,
        
        IsItem = true,
        ExcludeFromShop = true, 
        ExcludeFromRewards = true,
        IsInnate = true,
        Cost = 1,
        RankChangePerUse = -1,
    };
    
    public static CardDef LesserMagicalHat = new ()
    {
        Name = Utils._("Lesser magical hat"),
        ConcreteClassPath = "MagicalHatCard",
        DescriptionTemplate = Utils._("Swap two cards."),
        IconPath = "res://Sprites/Cards/magical_hat.png",
        Rank = Enums.CardRank.Ace,
        
        IsItem = true,
        ExcludeFromShop = true, 
        ExcludeFromRewards = true,
        Cost = 1,
        RankChangePerUse = -99,
    };
    
    public static CardDef BalaTroll = new ()
    {
        Name = Utils._("BalaTroll"),
        ConcreteClassPath = "BalaTrollCard",
        DescriptionTemplate = Utils._("Discard and redraw, like what you would do in balatro."),
        IconPath = "res://Sprites/Cards/balatroll.png",
        Rank = Enums.CardRank.Four,
        
        IsItem = true,
        IsPiled = true,
        PileCardCountMax = 3,
        ExcludeFromShop = true, 
        ExcludeFromRewards = true,
        IsInnate = true,
        Cost = 1,
        RankChangePerUse = -1,
    };

    public static CardDef CopyPaste = new()
    {
        Name = Utils._("Copy & paste"),
        ConcreteClassPath = "CopyPasteCard",
        DescriptionTemplate = Utils._("Create a copy of a selected card."),
        IconPath = "res://Sprites/Cards/copy_paste.png",
        Rank = Enums.CardRank.Three,
        
        IsItem = true,
        ExcludeFromShop = true, 
        ExcludeFromRewards = true,
        IsInnate = true,
        Cost = 1,
        RankChangePerUse = -1,
    };

    public static CardDef LesserCopyPaste = new()
    {

        Name = Utils._("Lesser copy & paste"),
        ConcreteClassPath = "CopyPasteCard",
        DescriptionTemplate = Utils._("Create a copy of a selected card."),
        IconPath = "res://Sprites/Cards/copy_paste.png",
        Rank = Enums.CardRank.Ace,
        
        IsItem = true,
        ExcludeFromShop = true, 
        ExcludeFromRewards = true,
        Cost = 1,
        RankChangePerUse = -99,
    };

    public static CardDef Copy = new()
    {
        Name = Utils._("Copy"),
        ConcreteClassPath = "CopyCard",
        DescriptionTemplate = Utils._("A copy. You can replace a card with it when needed. Exhaust."),
        IconPath = "res://Sprites/Cards/copy.png",

        IsItem = true,
        ExcludeFromShop = true, 
        ExcludeFromRewards = true,
        IsExhaust = true,
        Cost = 1,
        RankChangePerUse = 0,
    };
    
    public static CardDef GoldenEye = new ()
    {
        Name = Utils._("Golden eye"),
        ConcreteClassPath = "GoldenEyeCard",
        DescriptionTemplate = Utils._("This trinket reveals the face-down cards."),
        IconPath = "res://Sprites/Cards/golden_eye.png",
        
        IsItem = true,
        BasePrice = 5,
        Rarity = 1,
        Cost = 2,
        RankChangePerUse = -99,
    };
    
    public static CardDef Darkness = new ()
    {
        Name = Utils._("Darkness"),
        ConcreteClassPath = "DarknessCard",
        DescriptionTemplate = Utils._("Add last flip face down community cards."),
        IconPath = "res://Sprites/Cards/darkness.png",
        
        IsRule = true,
        IsUnstable = true,
        ExcludeFromShop = true, 
        ExcludeFromRewards = true,
        
        Cost = 1,
        UnstableWhenTapped = true,
    };
    
    public static CardDef KeepOut = new ()
    {
        Name = Utils._("Keep out"),
        ConcreteClassPath = "KeepOutCard",
        DescriptionTemplate = Utils._("Certain cards do not count. Current rule:\n{0}"),
        IconPath = "res://Sprites/Cards/keep_out.png",
        
        IsRule = true,
        IsUnstable = true,
        ExcludeFromShop = true, 
        ExcludeFromRewards = true,
        
        Cost = 1,
        UnstableWhenTapped = true,
    };
    
    public static CardDef Xom = new ()
    {
        Name = Utils._("Xom"),
        ConcreteClassPath = "XomCard",
        DescriptionTemplate = Utils._("Random effects that change every turn. Current effects:\n{0}"),
        IconPath = "res://Sprites/Cards/xom.png",
        Rank = Enums.CardRank.Joker,
        Suit = Enums.CardSuit.Joker,
        
        IsRule = true,
        ExcludeFromShop = true, 
        ExcludeFromRewards = true,
        
        Cost = 1,
    };
    
    public static CardDef NerfFlush = new ()
    {
        Name = Utils._("Nerf flush"),
        ConcreteClassPath = "NerfFlushCard",
        DescriptionTemplate = Utils._("Reduce Flush's hand tier by 1."),
        IconPath = "res://Sprites/Cards/nerf_flush.png",
        Rarity = 1,
        
        IsRule = true,
        ExcludeFromShop = true, 
        ExcludeFromRewards = true,
        Cost = 1,
    };
    
    public static CardDef ShortDeckRule = new ()
    {
        Name = Utils._("Short deck rule"),
        ConcreteClassPath = "ShortDeckRuleCard",
        DescriptionTemplate = Utils._("Straight connects Ace and 6 "),
        IconPath = "res://Sprites/Cards/short_deck_rule.png",
        
        IsInnate = true,
        IsRule = true,
        ExcludeFromShop = true, 
        ExcludeFromRewards = true,
        Cost = 1,
    };
    
    public static List<CardDef> All()
    {
        Type cards = typeof(CardDefs);
        var res = new List<CardDef>();
        FieldInfo[] staticFields = cards.GetFields(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
        foreach (FieldInfo field in staticFields)
        {
            var value = field.GetValue(null);
            var cardDef = (CardDef)value;
            res.Add(cardDef);
        }
        return res;
    }
}