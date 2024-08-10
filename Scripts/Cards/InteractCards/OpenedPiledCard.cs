using XCardGame.CardProperties;
using XCardGame.Common;

namespace XCardGame;

public class OpenedPiledCard: BaseCard
{
    public static CardDef GetDef(CardDef piledCardDef)
    {
        var res = piledCardDef.Clone<CardDef>();
        res.IsPiled = false;
        res.IsExhaust = true;
        return res;
    }

    public BaseCard PiledCard;
    public OpenedPiledCard(BaseCard piledCard) : base(GetDef(piledCard.Def))
    {
        PiledCard = piledCard;
        Rank.Value = Enums.CardRank.None;
    }

    public override void OnEnterField()
    {
        
    }
    
    public override void OnLeaveField()
    {
        
    }

    protected override void SetupProps()
    {
        foreach (var (type, prop) in PiledCard.Props)
        {
            if (prop is not CardPropPiled)
            {
                Props.Add(type, prop);
            }
            if (prop is BaseCardPropUsable usable)
            {
                usable.Enabled = true;
            }
        }
    }
}