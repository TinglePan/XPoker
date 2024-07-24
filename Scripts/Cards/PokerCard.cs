using XCardGame.Common;

namespace XCardGame;

public class PokerCard: BaseCard
{
	
	public static string GetCardName(Enums.CardSuit suit, Enums.CardRank rank)
	{
		return $"{rank} of {suit}";
	}

	protected bool AlreadyFunctioning;
	
	public PokerCard(BaseCardDef def): base(def)
	{
		def.Name = GetCardName(def.Suit, def.Rank);
		def.DescriptionTemplate = def.Name;
		Suit.DetailedValueChanged += OnSuitChanged;
	}

	public override void OnStartEffect(Battle battle)
	{
		base.OnStartEffect(battle);
		if (IsFunctioning() && !AlreadyFunctioning)
		{
			AlreadyFunctioning = true;
		}
	}
	
	public override void OnStopEffect(Battle battle)
	{
		base.OnStopEffect(battle);
		if (AlreadyFunctioning)
		{
			AlreadyFunctioning = false;
		}
	}
	
	public override string ToString()
	{
		return Description();
	}
	
	public override string Description()
	{
		return string.Format(Def.DescriptionTemplate, Rank.Value, Suit.Value);
	}

	protected void OnSuitChanged(object sender, ValueChangedEventDetailedArgs<Enums.CardSuit> args)
	{
		IconPath.Value = Utils.GetCardTexturePath(args.NewValue);
	}
}