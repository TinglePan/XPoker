using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Threading.Tasks;
using Godot;
using XCardGame.Scripts.Common.Constants;

namespace XCardGame.Scripts.Nodes;

public abstract partial class ContentContainer<TContentNode, TContent>: Node2D, ISetup
    where TContentNode: BaseContentNode<TContentNode, TContent>
    where TContent: IContent<TContent>
{
    public bool HasSetup { get; set; }

    public ObservableCollection<TContent> Contents;
    public ObservableCollection<TContentNode> ContentNodes;
    
    public Vector2 ContentNodeSize;
    public int Separation;

    public GameMgr GameMgr;
    
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
        HasSetup = false;
        SuppressNotifications = false;
    }

    public virtual void Setup(Dictionary<string, object> args)
    {
        HasSetup = true;
        ContentNodeSize = (Vector2)args["contentNodeSize"];
        Separation = (int)args["separation"];
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

    protected void OnV2MRemoveNodes(int startingIndex, IList nodes)
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

    protected void OnV2MReplaceNodes(int oldNodesStartingIndex, IList oldNodes, IList newNodes)
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

    public virtual Vector2 CalculateContentNodePosition(int index, int n, bool isInsideContainer = false)
    {
        var offset = CalculateContentNodeOffset(index, n);
        return isInsideContainer ? offset : Position + offset;
    }

    public virtual Vector2 CalculateContentNodeOffset(int index, int n)
    {
        var size = n * ContentNodeSize.X + (n - 1) * Separation;
        var offsetInContainer = (ContentNodeSize.X + Separation) * index + ContentNodeSize.X / 2;
        var offset = offsetInContainer - size / 2;
        return new Vector2(offset, 0);
    }
    
    public virtual float CalculateContentNodeRotation(int index, int n)
    {
        return 0f;
    }
    
    // public virtual void AddContentNode(int index, TContentNode node, float tweenTime = 0f)
    // {
    //     EnsureSetup();
    //     ContentNodes.Insert(index, node);
    //     node.Reparent(this);
    //     MoveChild(node, index);
    //     node.Container = this;
    //     GD.Print($"{node.Content.Value}({node}) container added {this}");
    //     for (int i = 0; i < ContentNodes.Count; i++)
    //     {
    //         AdjustContentNode(i, tweenTime);
    //     }
    //     OnV2MAddNode(index, node);
    // }
    
    // public void AppendContentNode(TContentNode node, float tweenTime = 0f)
    // {
    //     EnsureSetup();
    //     AddContentNode(ContentNodes.Count, node, tweenTime);
    // }

    // public TContentNode SwapContentNode(int index, TContentNode node, float tweenTime = 0f)
    // {
    //     EnsureSetup();
    //     if (ContentNodes[index] != node)
    //     {
    //         (node.Content.Value, ContentNodes[index].Content.Value) = (ContentNodes[index].Content.Value, node.Content.Value);
    //         return node;
    //     }
    //     return null;
    // }
    
    // public TContentNode RemoveContentNode(int index, bool free = false)
    // {
    //     EnsureSetup();
    //     var node = ContentNodes[index];
    //     ContentNodes.RemoveAt(index);
    //     OnV2MRemoveNode(index, node);
    //     node.Container = null;
    //     GD.Print($"{node.Content.Value}({node}) container removed");
    //     if (free)
    //     {
    //         node.QueueFree();
    //     }
    //     return node;
    // }
    
    // public void ClearContents()
    // {
    //     EnsureSetup();
    //     ContentNodes.Clear();
    //     OnV2MClearNodes();
    //     ClearChildren();
    // }
    
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
        var position = CalculateContentNodePosition(index, ContentNodes.Count, true);
        var rotation = CalculateContentNodeRotation(index, ContentNodes.Count);
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