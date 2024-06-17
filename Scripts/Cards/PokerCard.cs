using XCardGame.Scripts.Common;
using XCardGame.Scripts.Common.Constants;
using XCardGame.Scripts.Common.DataBinding;
using XCardGame.Scripts.Defs;
using XCardGame.Scripts.GameLogic;

namespace XCardGame.Scripts.Cards;

public class PokerCard: MarkerCard
{
    
    public static string GetCardName(Enums.CardSuit suit, Enums.CardRank rank)
    {
        return $"{rank} of {suit}";
    }
    
    public PokerCard(BaseCardDef def) : base(def)
    {
        def.Name = GetCardName(def.Suit, def.Rank);
        def.DescriptionTemplate = def.Name;
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
    
    protected virtual void OnSuitChanged(object sender, ValueChangedEventDetailedArgs<Enums.CardSuit> args)
    {
        IconPath.Value = Utils.GetCardTexturePath(args.NewValue);
    }

}