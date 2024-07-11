using XCardGame.Scripts.Buffs;
using XCardGame.Scripts.Common;
using XCardGame.Scripts.Defs.Def.Card;
using XCardGame.Scripts.Ui;

namespace XCardGame.Scripts.Cards.InteractCards.EquipmentCards;

public class BigShieldCard: BaseItemCard
{
    
    public class BigShieldBuff: BaseBuff
    {
        public BigShieldBuff() : base(Utils._("Big shield"), Utils._("Deal and receive no damage for this round"), 
            "res://Sprites/BuffIcons/big_shield", isTemporary:true)
        {
        }
    }

    public BigShieldCard(InteractCardDef def) : base(def)
    {
        
    }

    public override void Use(CardNode node)
    {
        base.Use(node);
        Battle.InflictBuffOn(new BigShieldBuff(), OwnerEntity, OwnerEntity, this);
    }
}