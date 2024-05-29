﻿using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using Godot;
using XCardGame.Scripts.Buffs;
using XCardGame.Scripts.GameLogic;

namespace XCardGame.Scripts.Nodes;

public partial class BuffContainer: ContentContainer<BuffNode, BaseBuff>
{
    [Export]
    public PackedScene BuffPrefab;

    protected Battle Battle;
    
    public override void Setup(Dictionary<string, object> args)
    {
        base.Setup(args);
        Battle = GameMgr.CurrentBattle;
        if (args["buffs"] is ObservableCollection<BaseBuff> buffs && buffs != Contents)
        {
            Contents = buffs;
            Contents.CollectionChanged += OnContentsChanged;
        }
    }

    public override void OnM2VAddContents(int startingIndex, IList contents)
    {
        EnsureSetup();
        var index = startingIndex;
        foreach (var content in contents)
        {
            if (content is BaseBuff buff && (index >= ContentNodes.Count || ContentNodes[index].Content.Value != content))
            {
                var buffNode = BuffPrefab.Instantiate<BuffNode>();
                AddChild(buffNode);
                MoveChild(buffNode, index);
                index++;
                buffNode.Setup(new Dictionary<string, object>()
                {
                    { "buff", buff },
                    { "container", this }
                });
                buff.Setup(new Dictionary<string, object>()
                {
                    { "gameMgr", GameMgr },
                    { "battle", Battle },
                    { "node", buffNode }
                });
            }
        }
    }
}