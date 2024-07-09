using System;
using System.Collections.Generic;
using System.Reflection;
using XCardGame.Scripts.Common;
using XCardGame.Scripts.Common.Constants;
using XCardGame.Scripts.Defs.Def.Card;

namespace XCardGame.Scripts.Defs;

public static class CardDefs
{
    public static InteractCardDef D6 = new ()
    {
        Name = Utils._("D6"),
        ConcreteClassPath = "InteractCards.ItemCards.D6Card",
        DescriptionTemplate = Utils._("Randomized destiny."),
        IconPath = "res://Sprites/Cards/d6.png",
        BasePrice = 5,
        Rarity = 1,
        Rank = Enums.CardRank.Six,
        
        InteractionType = Enums.InteractionType.Use,
        UseCost = 1,
        RankChangePerUse = -2,
    };
    
    public static InteractCardDef MagicalHat = new ()
    {
        Name = Utils._("Magical hat"),
        ConcreteClassPath = "InteractCards.ItemCards.MagicalHatCard",
        DescriptionTemplate = Utils._("Swap two cards."),
        IconPath = "res://Sprites/Cards/magical_hat.png",
        BasePrice = 5,
        Rarity = 1,
        Rank = Enums.CardRank.Three,
        
        InteractionType = Enums.InteractionType.Use,
        UseCost = 1,
        RankChangePerUse = -1,
    };
    
    public static InteractCardDef BalaTrollHand = new ()
    {
        Name = Utils._("BalaTroll Hand"),
        ConcreteClassPath = "InteractCards.ItemCards.BalaTrollHandCard",
        DescriptionTemplate = Utils._("Discard and redraw, like what you would do in balatro."),
        IconPath = "res://Sprites/Cards/balatroll_hand.png",
        BasePrice = 5,
        Rarity = 1,
        Rank = Enums.CardRank.Three,
        
        InteractionType = Enums.InteractionType.Use,
        UseCost = 1,
        RankChangePerUse = -1,
    };
    
    public static InteractCardDef TurnTheTables = new ()
    {
        Name = Utils._("Turn the tables"),
        ConcreteClassPath = "InteractCards.ItemCards.TurnTheTablesCard",
        DescriptionTemplate = Utils._("Swap your hole cards with your opponents."),
        IconPath = "res://Sprites/Cards/turn_the_tables.png",
        BasePrice = 5,
        Rarity = 1,
        
        InteractionType = Enums.InteractionType.Use,
        UseCost = 1,
        RankChangePerUse = -99,
    };
    
    public static InteractCardDef EyePatch = new ()
    {
        Name = Utils._("Eye patch"),
        ConcreteClassPath = "InteractCards.EquipmentCards.EyePatchCard",
        DescriptionTemplate = Utils._("Add face-down community cards."),
        IconPath = "res://Sprites/Cards/eye_patch.png",
        BasePrice = 5,
        Rarity = 1,
        InteractionType = Enums.InteractionType.Seal
    };
    
    public static InteractCardDef BigShield = new ()
    {
        Name = Utils._("Big shield"),
        ConcreteClassPath = "InteractCards.EquipmentCards.BigShieldCard",
        DescriptionTemplate = Utils._("Increase defend value while decrease attack value"),
        IconPath = "res://Sprites/Cards/big_shield.png",
        BasePrice = 5,
        Rarity = 1,
        
        InteractionType = Enums.InteractionType.Use,
        UseCost = 1,
        RankChangePerUse = -99,
    };
    
    public static InteractCardDef MillenniumEye = new ()
    {
        Name = Utils._("Millennium eye"),
        ConcreteClassPath = "InteractCards.EquipmentCards.MillenniumEyeCard",
        DescriptionTemplate = Utils._("I can see forever."),
        IconPath = "res://Sprites/Cards/millennium_eye.png",
        BasePrice = 5,
        Rarity = 1,
        
        InteractionType = Enums.InteractionType.Seal,
        UnSealCost = 1,
    };
    
    public static InteractCardDef Capitalism = new ()
    {
        Name = Utils._("Capitalism"),
        ConcreteClassPath = "InteractCards.RuleCards.CapitalismCard",
        DescriptionTemplate = Utils._("Add hole cards dealt each round."),
        IconPath = "res://Sprites/Cards/capitalism.png",
        Rarity = 1,
        
        InteractionType = Enums.InteractionType.Seal,
        RuleNature = Enums.RuleNature.Neutral,
        SealCost = 1,
        SealedRankChangePerTurn = 99
    };
    
    public static InteractCardDef Socialism = new ()
    {
        Name = Utils._("Socialism"),
        ConcreteClassPath = "InteractCards.RuleCards.SocialismCard",
        DescriptionTemplate = Utils._("Add community cards dealt each round."),
        IconPath = "res://Sprites/Cards/socialism.png",
        Rarity = 1,
        
        InteractionType = Enums.InteractionType.Seal,
        RuleNature = Enums.RuleNature.Neutral,
        SealCost = 1,
        SealedRankChangePerTurn = 99
    };
    
    public static InteractCardDef KeepOut = new ()
    {
        Name = Utils._("Keep out"),
        ConcreteClassPath = "InteractCards.RuleCards.KeepOutCard",
        DescriptionTemplate = Utils._("Certain cards do not count. Current rule:\n{}"),
        IconPath = "res://Sprites/Cards/keep_out.png",
        Rarity = 1,
        
        InteractionType = Enums.InteractionType.Seal,
        RuleNature = Enums.RuleNature.Neutral,
        SealCost = 1,
        SealedRankChangePerTurn = 99
    };
    
    public static InteractCardDef TheTieBreaker = new ()
    {
        Name = Utils._("The tie breaker"),
        ConcreteClassPath = "InteractCards.RuleCards.TheTieBreakerCard",
        DescriptionTemplate = Utils._("Card suit is used to break a tie. Current suit order:\n{}"),
        IconPath = "res://Sprites/Cards/the_tie_breaker.png",
        Rarity = 1,
        
        InteractionType = Enums.InteractionType.Seal,
        RuleNature = Enums.RuleNature.Neutral,
        SealCost = 1,
        SealedRankChangePerTurn = 99
    };
    
    public static InteractCardDef Xom = new ()
    {
        Name = Utils._("Xom"),
        ConcreteClassPath = "InteractCards.RuleCards.XomCard",
        DescriptionTemplate = Utils._("Random effects that change every turn. Current effects:\n{}"),
        IconPath = "res://Sprites/Cards/xom.png",
        Rarity = 1,
        Rank = Enums.CardRank.Joker,
        Suit = Enums.CardSuit.Joker,
        
        InteractionType = Enums.InteractionType.Seal,
        RuleNature = Enums.RuleNature.Neutral,
        SealCost = 1,
        SealedRankChangePerTurn = 99
    };
    
    public static InteractCardDef Separation = new ()
    {
        Name = Utils._("Separation"),
        ConcreteClassPath = "InteractCards.RuleCards.SeparationCard",
        DescriptionTemplate = Utils._("You will not be dealt cards from your opponent's deck, vice versa."),
        IconPath = "res://Sprites/Cards/hand_shake.png",
        Rarity = 1,
        
        InteractionType = Enums.InteractionType.Seal,
        RuleNature = Enums.RuleNature.Neutral,
        SealCost = 1,
        SealedRankChangePerTurn = 99
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