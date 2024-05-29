﻿using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Threading.Tasks;
using Godot;
using XCardGame.Scripts.Common.Constants;

namespace XCardGame.Scripts.Nodes;

public abstract partial class ContentContainer<TContentNode, TContent>: ManagedNode2D, ISetup 
    where TContentNode: BaseContentNode<TContentNode, TContent>
    where TContent: IContent<TContentNode, TContent>
{
    public bool HasSetup { get; set; }

    public ObservableCollection<TContent> Contents;
    public List<TContentNode> ContentNodes;
    
    public Vector2 ContentNodeSize;
    public int Separation;
    
    public override void _Ready()
    {
        base._Ready();
        ClearChildren();
        Contents = new ObservableCollection<TContent>();
        Contents.CollectionChanged += OnContentsChanged;
        ContentNodes = new List<TContentNode>();
        HasSetup = false;
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
    
    public abstract void OnM2VAddContents(int startingIndex, IList contents);

    public void OnM2VRemoveContents(int startingIndex, IList contents)
    {
        EnsureSetup();
        var index = startingIndex;
        if (index >= ContentNodes.Count) return;
        List<TContentNode> removedNodes = new List<TContentNode>();
        foreach (var content in contents)
        {
            if (content is TContent typedContent && ContentNodes[index].Content.Value.Equals(typedContent))
            {
                removedNodes.Add(ContentNodes[index]);
                ContentNodes.RemoveAt(index);
                index++;
            }
        }
        foreach (var removedNode in removedNodes)
        {
            removedNode.QueueFree();
        }
    }

    public void OnM2VReplaceContents(int oldContentsStartingIndex, IList oldContents, IList newContents)
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

    public void OnV2MAddNode(int index, TContentNode node)
    {
        EnsureSetup();
        if (index < Contents.Count && Contents[index].Equals(node.Content.Value)) return;
        Contents.Insert(index, node.Content.Value);
    }

    public void OnV2MRemoveNode(int index, TContentNode node)
    {
        EnsureSetup();
        if (index >= Contents.Count || !Contents[index].Equals(node.Content.Value)) return;
        Contents.RemoveAt(index);
    }

    public void OnV2MClearNodes()
    {
        EnsureSetup();
        if (Contents.Count == 0) return;
        Contents.Clear();
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
    
    public virtual void AddContentNode(int index, TContentNode node, float tweenTime = 0f)
    {
        EnsureSetup();
        if (index < ContentNodes.Count && ContentNodes[index] == node) return;
        ContentNodes.Insert(index, node);
        node.Reparent(this);
        MoveChild(node, index);
        node.Container = this;
        GD.Print($"{node.Content.Value}({node}) container added {this}");
        for (int i = 0; i < ContentNodes.Count; i++)
        {
            AdjustContentNode(i, tweenTime);
        }
        OnV2MAddNode(index, node);
    }
    
    public void AppendContentNode(TContentNode node, float tweenTime = 0f)
    {
        EnsureSetup();
        AddContentNode(ContentNodes.Count, node, tweenTime);
    }

    public TContentNode ReplaceContentNode(int index, TContentNode node, float tweenTime = 0f)
    {
        EnsureSetup();
        if (ContentNodes[index] != node)
        {
            var replacedNode = RemoveContentNode(index);
            AddContentNode(index, node, tweenTime);
            return replacedNode;
        }
        return null;
    }
    
    public TContentNode RemoveContentNode(int index, bool free = false)
    {
        EnsureSetup();
        var node = ContentNodes[index];
        ContentNodes.RemoveAt(index);
        OnV2MRemoveNode(index, node);
        node.Container = null;
        GD.Print($"{node.Content.Value}({node}) container removed");
        if (free)
        {
            node.QueueFree();
        }
        return node;
    }
    
    public void ClearContents()
    {
        EnsureSetup();
        ContentNodes.Clear();
        OnV2MClearNodes();
        // ClearChildren();
    }
    
    public void ClearChildren()
    {
        foreach (var child in GetChildren())
        {
            child.QueueFree();
        }
    }

    protected void OnContentsChanged(object sender, NotifyCollectionChangedEventArgs args)
    {
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

    protected void AdjustContentNode(int index, float tweenTime = 0f)
    {
        var node = ContentNodes[index];
        var position = CalculateContentNodePosition(index, ContentNodes.Count, true);
        var rotation = CalculateContentNodeRotation(index, ContentNodes.Count);
        if (tweenTime != 0)
        {
            var tween = node.GetTree().CreateTween();
            tween.TweenProperty(node, "position", position, tweenTime).SetTrans(Tween.TransitionType.Linear).SetEase(Tween.EaseType.Out);
            tween.Parallel().TweenProperty(node, "rotation", rotation, tweenTime).SetTrans(Tween.TransitionType.Linear).SetEase(Tween.EaseType.Out);
        }
        else
        {
            node.Position = position;
            node.Rotation = rotation;
        }
    }
}