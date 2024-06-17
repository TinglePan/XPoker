using XCardGame.Scripts.Buffs;
using XCardGame.Scripts.Common.Constants;
using XCardGame.Scripts.Defs;
using XCardGame.Scripts.GameLogic;

namespace XCardGame.Scripts.Cards.AbilityCards;

public class BigShieldCard: BaseTapCard
{
    
    public class BigShieldBuff: BaseBuff
    {
        public BigShieldBuff() : base("Big shield", "Deal and receive no damage for this round", 
            "res://Sprites/BuffIcons/big_shield", isTemporary:true)
        {
        }
    }

    public BigShieldCard(TapCardDef def) : base(def)
    {
    }
    
    public override void OnStart(Battle battle)
    {
        base.OnStart(battle);
        Battle.OnRoundEnd += OnRoundEnd;
    }
    
    public override void OnStop(Battle battle)
    {
        base.OnStop(battle);
        Battle.OnRoundEnd -= OnRoundEnd;
    }

    public override void ToggleTap()
    {
        base.ToggleTap();
        if (!IsTapped)
        {
            Battle.InflictBuffOn(new BigShieldBuff(), OwnerEntity, OwnerEntity, this);
            Rank.Value = Def.Rank;
        }
        else
        {
            Rank.Value = Enums.CardRank.Ace;
        }
    }

    protected void OnRoundEnd(Battle battle)
    {
        if (!IsTapped)
        {
            ToggleTap();
        }
    }
}