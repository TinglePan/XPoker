using System.Collections.Generic;
using Godot;
using XCardGame.Scripts.Cards;
using XCardGame.Scripts.Defs;
using XCardGame.Scripts.GameLogic;

namespace XCardGame.Scripts.Common;

public partial class DebugEntry: Node
{
    public GameMgr GameMgr;
    public Battle Battle;

    protected List<BaseCard> NextRewardCard;

    public override void _Ready()
    {
        base._Ready();
        GameMgr = GetNode<GameMgr>("/root/GameMgr");
        NextRewardCard = new List<BaseCard>();
    }

    public void AddToNextReward(BaseCardDef def)
    {
        var card = CardFactory.CreateInstance(def.ConcreteClassPath, def);
        NextRewardCard.Add(card);
        
    }
}