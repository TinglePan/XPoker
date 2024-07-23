using XCardGame.Scripts.Defs.Def.Card;
using XCardGame.Scripts.Ui;

namespace XCardGame.Scripts.Cards.InteractCards.ItemCards;

public class BaseItemCard: BaseInteractCard
{
    public BaseItemCard(ItemCardDef def): base(def)
    {
    }

    public override bool CanInteract(CardNode node)
    {
        if (!base.CanInteract(node)) return false;
        if (node.IsTapped.Value) return false;
        var itemCardDef = (ItemCardDef)Def;
        if (node.CurrentContainer.Value is CardContainer cardContainer && cardContainer.ExpectedInteractCardDefType != typeof(ItemCardDef)) return false;
        if (Battle.Player.Energy.Value < itemCardDef.Cost) return false;
        return true;
    }

    public override void Interact(CardNode node)
    {
        ChooseTargets(node);
    }

    public virtual void ChooseTargets(CardNode node)
    {
        Use(node);
    }

    public virtual void Use(CardNode node)
    {
        var interactCardDef = (ItemCardDef)Def;
        ChangeRank(interactCardDef.RankChangePerUse);
        Battle.Player.Energy.Value -= interactCardDef.Cost;
    }
}