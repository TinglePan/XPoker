using XCardGame.Scripts.Buffs;
using XCardGame.Scripts.Common;
using XCardGame.Scripts.Common.Constants;
using XCardGame.Scripts.Defs;
using XCardGame.Scripts.Defs.Def.Card;
using XCardGame.Scripts.GameLogic;
using XCardGame.Scripts.Nodes;

namespace XCardGame.Scripts.Cards.AbilityCards;

public class BigShieldCard: BaseUseCard
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