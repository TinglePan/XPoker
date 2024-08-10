using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
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
    public static void LoadDebugCommands()
    {
        DC.ChangeContext("/root/BattleScene/DebugProxy");
    }

    [ConsoleCommand]
    public void Reward(string defName)
    {
        var defs = new List<CardDef>();
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
            InitReRollPrice = Configuration.DefaultReRollPrice,
            ReRollPriceIncrease = Configuration.DefaultReRollPriceIncrease,
            SkipReward = Configuration.DefaultSkipReward,
            PassInDefs = defs
        });
    }

    [ConsoleCommand]
    public void SetTopCard(string cardName)
    {
        var fieldInfo = typeof(CardDefs).GetField(cardName, BindingFlags.Static | BindingFlags.Public);
        if (fieldInfo == null)
        {
            GD.PrintErr($"[{cardName}] not found in CardDefs");
            return;
        }
        var cardDef = (CardDef)fieldInfo.GetValue(null);
        Debug.Assert(cardDef != null, $"[{cardName}] cast failed");
        var card = CardFactory.CreateInstance(cardDef.ConcreteClassPath, cardDef);
        GameMgr.CurrentBattle.Dealer.DealCardPile.Cards[0] = card;
    }
    
}