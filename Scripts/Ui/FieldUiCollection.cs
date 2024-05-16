using System.Collections.Generic;
using System.Collections.Specialized;
using Godot;
using XCardGame.Scripts.Cards;
using XCardGame.Scripts.Common.DataBinding;
using XCardGame.Scripts.GameLogic;

namespace XCardGame.Scripts.Ui;

public partial class FieldUiCollection : Control, ISetup, IManagedUi
{
	[Export] public CardContainer AbilityCardContainer;
	[Export] public Label CurrentCostLabel;
	[Export] public Label MaxCostLabel;
	[Export] public Label CurrentConcentrationLabel;
	[Export] public Label MaxConcentrationLabel;
	
	[Export]
	public string Identifier { get; set; }
	
	public GameMgr GameMgr { get; private set; }
	public UiMgr UiMgr { get; private set; }
	
	public PlayerBattleEntity Player;
	
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

	public override void _ExitTree()
	{
		base._ExitTree();
		if (Player != null)
		{
			Player.Cost.DetailedValueChanged -= OnPlayerCostChanged;
			Player.MaxCost.DetailedValueChanged -= OnPlayerMaxCostChanged;
			Player.Concentration.DetailedValueChanged -= OnPlayerConcentrationChanged;
			Player.MaxConcentration.DetailedValueChanged -= OnPlayerMaxConcentrationChanged;
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void Setup(Dictionary<string, object> args)
	{
		Player = (PlayerBattleEntity)args["player"];
		// GD.Print($"BattleEntityUiCollection Setup {this}");
		// GD.Print($"BattleEntityUiCollection Setup BattleEntityInfoUi is {BattleEntityInfoUi}");
		// GD.Print($"BattleEntityUiCollection Setup HoleCardContainer is {HoleCardContainer}");
		if (Player != null)
		{
			AbilityCardContainer.Setup(new Dictionary<string, object>()
			{
				{ "cards", Player.AbilityCards }
			});
			Player.Cost.DetailedValueChanged += OnPlayerCostChanged;
			Player.Cost.FireValueChangeEventsOnInit();
			Player.MaxCost.DetailedValueChanged += OnPlayerMaxCostChanged;
			Player.MaxCost.FireValueChangeEventsOnInit();
			Player.Concentration.DetailedValueChanged += OnPlayerConcentrationChanged;
			Player.Concentration.FireValueChangeEventsOnInit();
			Player.MaxConcentration.DetailedValueChanged += OnPlayerMaxConcentrationChanged;
			Player.MaxConcentration.FireValueChangeEventsOnInit();
		}
	}
	
	protected void OnPlayerCostChanged(object sender, ValueChangedEventDetailedArgs<int> args)
	{
		CurrentCostLabel.Text = args.NewValue.ToString();
	}
	
	protected void OnPlayerMaxCostChanged(object sender, ValueChangedEventDetailedArgs<int> args)
	{
		MaxCostLabel.Text = args.NewValue.ToString();
	}
	
	protected void OnPlayerConcentrationChanged(object sender, ValueChangedEventDetailedArgs<int> args)
	{
		CurrentConcentrationLabel.Text = args.NewValue.ToString();
	}
	
	protected void OnPlayerMaxConcentrationChanged(object sender, ValueChangedEventDetailedArgs<int> args)
	{
		MaxConcentrationLabel.Text = args.NewValue.ToString();
	}
}