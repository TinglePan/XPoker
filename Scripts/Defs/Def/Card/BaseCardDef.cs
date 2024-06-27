using XCardGame.Scripts.Common.Constants;

namespace XCardGame.Scripts.Defs.Def.Card;


public class BaseCardDef
{
    public string Name;
    public string ConcreteClassPath;
    public string DescriptionTemplate;
    public string IconPath;
    public Enums.CardSuit Suit;
    public Enums.CardRank Rank;
    public int Rarity;
    public int BasePrice;

    public BaseCardDef()
    {
        Suit = Enums.CardSuit.None;
        Rank = Enums.CardRank.None;
    }
}
