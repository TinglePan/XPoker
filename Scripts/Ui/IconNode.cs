﻿using Godot;

namespace XCardGame.Ui;

public partial class IconNode: BaseContentNode
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
        var iconContent = (IContentContractGetIcon<Texture2D>)content;
        Icon.Texture = iconContent.GetIcon();
    }

    protected override void OnContentDetached(IContent content)
    {
        base.OnContentDetached(content);
        Icon.Texture = null;
    }
}