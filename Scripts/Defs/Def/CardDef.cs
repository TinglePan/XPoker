using System;
using XCardGame.Common;

namespace XCardGame;


[Serializable]
public class CardDef: BaseDef
{
    public string Name;
    public string ConcreteClassPath;
    public string DescriptionTemplate;
    public string IconPath;
    public Enums.CardSuit Suit;
    public Enums.CardRank Rank;
    
    // Tags
    public bool IsItem;
    public bool IsRule;
    public bool IsPiled;
    public bool IsUnstable;
    public bool ExcludeFromRewards;
    public bool ExcludeFromShop;
    
    // Rng
    public int Rarity;
    
    // Shop
    public int BasePrice;
    
    // Usable
    public bool IsInnate;
    public bool IsExhaust;
    public int Cost;
    
    // Item
    public int RankChangePerUse;
    
    // Piled
    public int PileCardCountMax;
    
    // Unstable
    public bool UnstableWhenTapped;
    
    public bool IsUsable => IsItem || IsRule;
    public bool IsRng => !(ExcludeFromRewards && ExcludeFromShop); 
    
    public CardDef()
    {
        Suit = Enums.CardSuit.None;
        Rank = Enums.CardRank.None;
    }
    
}
