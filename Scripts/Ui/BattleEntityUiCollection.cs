using System.Collections.Generic;
using System.Collections.Specialized;
using Godot;
using XCardGame.Scripts.Cards;
using XCardGame.Scripts.Cards.PokerCards;
using XCardGame.Scripts.GameLogic;

namespace XCardGame.Scripts.Ui;

public partial class BattleEntityUiCollection : Node, ISetup
{
	[Export] public BattleEntityInfoUi BattleEntityInfoUi;
	[Export] public CardContainer HoleCardContainer;
	
	public BattleEntity Entity;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void Setup(Dictionary<string, object> args)
	{
		Entity = (BattleEntity)args["entity"];
		if (Entity != null)
		{
			HoleCardContainer.Setup(new Dictionary<string, object>()
			{
				{ "cards", Entity.HoleCards }
			});
			BattleEntityInfoUi.Setup(args);
		}
	}
}