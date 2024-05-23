using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Threading.Tasks;
using Godot;

namespace XCardGame.Scripts.Nodes;

public abstract partial class BaseContentContainer<TContentNode, TContent>: Node3D, ISetup, IManagedUi 
    where TContentNode: BaseContentNode<TContentNode, TContent>
    where TContent: IContent<TContentNode, TContent>
{
    [Export]
    public string Identifier { get; set; }
    public GameMgr GameMgr { get; private set; }
    public UiMgr UiMgr { get; private set;  }
    
    public bool HasSetup { get; set; }

    public ObservableCollection<TContent> Contents;
    public List<TContentNode> ContentNodes;
    
    public Vector2 ContentMinimumSize;
    
    public override void _Ready()
    {
        ClearChildren();
        GameMgr = GetNode<GameMgr>("/root/GameMgr");
        UiMgr = GetNode<UiMgr>("/root/UiMgr");
        UiMgr.Register(this);
        Contents = new ObservableCollection<TContent>();
        Contents.CollectionChanged += OnContentsChanged;
        ContentNodes = new List<TContentNode>();
        HasSetup = false;
    }

    public virtual void Setup(Dictionary<string, object> args)
    {
        HasSetup = true;
        ContentMinimumSize = args.TryGetValue("contentMinimumSize", out var value) ? (Vector2)value : Vector2.Zero;
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
        List<Control> removedNodes = new List<Control>();
        foreach (var content in contents)
        {
            removedNodes.Add(GetChild<Control>(index));
            index++;
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
                var oldContentNode = GetChild<Control>(oldContentsStartingIndex + i).GetChild<TContentNode>(0);
                oldContentNode.Content.Value = newContent;
            }
        }
    }

    public void OnV2MAddNode(int index, TContentNode node)
    {
        EnsureSetup();
        Contents.Insert(index, node.Content.Value);
    }

    public void OnV2MRemoveNode(int index)
    {
        EnsureSetup();
        Contents.RemoveAt(index);
    }

    public void OnV2MClearNodes()
    {
        EnsureSetup();
        ContentNodes.Clear();
        Contents.Clear();
    }
    
    public virtual Vector3 CalculateContentNodePosition(int index)
    {
        return Transform.Origin;
    }
    
    public virtual float CalculateContentNodeRotationZ(int index)
    {
        return 0f;
    }
    
    public async Task AddContentNode(int index, TContentNode node, float tweenTime = 0f)
    {
        EnsureSetup();
        if (ContentNodes[index] != node)
        {
            node.CustomMinimumSize = ContentMinimumSize;
            if (tweenTime != 0)
            {
                var position = CalculateContentNodePosition(index);
                var rotationZ = CalculateContentNodeRotationZ(index);
                var tween = node.GetTree().CreateTween();
                tween.TweenProperty(node, "position", position, tweenTime);
                tween.Parallel().TweenProperty(node, "rotation:z", rotationZ, tweenTime);
                await ToSignal(tween, Tween.SignalName.Finished);
            }
            ContentNodes.Insert(index, node);
            AddChild(node);
            MoveChild(node, index);
            node.Container = this;
            OnV2MAddNode(index, node);
        }
    }

    public async Task<TContentNode> ReplaceContentNode(int index, TContentNode node, float tweenTime = 0f)
    {
        EnsureSetup();
        if (ContentNodes[index] != node)
        {
            var replacedNode = RemoveContentNode(index);
            await AddContentNode(index, node, tweenTime);
            return replacedNode;
        }
        return null;
    }
    
    public TContentNode RemoveContentNode(int index, bool free = false)
    {
        EnsureSetup();
        var node = ContentNodes[index];
        OnV2MRemoveNode(index);
        node.Container = null;
        if (free)
        {
            node.QueueFree();
        }
        return node;
    }
    
    public void ClearContents()
    {
        EnsureSetup();
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
}