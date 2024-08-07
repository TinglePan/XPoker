﻿using System.Collections;
using Godot;
using XCardGame.Common;

namespace XCardGame.Ui;

public partial class IconContainer: BaseContentContainer
{
    public PackedScene IconPrefab;

    public override void _Ready()
    {
        base._Ready();
        IconPrefab = ResourceCache.Instance.Load<PackedScene>("res://Scenes/IconInContainer.tscn");
    }

    protected override void OnM2VAddContents(int startingIndex, IList contents)
    {
        SuppressNotifications = true;
        var index = startingIndex;
        foreach (var content in contents)
        {
            if (content is IContentContractGetIcon<Texture2D> iconContent)
            {
                var iconNode = IconPrefab.Instantiate<IconNode>();
                AddChild(iconNode);
                iconNode.Setup(new BaseContentNode.SetupArgs
                {
                    Content = iconContent,
                    Container = this,
                    HasPhysics = true
                });
                iconContent.Nodes.Add(iconNode);
                ContentNodes.Insert(index, iconNode);
                OnAddContentNode?.Invoke(iconNode);
            }
            index++;
        }
        AdjustLayout();
        SuppressNotifications = false;
    }
}