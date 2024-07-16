using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using Godot;
using XCardGame.Scripts.Common.Constants;
using Vector2 = Godot.Vector2;
using Vector4 = Godot.Vector4;

namespace XCardGame.Scripts.Ui;

public abstract partial class BaseContentContainer: Node2D
{
    public class SetupArgs
    {
        public Vector2 ContentNodeSize;
        public Vector2 Separation;
        public Enums.Direction2D8Ways PivotDirection; 
        public int NodesPerRow;
        
        public bool HasBorder;
        public Vector4 Margins;
        public int MinContentNodeCountForBorder;
        
        public bool HasName;
        public string ContainerName;

        public SetupArgs()
        {
            
        }
        
        public SetupArgs(SetupArgs other)
        {
            ContentNodeSize = other.ContentNodeSize;
            Separation = other.Separation;
            PivotDirection = other.PivotDirection;
            NodesPerRow = other.NodesPerRow;
            HasBorder = other.HasBorder;
            Margins = other.Margins;
            MinContentNodeCountForBorder = other.MinContentNodeCountForBorder;
            HasName = other.HasName;
            ContainerName = other.ContainerName;
        }
    }
    
    public GameMgr GameMgr;
    public NinePatchRect Border;
    public Label ContainerNameLabel;
    
    public Action<BaseContentNode> OnAddContentNode;
    public Action<BaseContentNode> OnRemoveContentNode;
    public Action OnAdjustLayout;
    
    public Vector2 ContentNodeSize;
    public Vector2 Separation;
    public Enums.Direction2D8Ways PivotDirection;
    public int NodesPerRow;

    public bool HasBorder;
    public int MinContentNodeCountForBorder;

    public bool HasName;
    public string ContainerName;

    public Vector4 Margins;
    
    public ObservableCollection<IContent> Contents;
    public ObservableCollection<BaseContentNode> ContentNodes;
    protected bool SuppressNotifications;
    
    public override void _Ready()
    {
        base._Ready();
        ClearChildren();
        GameMgr = GetNode<GameMgr>("/root/GameMgr");

        Contents = new ObservableCollection<IContent>();
        Contents.CollectionChanged += OnContentsChanged;
        ContentNodes = new ObservableCollection<BaseContentNode>();
        ContentNodes.CollectionChanged += OnContentNodesChanged;
        HasBorder = false;
        HasName = false;
        SuppressNotifications = false;
    }

    public virtual void Setup(SetupArgs args)
    {
        ContentNodeSize = args.ContentNodeSize;
        Separation = args.Separation;
        PivotDirection = args.PivotDirection;
        NodesPerRow = args.NodesPerRow;
        Margins = args.Margins;

        HasBorder = args.HasBorder;
        if (HasBorder)
        {
            Border = GetNode<NinePatchRect>("Border");
            MinContentNodeCountForBorder = args.MinContentNodeCountForBorder;
        }

        HasName = args.HasName;
        if (HasName)
        {
            ContainerNameLabel = GetNode<Label>("Name");
            ContainerName = args.ContainerName;
            ContainerNameLabel.Text = ContainerName;
        }
        AdjustLayout();
    }
    
    public Vector2 Size()
    {
        var effectiveNodeCount = EffectiveNodeCount();
        if (effectiveNodeCount == 0) return new Vector2(0, ContentNodeSize.Y);
        var effectiveNodesPerRow = NodesPerRow == 0 ? effectiveNodeCount : Mathf.Min(effectiveNodeCount, NodesPerRow);
        var effectiveRowCount = effectiveNodesPerRow == 0 ? 0 : (effectiveNodeCount - 1) / effectiveNodesPerRow + 1;
        return new Vector2(ContentNodeSize.X * effectiveNodesPerRow + Separation.X * (effectiveNodesPerRow - 1),
            ContentNodeSize.Y * effectiveRowCount + Separation.Y * (effectiveRowCount - 1));
    }
    
    public Vector2 CalculateContentNodePosition(int index, bool globalPosition = false)
    {
        var offset = CalculateContentNodeOffset(index);
        return globalPosition ? offset : GlobalPosition + offset;
    }

    public Vector2 CalculateContentNodeOffset(int index)
    {
        var colIndex = NodesPerRow != 0 ? index % NodesPerRow : index;
        var rowIndex = NodesPerRow != 0 ? index / NodesPerRow : 0;
        Vector2 pivotOffsetFromBottomLeft = GetPivotOffset();
        var nodeOffsetFromBottomLeft = new Vector2(colIndex * (ContentNodeSize.X + Separation.X) + ContentNodeSize.X / 2,
            rowIndex * (ContentNodeSize.Y + Separation.Y) + ContentNodeSize.Y / 2);
        var offset = nodeOffsetFromBottomLeft - pivotOffsetFromBottomLeft;
        return offset;
    }
    
    public virtual float CalculateContentNodeRotation(int index)
    {
        return 0f;
    }
    
    public Vector2 GetPivotOffset()
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

    protected int EffectiveNodeCount()
    {
        if (HasBorder && MinContentNodeCountForBorder > 0)
        {
            return Mathf.Max(MinContentNodeCountForBorder, ContentNodes.Count);
        }
        return ContentNodes.Count;
    }
    
    protected abstract void OnM2VAddContents(int startingIndex, IList contents);

    protected void OnM2VRemoveContents(int startingIndex, IList contents)
    {
        SuppressNotifications = true;
        for (int i = 0; i < contents.Count; i++)
        {
            var node = ContentNodes[startingIndex + i];
            OnRemoveContentNode?.Invoke(node);
            ContentNodes.RemoveAt(startingIndex + i);
        }
        SuppressNotifications = false;
    }

    protected void OnM2VReplaceContents(int oldContentsStartingIndex, IList oldContents, IList newContents)
    {
        for (int i = 0; i < oldContents.Count; i++)
        {
            if (oldContents[i] is IContent oldContent && newContents[i] is IContent newContent)
            {
                var oldContentNode = ContentNodes[oldContentsStartingIndex + i];
                oldContentNode.Content.Value = newContent;
            }
        }
    }

    protected virtual void OnV2MAddNodes(int startingIndex, IList nodes)
    {
        SuppressNotifications = true;
        var index = startingIndex;
        foreach (var node in nodes)
        {
            if (node is BaseContentNode contentNode)
            {
                Contents.Insert(index, contentNode.Content.Value);
                if (contentNode.GetParent() != null)
                {
                    contentNode.Reparent(this);
                }
                else
                {
                    AddChild(contentNode);
                }
                MoveChild(contentNode, index);
                contentNode.PreviousContainer = contentNode.CurrentContainer.Value;
                contentNode.CurrentContainer.Value = this;
                OnAddContentNode?.Invoke(contentNode);
                // GD.Print($"{contentNode.Content.Value}({contentNode}) container added {this}");
            }
            index++;
        }
        AdjustLayout();
        SuppressNotifications = false;
    }

    protected virtual void OnV2MRemoveNodes(int startingIndex, IList nodes)
    {
        SuppressNotifications = true;
        var index = startingIndex;
        foreach (var node in nodes)
        {
            if (node is BaseContentNode contentNode)
            {
                OnRemoveContentNode?.Invoke(contentNode);
                Contents.RemoveAt(index);
                // if (contentNode.GetParent() == this)
                // {
                //     RemoveChild(contentNode);
                // }
                if (contentNode.CurrentContainer.Value == this)
                {
                    contentNode.PreviousContainer = contentNode.CurrentContainer.Value;
                    contentNode.CurrentContainer.Value = null;
                }
                // GD.Print($"{contentNode.Content.Value}({contentNode}) container removed {this}");
            }
            index++;
        }
        AdjustLayout();
        
        SuppressNotifications = false;
    }

    protected virtual void OnV2MReplaceNodes(int oldNodesStartingIndex, IList oldNodes, IList newNodes)
    {
        SuppressNotifications = true;
        for (int i = 0; i < oldNodes.Count; i++)
        {
            if (oldNodes[i] is BaseContentNode oldNode && newNodes[i] is BaseContentNode newNode)
            {
                Contents[oldNodesStartingIndex + i] = newNode.Content.Value;
                RemoveChild(oldNode);
                oldNode.PreviousContainer = oldNode.CurrentContainer.Value;
                oldNode.CurrentContainer.Value = null;
                newNode.Reparent(newNode);
                MoveChild(newNode, oldNodesStartingIndex + i);
                newNode.PreviousContainer = newNode.CurrentContainer.Value;
                newNode.CurrentContainer.Value = this;
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
        SuppressNotifications = true;
        ClearChildren();
        Contents.Clear();
        SuppressNotifications = false;
        AdjustLayout();
    }
    
    protected void ClearChildren()
    {
        foreach (var child in GetChildren())
        {
            if (child is BaseContentNode)
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

    protected void AdjustLayout()
    {
        for (int i = 0; i < ContentNodes.Count; i++)
        {
            AdjustContentNode(i, true);
        }
        AdjustBorder();
        AdjustNameLabel();
        OnAdjustLayout?.Invoke();
    }

    protected async void AdjustContentNode(int index, bool useTween)
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
            await node.AnimateTransform(position, rotation, tweenTime);
        }
        else
        {
            node.Position = position;
            node.Rotation = rotation;
        }
    }

    protected virtual void AdjustBorder()
    {
        if (!HasBorder || EffectiveNodeCount() == 0)
        {
            // GD.Print($"hide {Border}");
            if (Border != null)
            {
                Border.Hide();
            }
        }
        else
        {
            // GD.Print($"show {Border}");
            var size = Size();
            Border.Size = new Vector2(size.X + Margins.X + Margins.Z, size.Y + Margins.W + Margins.Y);
            // var pivotOffsetFromTopLeft = pivotOffsetFromBottomLeft + new Vector2(0, size.Y);
            Border.Position = -(GetPivotOffset() + new Vector2(Margins.X, Margins.Y));
            Border.Show();
        }
    }

    protected virtual void AdjustNameLabel()
    {
        if (!HasName)
        {
            if (ContainerNameLabel != null)
            {
                ContainerNameLabel.Hide();
            }
        }
        else
        {
            var pivotOffset = GetPivotOffset();
            ContainerNameLabel.Position = - new Vector2(ContainerNameLabel.Size.X / 2,  pivotOffset.Y + Margins.Y + ContainerNameLabel.Size.Y);
            ContainerNameLabel.Show();
        }
    }
}