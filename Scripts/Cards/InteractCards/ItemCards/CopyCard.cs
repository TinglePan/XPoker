using System.Collections.Generic;
using XCardGame.Scripts.Common.Constants;
using XCardGame.Scripts.Defs.Def.Card;
using XCardGame.Scripts.Ui;

namespace XCardGame.Scripts.Cards.InteractCards.ItemCards;

public class CopyCard : BaseTokenCard<CopyCard, BaseTokenCardInputHandler<CopyCard>>
{
    protected static ItemCardDef CreateDefFromCopiedCard(ItemCardDef def, BaseCard card)
    {
        var res = new ItemCardDef()
        {
            Name = def.Name,
            DescriptionTemplate = def.DescriptionTemplate,
            ConcreteClassPath = def.ConcreteClassPath,
            Rarity = def.Rarity,
            IconPath = def.IconPath,
            Cost = def.Cost,
            RankChangePerUse = def.RankChangePerUse,
        };
        res.Rank = card.Def.Rank;
        res.Suit = card.Def.Suit;
        return res;
    }
        
    public CopyCard(ItemCardDef def, BaseCard target) : base(CreateDefFromCopiedCard(def, target))
    {
    }

    public override void Setup(object o)
    {
        base.Setup(o);
        Rank.Value = Def.Rank;
        Suit.Value = Def.Suit;
    }
    
    protected override IEnumerable<CardContainer> GetValidTargetContainers()
    {
        yield return Battle.CommunityCardContainer;
        yield return Battle.Player.HoleCardContainer;
        yield return Battle.Enemy.HoleCardContainer;
    }
    
    protected override bool FilterValidCardNodes(CardNode node)
    {
        return node.FaceDirection.Value == Enums.CardFace.Up;
    }

    protected override BaseTokenCardInputHandler<CopyCard> GetInputHandler()
    {
        var cardNode = Node<CardNode>();
        return new BaseTokenCardInputHandler<CopyCard>(GameMgr, cardNode);
    }
}