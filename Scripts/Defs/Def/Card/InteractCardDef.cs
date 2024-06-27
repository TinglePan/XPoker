using Godot;
using XCardGame.Scripts.Cards;
using XCardGame.Scripts.Common;
using XCardGame.Scripts.Common.Constants;

namespace XCardGame.Scripts.Defs.Def.Card;

public class InteractCardDef: BaseCardDef
{
    public Enums.InteractCardType Type;
    public Enums.InteractionType InteractionType;
    public Enums.ExhaustBehavior ExhaustBehavior;
    
    // Use
    public int UseCost;
    public int RankChangePerUse;
    
    // Seal
    public Enums.RuleNature RuleNature;
    public int SealCost;
    public int UnSealCost;
    public int SealWhenLessThan;
    
    // Use and seal
    public int RankChangePerTurn;
    public int SealedRankChangePerTurn;
    public Vector2I NaturalRankChangeRange;
    public int UnSealWhenGreaterThan;
    

    public InteractCardDef()
    {
        NaturalRankChangeRange = new Vector2I(Utils.GetCardRankValue(Enums.CardRank.Ace),
            Utils.GetCardRankValue(Enums.CardRank.King));
        SealWhenLessThan = Utils.GetCardRankValue(Enums.CardRank.Ace);
        UnSealWhenGreaterThan = Utils.GetCardRankValue(Enums.CardRank.King);
    }
}