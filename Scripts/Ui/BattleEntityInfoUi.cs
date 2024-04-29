using System.Collections.Generic;
using Godot;
using XCardGame.Scripts.Common.DataBinding;
using XCardGame.Scripts.GameLogic;

namespace XCardGame.Scripts.Ui;

public partial class BattleEntityInfoUi : Node, ISetup
{
	[Export] public Label NameLabel;
	[Export] public TextureProgressBar MoralBar;
	
	public BattleEntity Entity;

	public void Setup(Dictionary<string, object> args)
	{
		Entity = (BattleEntity)args["entity"];
		if (Entity != null)
		{
			NameLabel.Text = Entity.Name;
			// TODO: bind morale
		}
	}
}