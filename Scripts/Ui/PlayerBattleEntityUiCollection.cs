using System.Collections.Generic;
using System.Diagnostics;
using Godot;
using XCardGame.Scripts.GameLogic;

namespace XCardGame.Scripts.Ui;

public partial class PlayerBattleEntityUiCollection: BattleEntityUiCollection
{
    [Export]
    public CardContainer AbilityCardContainer;

    public override void Setup(Dictionary<string, object> args)
    {
        base.Setup(args);
        // GD.Print($"Self is {this}");
        // GD.Print($"BattleEntityInfoUi2 is {BattleEntityInfoUi}");
        // GD.Print($"HoleCardContainer2 is {HoleCardContainer}");
        var playerEntity = Entity as PlayerBattleEntity;
        Debug.Assert(playerEntity != null);
        AbilityCardContainer.Setup(new Dictionary<string, object>()
        {
            { "cards", playerEntity.AbilityCards }
        });
    }
    
}