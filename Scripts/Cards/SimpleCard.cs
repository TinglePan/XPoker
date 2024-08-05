using XCardGame.Common;

namespace XCardGame;

public class SimpleCard: BaseCard
{
	public SimpleCard(CardDef def): base(def)
	{
		def.Name = GetCardName();
		Suit.DetailedValueChanged += OnSuitChanged;
		Suit.FireValueChangeEventsOnInit();
	}
	
	public override string ToString()
	{
		return Description();
	}
	
	public override string Description()
	{
		return GetCardName();
	}

	protected string GetCardName()
	{
		return string.Format(Def.DescriptionTemplate, Def.Rank, Def.Suit);
	}

	protected void OnSuitChanged(object sender, ValueChangedEventDetailedArgs<Enums.CardSuit> args)
	{
		IconPath.Value = Utils.GetCardTexturePath(args.NewValue);
	}
}