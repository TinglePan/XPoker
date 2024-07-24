using XCardGame.Common;
using XCardGame.Ui;

namespace XCardGame;

public class BigShieldCard: BaseItemCard
{
    
    public class BigShieldBuff: BaseBuff
    {
        public BigShieldBuff() : base(Utils._("Big shield"), Utils._("Deal and receive no damage for this round"), 
            "res://Sprites/BuffIcons/big_shield", isTemporary:true)
        {
        }
    }

    public BigShieldCard(ItemCardDef def) : base(def)
    {
        
    }

    public override void Use(CardNode node)
    {
        base.Use(node);
        Battle.InflictBuffOn(new BigShieldBuff(), OwnerEntity, OwnerEntity, this);
    }
}