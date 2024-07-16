﻿using System.Collections.Generic;
using Godot;
using XCardGame.Scripts.Buffs;
using XCardGame.Scripts.Common;

namespace XCardGame.Scripts.Ui;

public partial class BuffNode: BaseContentNode
{
    public Sprite2D Icon;

    public override void _Ready()
    {
        base._Ready();
        Icon = GetNode<Sprite2D>("Icon");
    }

    public override void _Notification(int what)
    {
        if (what == NotificationPredelete && Content.Value != null)
        {
            Content.Value = null;
        }
    }
    
    protected override void OnContentAttached(IContent content)
    {
        base.OnContentAttached(content);
        var buff = (BaseBuff)content;
        Icon.Texture = ResourceCache.Instance.Load<Texture2D>(buff.IconPath);
        buff.OnStart(buff.Battle);
    }

    protected override void OnContentDetached(IContent content)
    {
        base.OnContentDetached(content);
        Icon.Texture = null;
        var buff = (BaseBuff)content;
        buff.OnStop(buff.Battle);
    }
}