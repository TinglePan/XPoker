using System.Collections.Generic;
using Godot;
using XCardGame.Scripts.Common;
using XCardGame.Scripts.Common.DataBinding;
using XCardGame.Scripts.GameLogic;

namespace XCardGame.Scripts.Ui;

public partial class BattleEntityInfoUi : Control, ISetup, IManagedUi
{

	[Export]
	public string Identifier { get; set; }
	
	public GameMgr GameMgr { get; private set; }
	public UiMgr UiMgr { get; private set; }
	
	// FIXME: Exporting these fields cause weird problems
	public Label NameLabel;
	// public TextureProgressBar MoraleBar;
	public Label MoraleLabel;
	public Label MaxMoraleLabel;
	public TextureRect Portrait;
	
	
	public BattleEntity Entity;

	public override void _Ready()
	{
		GameMgr = GetNode<GameMgr>("/root/GameMgr");
		UiMgr = GetNode<UiMgr>("/root/UiMgr");
		UiMgr.Register(this);
		NameLabel = GetNode<Label>("Panel/Portrait/Name");
		// MoraleBar = GetNode<TextureProgressBar>("Panel/MoraleBar");
		MoraleLabel = GetNode<Label>("Panel/Morale");
		MaxMoraleLabel = GetNode<Label>("Panel/MaxMorale");
		Portrait = GetNode<TextureRect>("Panel/Portrait");
	}

	public override void _ExitTree()
	{
		base._ExitTree();
		if (Entity != null)
		{
			Entity.Morale.DetailedValueChanged -= OnMoraleChanged;
			Entity.MaxMorale.DetailedValueChanged -= OnMaxMoraleChanged;
		}
	}

	public void Setup(Dictionary<string, object> args)
	{
		Entity = (BattleEntity)args["entity"];
		if (Entity != null)
		{
			// GD.Print($"BattleEntityInfoUi Setup {NameLabel} to text {Entity.DisplayName}");
			NameLabel.Text = Entity.DisplayName;
			Portrait.Texture = ResourceCache.Instance.Load<Texture2D>(Entity.PortraitPath);
			Entity.Morale.DetailedValueChanged += OnMoraleChanged;
			Entity.Morale.FireValueChangeEventsOnInit();
			Entity.MaxMorale.DetailedValueChanged += OnMaxMoraleChanged;
			Entity.MaxMorale.FireValueChangeEventsOnInit();
		}
	}
	
	
	protected void OnMoraleChanged(object sender, ValueChangedEventDetailedArgs<int> args)
	{
		// MoraleBar.Value = args.NewValue;
		MoraleLabel.Text = args.NewValue.ToString();
	}
	
	protected void OnMaxMoraleChanged(object sender, ValueChangedEventDetailedArgs<int> args)
	{
		// MoraleBar.MaxValue = args.NewValue;
		MaxMoraleLabel.Text = args.NewValue.ToString();
	}
}