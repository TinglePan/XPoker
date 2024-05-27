﻿using System.Collections.Generic;
using Godot;
using XCardGame.Scripts.Buffs;
using XCardGame.Scripts.Common;
using XCardGame.Scripts.Common.DataBinding;

namespace XCardGame.Scripts.Nodes;

public partial class BuffNode: BaseContentNode<BuffNode, BaseBuff>
{
    [Export]
    public TextureRect Icon;
    
    public override void _ExitTree()
    {
        base._ExitTree();
        Content.Value = null;
    }

    public override void Setup(Dictionary<string, object> args)
    {
        base.Setup(args);
        Content.Value = (BaseBuff)args["buff"];
    }
    
    protected override void OnContentAttached(BaseBuff buff)
    {
        base.OnContentAttached(buff);
        Icon.Texture = ResourceCache.Instance.Load<Texture2D>(buff.IconPath);
        buff.OnStart(buff.Battle);
    }

    protected override void OnContentDetached(BaseBuff buff)
    {
        base.OnContentDetached(buff);
        Icon.Texture = null;
        buff.OnStop(buff.Battle);
    }
}