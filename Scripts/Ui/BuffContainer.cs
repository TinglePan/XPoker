using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Godot;
using XCardGame.Scripts.Buffs;
using XCardGame.Scripts.Common;
using XCardGame.Scripts.Game;

namespace XCardGame.Scripts.Ui;

public partial class BuffContainer: BaseContentContainer
{ 
    public PackedScene BuffPrefab;

    public List<BaseBuff> Buffs => Contents.Cast<BaseBuff>().ToList();

    protected Battle Battle;

    public override void _Ready()
    {
        base._Ready();
        BuffPrefab = ResourceCache.Instance.Load<PackedScene>("res://Scenes/Buff.tscn");
    }

    public override void Setup(SetupArgs args)
    {
        base.Setup(args);
        Battle = GameMgr.CurrentBattle;
    }

    protected override void OnM2VAddContents(int startingIndex, IList contents)
    {
        SuppressNotifications = true;
        var index = startingIndex;
        foreach (var content in contents)
        {
            if (content is BaseBuff buff && (index >= ContentNodes.Count || ContentNodes[index].Content.Value != content))
            {
                var buffNode = BuffPrefab.Instantiate<BuffNode>();
                AddChild(buffNode);
                buffNode.Setup(new BaseContentNode.SetupArgs()
                {
                    Content = buff,
                    Container = this,
                });
                buff.Setup(new BaseBuff.SetupArgs
                {
                    GameMgr = GameMgr,
                    Node = buffNode,
                });
                ContentNodes.Insert(index, buffNode);
                OnAddContentNode?.Invoke(buffNode);
            }
            index++;
        }
        SuppressNotifications = false;
    }
}