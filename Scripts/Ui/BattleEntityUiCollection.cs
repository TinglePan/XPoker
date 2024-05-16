using System.Collections.Generic;
using System.Collections.Specialized;
using Godot;
using XCardGame.Scripts.Cards;

using XCardGame.Scripts.GameLogic;

namespace XCardGame.Scripts.Ui;

public partial class BattleEntityUiCollection : Control, ISetup, IManagedUi
{
	[Export] public BattleEntityInfoUi BattleEntityInfoUi;
	[Export] public CardContainer HoleCardContainer;
	[Export] public BuffContainer BuffContainer;
	
	[Export]
	public string Identifier { get; set; }
	
	public GameMgr GameMgr { get; private set; }
	public UiMgr UiMgr { get; private set; }
	
	public BattleEntity Entity;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		GameMgr = GetNode<GameMgr>("/root/GameMgr");
		UiMgr = GetNode<UiMgr>("/root/UiMgr");
		UiMgr.Register(this);
		// GD.Print($"self is {this}");
		// GD.Print($"BattleEntityInfoUi is {BattleEntityInfoUi}");
		// GD.Print($"HoleCardContainer is {HoleCardContainer}");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public virtual void Setup(Dictionary<string, object> args)
	{
		Entity = (BattleEntity)args["entity"];
		// GD.Print($"BattleEntityUiCollection Setup {this}");
		// GD.Print($"BattleEntityUiCollection Setup BattleEntityInfoUi is {BattleEntityInfoUi}");
		// GD.Print($"BattleEntityUiCollection Setup HoleCardContainer is {HoleCardContainer}");
		if (Entity != null)
		{
			HoleCardContainer.Setup(new Dictionary<string, object>()
			{
				{ "cards", Entity.HoleCards }
			});
			BattleEntityInfoUi.Setup(args);
			BuffContainer.Setup(new Dictionary<string, object>()
			{
				{ "buffs", Entity.Buffs },
			});
		}
	}
}