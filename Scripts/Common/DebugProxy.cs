using System.Collections.Generic;
using Godot;
using hamsterbyte.DeveloperConsole;
using XCardGame.Scripts.Common.Constants;
using XCardGame.Scripts.Defs;
using XCardGame.Scripts.Defs.Def.Card;
using XCardGame.Scripts.Game;

namespace XCardGame.Scripts.Common;

public partial class DebugProxy: Node
{
    public GameMgr GameMgr;
    public PackedScene SelectRewardCardScene;
    
    public override void _Ready()
    {
        base._Ready();
        GameMgr = GetNode<GameMgr>("/root/GameMgr");
        SelectRewardCardScene = ResourceCache.Instance.Load<PackedScene>("res://Scenes/SelectRewardCard.tscn");
    }

    [ConsoleCommand]
    public void Reward(string defName)
    {
        var defs = new List<BaseCardDef>();
        foreach (var def in CardDefs.All())
        {
            if (def.Name == defName)
            {
                defs.Add(def);
                break;
            }
        }
        var selectRewardCard = GameMgr.OverlayScene(SelectRewardCardScene) as SelectRewardCard;
        selectRewardCard.Setup(new System.Collections.Generic.Dictionary<string, object>()
        {
            { "rewardCardCount", defs.Count },
            { "rewardCardDefType", typeof(InteractCardDef) },
            { "reRollPrice", Configuration.DefaultReRollPrice },
            { "reRollPriceIncrease", Configuration.DefaultReRollPriceIncrease },
            { "skipReward", Configuration.DefaultSkipReward },
            { "defs", defs }
        });
    }
}