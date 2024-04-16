using System.Collections.Generic;
using Godot;
using XCardGame.Scripts.Common.DataBinding;

namespace XCardGame.Scripts.Ui;

public partial class PlayerInfoTab : Node, ISetup
{
	[Export] public Label PlayerNameLabel;
	[Export] public Label PlayerCashLabel;
	[Export] public Label PlayerBetLabel;
	
	public PokerPlayer Player;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public override void _ExitTree()
	{
		base._ExitTree();
		if (Player != null)
		{
			Player.NChipsInHand.DetailedValueChanged -= OnPlayerCashChanged;
			Player.NChipsInPot.DetailedValueChanged -= OnPlayerBetChanged;
		}
	}

	public void Setup(Dictionary<string, object> args)
	{
		Player = args["player"] as PokerPlayer;
		if (Player != null)
		{
			PlayerNameLabel.Text = Player.Creature.Name;
			PlayerCashLabel.Text = Player.NChipsInHand.Value.ToString();
			PlayerBetLabel.Text = Player.NChipsInPot.Value.ToString();
			Player.NChipsInHand.DetailedValueChanged += OnPlayerCashChanged;
			Player.NChipsInPot.DetailedValueChanged += OnPlayerBetChanged;
		}
	}
	
	protected void OnPlayerCashChanged(object sender, ValueChangedEventDetailedArgs<int> args)
	{
		PlayerCashLabel.Text = args.NewValue.ToString();
	}
	
	protected void OnPlayerBetChanged(object sender, ValueChangedEventDetailedArgs<int> args)
	{
		PlayerBetLabel.Text = args.NewValue.ToString();
	}
	
}