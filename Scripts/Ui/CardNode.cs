using System.Collections.Generic;
using Godot;
using XCardGame.Scripts.Cards;
using XCardGame.Scripts.Common.Constants;
using XCardGame.Scripts.Common.DataBinding;

namespace XCardGame.Scripts.Ui;

public partial class CardNode: Control, ISetup
{
	[Export] public Control Front;
    [Export] public Control Back;
 
    public ObservableProperty<BaseCard> Card;
    
    public override void _ExitTree()
	{
	 	base._ExitTree();
	 	if (Card is { Value: not null })
	 	{
	 		Card.Value.Face.DetailedValueChanged -= OnCardFaceChanged;
	 	}
	}

	public virtual void Setup(Dictionary<string, object> args)
    {
	    Card = new ObservableProperty<BaseCard>(nameof(Card), null);
	    Card.DetailedValueChanged += OnCardChanged;
	    if (args["card"] is BaseCard card)
     	{
	        Card.Value = card;
	        card.Node = this;
     	}
    }
    
    public void OnCardFaceChanged(object sender, ValueChangedEventDetailedArgs<Enums.CardFace> args)
    {
     	if (args.NewValue == Enums.CardFace.Down)
     	{
     		Front.Visible = false;
     		Back.Visible = true;
     	}
     	else
     	{
     		Front.Visible = true;
     		Back.Visible = false;
     	}
    }
    
    public virtual void OnCardChanged(object sender, ValueChangedEventDetailedArgs<BaseCard> args)
	{
		args.NewValue.Face.DetailedValueChanged += OnCardFaceChanged;
		args.NewValue.Face.FireValueChangeEventsOnInit();
	}
    
    
}