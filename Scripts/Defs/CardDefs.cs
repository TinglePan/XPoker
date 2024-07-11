using System;
using System.Collections.Generic;
using System.Reflection;
using XCardGame.Scripts.Common;
using XCardGame.Scripts.Common.Constants;
using XCardGame.Scripts.Defs.Def.Card;

namespace XCardGame.Scripts.Defs;

public static class CardDefs
{
    public static ItemCardDef D6 = new ()
    {
        Name = Utils._("D6"),
        ConcreteClassPath = "InteractCards.ItemCards.D6Card",
        DescriptionTemplate = Utils._("Randomized destiny."),
        IconPath = "res://Sprites/Cards/d6.png",
        BasePrice = 5,
        Rarity = 1,
        Rank = Enums.CardRank.Six,
        
        Cost = 1,
        RankChangePerUse = -2,
    };
    
    public static ItemCardDef MagicalHat = new ()
    {
        Name = Utils._("Magical hat"),
        ConcreteClassPath = "InteractCards.ItemCards.MagicalHatCard",
        DescriptionTemplate = Utils._("Swap two cards."),
        IconPath = "res://Sprites/Cards/magical_hat.png",
        BasePrice = 5,
        Rarity = 1,
        Rank = Enums.CardRank.Three,
        
        Cost = 1,
        RankChangePerUse = -1,
    };
    
    public static ItemCardDef BalaTrollHand = new ()
    {
        Name = Utils._("BalaTroll Hand"),
        ConcreteClassPath = "InteractCards.ItemCards.BalaTrollHandCard",
        DescriptionTemplate = Utils._("Discard and redraw, like what you would do in balatro."),
        IconPath = "res://Sprites/Cards/balatroll_hand.png",
        BasePrice = 5,
        Rarity = 1,
        Rank = Enums.CardRank.Three,
        
        Cost = 1,
        RankChangePerUse = -1,
    };
    
    public static ItemCardDef TurnTheTables = new ()
    {
        Name = Utils._("Turn the tables"),
        ConcreteClassPath = "InteractCards.ItemCards.TurnTheTablesCard",
        DescriptionTemplate = Utils._("Swap your hole cards with your opponents."),
        IconPath = "res://Sprites/Cards/turn_the_tables.png",
        BasePrice = 5,
        Rarity = 1,
        
        Cost = 1,
        RankChangePerUse = -99,
    };
    
    public static ItemCardDef BigShield = new ()
    {
        Name = Utils._("Big shield"),
        ConcreteClassPath = "InteractCards.ItemCards.BigShieldCard",
        DescriptionTemplate = Utils._("Increase defend value while decrease attack value"),
        IconPath = "res://Sprites/Cards/big_shield.png",
        BasePrice = 5,
        Rarity = 1,
        
        Cost = 1,
        RankChangePerUse = -99,
    };
    
    public static ItemCardDef MillenniumEye = new ()
    {
        Name = Utils._("Millennium eye"),
        ConcreteClassPath = "InteractCards.ItemCards.MillenniumEyeCard",
        DescriptionTemplate = Utils._("I can see forever."),
        IconPath = "res://Sprites/Cards/millennium_eye.png",
        BasePrice = 5,
        Rarity = 1,
        
        Cost = 2,
        RankChangePerUse = -99,
    };
    
    public static RuleCardDef Darkness = new ()
    {
        Name = Utils._("Darkness"),
        ConcreteClassPath = "InteractCards.RuleCards.DarknessCard",
        DescriptionTemplate = Utils._("Add face-down community cards."),
        IconPath = "res://Sprites/Cards/darkness.png",
        Rarity = 1,
        
        SealCost = 1,
        AutoUnSeal = true,
    };
    
    public static RuleCardDef Capitalism = new ()
    {
        Name = Utils._("Capitalism"),
        ConcreteClassPath = "InteractCards.RuleCards.CapitalismCard",
        DescriptionTemplate = Utils._("Add hole cards dealt each round."),
        IconPath = "res://Sprites/Cards/capitalism.png",
        Rarity = 1,
        
        SealCost = 1,
        AutoUnSeal = true,
    };
    
    public static RuleCardDef Socialism = new ()
    {
        Name = Utils._("Socialism"),
        ConcreteClassPath = "InteractCards.RuleCards.SocialismCard",
        DescriptionTemplate = Utils._("Add community cards dealt each round."),
        IconPath = "res://Sprites/Cards/socialism.png",
        Rarity = 1,
        
        SealCost = 1,
        AutoUnSeal = true,
    };
    
    public static RuleCardDef KeepOut = new ()
    {
        Name = Utils._("Keep out"),
        ConcreteClassPath = "InteractCards.RuleCards.KeepOutCard",
        DescriptionTemplate = Utils._("Certain cards do not count. Current rule:\n{}"),
        IconPath = "res://Sprites/Cards/keep_out.png",
        Rarity = 1,
        
        SealCost = 1,
        AutoUnSeal = true,
    };
    
    public static RuleCardDef TheTieBreaker = new ()
    {
        Name = Utils._("The tie breaker"),
        ConcreteClassPath = "InteractCards.RuleCards.TheTieBreakerCard",
        DescriptionTemplate = Utils._("Card suit is used to break a tie. Current suit order:\n{}"),
        IconPath = "res://Sprites/Cards/the_tie_breaker.png",
        Rarity = 1,
        
        SealCost = 1,
        AutoUnSeal = true,
    };
    
    public static RuleCardDef Xom = new ()
    {
        Name = Utils._("Xom"),
        ConcreteClassPath = "InteractCards.RuleCards.XomCard",
        DescriptionTemplate = Utils._("Random effects that change every turn. Current effects:\n{}"),
        IconPath = "res://Sprites/Cards/xom.png",
        Rarity = 1,
        Rank = Enums.CardRank.Joker,
        Suit = Enums.CardSuit.Joker,
        
        SealCost = 1,
        AutoUnSeal = true,
    };
    
    public static RuleCardDef Separation = new ()
    {
        Name = Utils._("Separation"),
        ConcreteClassPath = "InteractCards.RuleCards.SeparationCard",
        DescriptionTemplate = Utils._("You will not be dealt cards from your opponent's deck, vice versa."),
        IconPath = "res://Sprites/Cards/hand_shake.png",
        Rarity = 1,
        
        SealCost = 0,
    };
    
    public static RuleCardDef SpadesRule = new ()
    {
        Name = Utils._("Spades rule"),
        ConcreteClassPath = "InteractCards.RuleCards.SpadesRuleCard",
        DescriptionTemplate = Utils._("When attack, spades resolve with 50% life leech."),
        IconPath = "res://Sprites/Cards/spades_rule.png",
        Rarity = 1,
        
        SealCost = 1,
        AutoUnSeal = true,
    };
    
    public static RuleCardDef HeartsRule = new ()
    {
        Name = Utils._("Hearts rule"),
        ConcreteClassPath = "InteractCards.RuleCards.HeartsRuleCard",
        DescriptionTemplate = Utils._("When defend, hearts resolve with life recovery of 50% card rank value."),
        IconPath = "res://Sprites/Cards/hearts_rule.png",
        Rarity = 1,
        
        SealCost = 1,
        AutoUnSeal = true,
    };
    
    public static RuleCardDef ClubsRule = new ()
    {
        Name = Utils._("Clubs rule"),
        ConcreteClassPath = "InteractCards.RuleCards.ClubsRuleCard",
        DescriptionTemplate = Utils._("When attack, clubs resolve with double card rank value."),
        IconPath = "res://Sprites/Cards/clubs_rule.png",
        Rarity = 1,
        
        SealCost = 1,
        AutoUnSeal = true,
    };
    
    public static RuleCardDef DiamondsRule = new ()
    {
        Name = Utils._("Diamonds rule"),
        ConcreteClassPath = "InteractCards.RuleCards.DiamondsRuleCard",
        DescriptionTemplate = Utils._("When defend, diamonds are the only suit that matters."),
        IconPath = "res://Sprites/Cards/diamonds_rule.png",
        Rarity = 1,
        
        SealCost = 1,
        AutoUnSeal = true,
    };
    
    public static RuleCardDef NerfFlush = new ()
    {
        Name = Utils._("Nerf flush"),
        ConcreteClassPath = "InteractCards.RuleCards.NerfFlushCard",
        DescriptionTemplate = Utils._("Reduce Flush's hand tier by 1."),
        IconPath = "res://Sprites/Cards/nerf_flush.png",
        Rarity = 1,
        
        SealCost = 1,
        AutoUnSeal = true,
    };
    
    public static RuleCardDef ShortDeckRule = new ()
    {
        Name = Utils._("Short deck rule"),
        ConcreteClassPath = "InteractCards.RuleCards.ShortDeckRuleCard",
        DescriptionTemplate = Utils._("Straight connects Ace and 6 "),
        IconPath = "res://Sprites/Cards/short_deck_rule.png",
        Rarity = 1,
        
        SealCost = 1,
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