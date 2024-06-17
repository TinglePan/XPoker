using XCardGame.Scripts.Common.Constants;

namespace XCardGame.Scripts.Defs;

public class BaseCardDef
{
    public string Name;
    public string DescriptionTemplate;
    public string IconPath;
    public Enums.CardSuit Suit;
    public Enums.CardRank Rank;
    public int BaseCredit;
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

public static class Cards
{
    public static UseCardDef D6 = new ()
    {
        Name = "D6",
        DescriptionTemplate = "Reroll your destiny.",
        IconPath = "res://Sprites/Cards/d6.png",
        RankChangePerUse = -1,
        Cost = 5,
        BaseCredit = 5,
        Rank = Enums.CardRank.Six,
        Suit = Enums.CardSuit.Diamonds
    };
    
    public static TapCardDef Capitalism = new ()
    {
        Name = "Capitalism",
        DescriptionTemplate = "Add hole cards dealt each round.",
        IconPath = "res://Sprites/Cards/capitalism.png",
        TappedCost = 2,
        Cost = 0,
        BaseCredit = 5,
        Rank = Enums.CardRank.Ace,
        Suit = Enums.CardSuit.Diamonds
    };
    
    public static TapCardDef Socialism = new ()
    {
        Name = "Socialism",
        DescriptionTemplate = "Add community cards dealt each round.",
        IconPath = "res://Sprites/Cards/socialism.png",
        TappedCost = 2,
        Cost = 0,
        BaseCredit = 5,
        Rank = Enums.CardRank.Ace,
        Suit = Enums.CardSuit.Clubs
    };
    
    public static UseCardDef MagicalHat = new ()
    {
        Name = "Magical Hat",
        DescriptionTemplate = "Swap two cards in hole card area or community card area.",
        IconPath = "res://Sprites/Cards/magical_hat.png",
        RankChangePerUse = -1,
        Cost = 5,
        BaseCredit = 5,
        Rank = Enums.CardRank.Three,
        Suit = Enums.CardSuit.Spades
    };
    
    public static UseCardDef Balatroll = new ()
    {
        Name = "Balatroll",
        DescriptionTemplate = "Discard and redraw.",
        IconPath = "res://Sprites/Cards/balatroll.png",
        RankChangePerUse = -1,
        Cost = 4,
        BaseCredit = 4,
        Rank = Enums.CardRank.Three,
        Suit = Enums.CardSuit.Hearts
    };
    
    public static TapCardDef BigShield = new ()
    {
        Name = "Big shield",
        DescriptionTemplate = "Deal and receive no damage for this round",
        IconPath = "res://Sprites/Cards/big_shield.png",
        TappedCost = 0,
        Cost = 5,
        BaseCredit = 5,
        Rank = Enums.CardRank.King,
        Suit = Enums.CardSuit.Hearts
    };
    
    public static TapCardDef KeepOut = new ()
    {
        Name = "Keep out",
        DescriptionTemplate = "Certain cards do not count, the rule alters each round.",
        IconPath = "res://Sprites/Cards/keep_out.png",
        TappedCost = 2,
        Cost = 0,
        BaseCredit = 5,
        Rank = Enums.CardRank.King,
        Suit = Enums.CardSuit.Spades
    };
    
    public static UseCardDef MillenniumEye = new ()
    {
        Name = "Millennium Eye",
        DescriptionTemplate = "All knowing at the cost of all power.",
        IconPath = "res://Sprites/Cards/millennium_eye.png",
        RankChangePerUse = -1,
        Cost = 5,
        BaseCredit = 5,
        Rank = Enums.CardRank.Ace,
        Suit = Enums.CardSuit.Spades
    };
    
    public static TapCardDef TheTieBreaker = new ()
    {
        Name = "The Tie Breaker",
        DescriptionTemplate = "Card suit is used to break a tie, suit order from high to low: Spades, Hearts, Clubs, Diamonds",
        IconPath = "res://Sprites/Cards/the_tie_breaker.png",
        TappedCost = 2,
        Cost = 0,
        BaseCredit = 5,
        Rank = Enums.CardRank.Ace,
        Suit = Enums.CardSuit.Spades
    };
    
    public static UseCardDef TurnTheTables = new ()
    {
        Name = "Turn The Tables",
        DescriptionTemplate = "Swap your hole cards with your opponents.",
        IconPath = "res://Sprites/Cards/turn_the_tables.png",
        RankChangePerUse = -10,
        Cost = 5,
        BaseCredit = 5,
        Rank = Enums.CardRank.Ten,
        Suit = Enums.CardSuit.Spades
    };
    
    public static TapCardDef EyePatch = new ()
    {
        Name = "Eye Patch",
        DescriptionTemplate = "Add face-down community cards.",
        IconPath = "res://Sprites/Cards/eye_patch.png",
        TappedCost = 4,
        Cost = 0,
        BaseCredit = 0,
        Rank = Enums.CardRank.Jack,
        Suit = Enums.CardSuit.Spades
    };
    
    public static TapCardDef Xom = new ()
    {
        Name = "Xom",
        DescriptionTemplate = "Random effects that change every turn.",
        IconPath = "res://Sprites/Cards/xom.png",
        TappedCost = 5,
        Cost = 0,
        BaseCredit = 0,
        Rank = Enums.CardRank.BlackJoker,
        Suit = Enums.CardSuit.BlackJoker
    };
    
    public static TapCardDef HandShake = new ()
    {
        Name = "Hand Shake",
        DescriptionTemplate = "You will be dealt cards in your opponent's deck and vice versa.",
        IconPath = "res://Sprites/Cards/hand_shake.png",
        TappedCost = 2,
        Cost = 0,
        BaseCredit = 0,
        Rank = Enums.CardRank.Five,
        Suit = Enums.CardSuit.Hearts
    };
}