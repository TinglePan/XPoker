﻿using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Numerics;
using System.Threading.Tasks;
using Godot;
using XCardGame.Scripts.Common;
using XCardGame.Scripts.Common.Constants;
using Vector2 = Godot.Vector2;

namespace XCardGame.Scripts.Nodes;

public abstract partial class ContentContainer<TContentNode, TContent>: Node2D, ISetup
    where TContentNode: BaseContentNode<TContentNode, TContent>
    where TContent: IContent<TContent>
{
    public GameMgr GameMgr;
    public NinePatchRect Border;
    public bool HasSetup { get; set; }

    public ObservableCollection<TContent> Contents;
    public ObservableCollection<TContentNode> ContentNodes;

    public Vector2 ContentNodeSize;
    public Vector2 Separation;
    public Enums.Direction2D8Ways PivotDirection;
    public int NodesPerRow;
    
    protected bool SuppressNotifications;
    
    public override void _Ready()
    {
        base._Ready();
        ClearChildren();
        GameMgr = GetNode<GameMgr>("/root/GameMgr");
        Contents = new ObservableCollection<TContent>();
        Contents.CollectionChanged += OnContentsChanged;
        ContentNodes = new ObservableCollection<TContentNode>();
        ContentNodes.CollectionChanged += OnContentNodesChanged;
        SuppressNotifications = false;
        HasSetup = false;
    }

    public virtual void Setup(Dictionary<string, object> args)
    {
        ContentNodeSize = (Vector2)args["contentNodeSize"];
        Separation = (Vector2)args["separation"];
        PivotDirection = (Enums.Direction2D8Ways)args["pivotDirection"]; 
        NodesPerRow = (int)args["nodesPerRow"];
        HasSetup = true;
    }
    
    public void EnsureSetup()
    {
        if (!(HasSetup && Contents != null))
        {
            GD.PrintErr($"{this} not setup yet");
        }
    }
    
    protected abstract void OnM2VAddContents(int startingIndex, IList contents);

    protected void OnM2VRemoveContents(int startingIndex, IList contents)
    {
        EnsureSetup();
        SuppressNotifications = true;
        for (int i = 0; i < contents.Count; i++)
        {
            ContentNodes.RemoveAt(startingIndex + i);
        }
        SuppressNotifications = false;
    }

    protected void OnM2VReplaceContents(int oldContentsStartingIndex, IList oldContents, IList newContents)
    {
        EnsureSetup();
        for (int i = 0; i < oldContents.Count; i++)
        {
            if (oldContents[i] is TContent oldContent && newContents[i] is TContent newContent)
            {
                var oldContentNode = ContentNodes[oldContentsStartingIndex + i];
                oldContentNode.Content.Value = newContent;
            }
        }
    }

    protected virtual void OnV2MAddNodes(int startingIndex, IList nodes)
    {
        EnsureSetup();
        SuppressNotifications = true;
        var index = startingIndex;
        foreach (var node in nodes)
        {
            if (node is TContentNode contentNode)
            {
                Contents.Insert(index, contentNode.Content.Value);
                contentNode.Reparent(this);
                MoveChild(contentNode, index);
                contentNode.Container = this;
                // GD.Print($"{contentNode.Content.Value}({contentNode}) container added {this}");
            }
            index++;
        }
        for (int i = 0; i < ContentNodes.Count; i++)
        {
            AdjustContentNode(i, true);
        }
        SuppressNotifications = false;
    }

    protected virtual void OnV2MRemoveNodes(int startingIndex, IList nodes)
    {
        EnsureSetup();
        SuppressNotifications = true;
        var index = startingIndex;
        foreach (var node in nodes)
        {
            if (node is TContentNode contentNode)
            {
                Contents.RemoveAt(index);
                RemoveChild(contentNode);
                contentNode.Container = null;
                // GD.Print($"{contentNode.Content.Value}({contentNode}) container removed {this}");
            }
            index++;
        }
        for (int i = 0; i < ContentNodes.Count; i++)
        {
            AdjustContentNode(i, true);
        }
        SuppressNotifications = false;
    }

    protected virtual void OnV2MReplaceNodes(int oldNodesStartingIndex, IList oldNodes, IList newNodes)
    {
        EnsureSetup();
        SuppressNotifications = true;
        for (int i = 0; i < oldNodes.Count; i++)
        {
            if (oldNodes[i] is TContentNode oldNode && newNodes[i] is TContentNode newNode)
            {
                Contents[oldNodesStartingIndex + i] = newNode.Content.Value;
                RemoveChild(oldNode);
                oldNode.Container = null;
                newNode.Reparent(newNode);
                MoveChild(newNode, oldNodesStartingIndex + i);
                newNode.Container = this;
            }
        }
        for (int i = 0; i < ContentNodes.Count; i++)
        {
            AdjustContentNode(i, true);
        }
        SuppressNotifications = false;
    }

    protected virtual void OnV2MClearNodes()
    {
        EnsureSetup();
        SuppressNotifications = true;
        ClearChildren();
        Contents.Clear();
        SuppressNotifications = false;
    }

    protected Vector2 CalculateContentNodePosition(int index, bool globalPosition = false)
    {
        var offset = CalculateContentNodeOffset(index);
        return globalPosition ? offset : GlobalPosition + offset;
    }

    protected Vector2 CalculateContentNodeOffset(int index)
    {
        var colIndex = index % NodesPerRow;
        var rowIndex = index / NodesPerRow;
        Vector2 pivotOffsetFromBottomLeft = GetPivotOffset();
        var nodeOffsetFromBottomLeft = new Vector2(colIndex * (ContentNodeSize.X + Separation.X) + ContentNodeSize.X / 2,
            rowIndex * (ContentNodeSize.Y + Separation.Y) + ContentNodeSize.Y / 2);
        var offset = nodeOffsetFromBottomLeft - pivotOffsetFromBottomLeft;
        return offset;
    }
    
    protected virtual float CalculateContentNodeRotation(int index)
    {
        return 0f;
    }

    protected virtual int ActualNodeCount()
    {
        return ContentNodes.Count;
    }
    
    protected Vector2 Size()
    {
        var actualNodeCount = ActualNodeCount();
        var actualNodesPerRow = Mathf.Min(actualNodeCount, NodesPerRow);
        var actualRowCount = actualNodeCount == 0 ? 0 : (actualNodeCount - 1) / actualNodesPerRow + 1;
        if (actualNodeCount == 0) return Vector2.Zero;
        return new Vector2(ContentNodeSize.X * actualNodesPerRow + Separation.X * (actualNodesPerRow - 1),
            ContentNodeSize.Y * actualRowCount + Separation.Y * (actualRowCount - 1));
    }
    
    protected Vector2 GetPivotOffset()
    {
        Vector2 pivotOffsetFromBottomLeft = Vector2.Zero;
        var size = Size();
        switch (PivotDirection)
        {
            case Enums.Direction2D8Ways.Neutral:
                pivotOffsetFromBottomLeft = new Vector2(size.X / 2, size.Y / 2);
                break;
            case Enums.Direction2D8Ways.Down:
                pivotOffsetFromBottomLeft = new Vector2(size.X / 2, 0);
                break;
            case Enums.Direction2D8Ways.Left:
                pivotOffsetFromBottomLeft = new Vector2(0, size.Y / 2);
                break;
            case Enums.Direction2D8Ways.Up:
                pivotOffsetFromBottomLeft = new Vector2(size.X / 2, size.Y);
                break;
            case Enums.Direction2D8Ways.Right:
                pivotOffsetFromBottomLeft = new Vector2(size.X, size.Y / 2);
                break;
            case Enums.Direction2D8Ways.DownLeft:
                pivotOffsetFromBottomLeft = new Vector2(0, 0);
                break;
            case Enums.Direction2D8Ways.UpLeft:
                pivotOffsetFromBottomLeft = new Vector2(0, size.Y);
                break;
            case Enums.Direction2D8Ways.UpRight:
                pivotOffsetFromBottomLeft = new Vector2(size.X, size.Y);
                break;
            case Enums.Direction2D8Ways.DownRight:
                pivotOffsetFromBottomLeft = new Vector2(size.X, 0);
                break;
        }
        return pivotOffsetFromBottomLeft;
    }
    
    protected void ClearChildren()
    {
        foreach (var child in GetChildren())
        {
            if (child is TContentNode)
            {
                child.QueueFree();
            }
        }
    }

    protected void OnContentsChanged(object sender, NotifyCollectionChangedEventArgs args)
    {
        if (SuppressNotifications) return;
        switch (args.Action)
        {
            case NotifyCollectionChangedAction.Add:
                if (args.NewItems != null)
                    OnM2VAddContents(args.NewStartingIndex, args.NewItems);
                break;
            case NotifyCollectionChangedAction.Remove:
                if (args.OldItems != null)
                {
                    OnM2VRemoveContents(args.OldStartingIndex, args.OldItems);
                }
                break;
            case NotifyCollectionChangedAction.Replace:
                if (args.OldItems != null && args.NewItems != null && args.OldItems.Count == args.NewItems.Count)
                {
                    OnM2VReplaceContents(args.OldStartingIndex, args.OldItems, args.NewItems);
                }
                break;
            case NotifyCollectionChangedAction.Reset:
                ClearChildren();
                break;
        }
    }
    
    protected void OnContentNodesChanged(object sender, NotifyCollectionChangedEventArgs args)
    {
        if (SuppressNotifications) return;
        switch (args.Action)
        {
            case NotifyCollectionChangedAction.Add:
                if (args.NewItems != null)
                {
                    OnV2MAddNodes(args.NewStartingIndex, args.NewItems);
                }
                break;
            case NotifyCollectionChangedAction.Remove:
                if (args.OldItems != null)
                {
                    OnV2MRemoveNodes(args.OldStartingIndex, args.OldItems);
                }
                break;
            case NotifyCollectionChangedAction.Replace:
                if (args.OldItems != null && args.NewItems != null && args.OldItems.Count == args.NewItems.Count)
                {
                    OnV2MReplaceNodes(args.OldStartingIndex, args.OldItems, args.NewItems);
                }
                break;
            case NotifyCollectionChangedAction.Reset:
                ClearChildren();
                break;
        }
    }

    protected void AdjustContentNode(int index, bool useTween)
    {
        var node = ContentNodes[index];
        var position = CalculateContentNodePosition(index, true);
        var rotation = CalculateContentNodeRotation(index);
        if (useTween)
        {
            float tweenTime;
            if (node.TweenControl.IsRunning("transform"))
            {
                var controlledTween = node.TweenControl.GetControlledTween("transform");
                tweenTime = controlledTween.Time - (float)controlledTween.Tween.Value.GetTotalElapsedTime();
            }
            else
            {
                tweenTime = Configuration.ContentContainerAdjustTweenTime;
            }
            node.TweenTransform(position, rotation, tweenTime);
        }
        else
        {
            node.Position = position;
            node.Rotation = rotation;
        }
    }
}