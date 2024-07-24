using System.Collections.Generic;
using Godot;
using hamsterbyte.DeveloperConsole;


namespace XCardGame.Common;

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
        selectRewardCard.Setup(new SelectRewardCard.SetupArgs
        {
            RewardCardCount = defs.Count,
            RewardCardDefType = typeof(InteractCardDef),
            InitReRollPrice = Configuration.DefaultReRollPrice,
            ReRollPriceIncrease = Configuration.DefaultReRollPriceIncrease,
            SkipReward = Configuration.DefaultSkipReward,
            PassInDefs = defs
        });
    }
}