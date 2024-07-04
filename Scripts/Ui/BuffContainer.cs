using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Godot;
using XCardGame.Scripts.Buffs;
using XCardGame.Scripts.Common;
using XCardGame.Scripts.Game;

namespace XCardGame.Scripts.Ui;

public partial class BuffContainer: ContentContainer<BuffNode, BaseBuff>
{ 
    public PackedScene BuffPrefab;

    protected Battle Battle;

    public override void _Ready()
    {
        base._Ready();
        BuffPrefab = ResourceCache.Instance.Load<PackedScene>("res://Scenes/Buff.tscn");
    }

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

    protected override void OnM2VAddContents(int startingIndex, IList contents)
    {
        EnsureSetup();
        SuppressNotifications = true;
        var index = startingIndex;
        foreach (var content in contents)
        {
            if (content is BaseBuff buff && (index >= ContentNodes.Count || ContentNodes[index].Content.Value != content))
            {
                var buffNode = BuffPrefab.Instantiate<BuffNode>();
                AddChild(buffNode);
                buffNode.Setup(new Dictionary<string, object>()
                {
                    { "buff", buff },
                    { "container", this }
                });
                buff.Setup(new Dictionary<string, object>()
                {
                    { "gameMgr", GameMgr },
                    { "node", buffNode }
                });
                OnAddContentNode?.Invoke(buffNode);
                ContentNodes.Insert(index, buffNode);
            }
            index++;
        }
        SuppressNotifications = false;
    }
}