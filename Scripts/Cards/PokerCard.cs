using XCardGame.Scripts.Common;
using XCardGame.Scripts.Common.Constants;
using XCardGame.Scripts.Common.DataBinding;
using XCardGame.Scripts.GameLogic;

namespace XCardGame.Scripts.Cards;

public class PokerCard: MarkerCard
{
    public PokerCard(string texturePath, Enums.CardSuit suit, Enums.CardRank rank, BattleEntity ownerEntity = null) : 
        base(GetCardName(suit, rank), GetCardName(suit, rank), texturePath, suit, rank, ownerEntity)
    {
    }
    
    public override void OnStart(Battle battle)
    {
        base.OnStart(battle);
        Suit.DetailedValueChanged += OnSuitChanged;
    }

    public override void OnStop(Battle battle)
    {
        base.OnStop(battle);
        Suit.DetailedValueChanged -= OnSuitChanged;
    }
    
    protected static string GetCardName(Enums.CardSuit suit, Enums.CardRank rank)
    {
        return $"{rank} of {suit}";
    }
    
    protected virtual void OnSuitChanged(object sender, ValueChangedEventDetailedArgs<Enums.CardSuit> args)
    {
        IconPath.Value = Utils.GetCardTexturePath(args.NewValue);
    }

}