using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using Godot;
using XCardGame.Scripts.Buffs;

namespace XCardGame.Scripts.Nodes;

public partial class BuffContainer: BaseContentContainer<BuffNode, BaseBuff>
{
    [Export]
    public PackedScene BuffPrefab;
    
    public override void Setup(Dictionary<string, object> args)
    {
        base.Setup(args);
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
            if (content is BaseBuff buff)
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
            }
        }
    }
}