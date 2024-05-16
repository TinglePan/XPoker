using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using Godot;
using XCardGame.Scripts.Buffs;

namespace XCardGame.Scripts.Ui;

public partial class BuffContainer: Container, IManagedUi, ISetup
{
    [Export] public string Identifier { get; set; }
    [Export]
    public PackedScene BuffPrefab;
    public GameMgr GameMgr { get; private set; }
    public UiMgr UiMgr { get; private set; }
    
    public ObservableCollection<BaseBuff> Buffs;
    
    public override void _Ready()
    {
        ClearChildren();
        GameMgr = GetNode<GameMgr>("/root/GameMgr");
        UiMgr = GetNode<UiMgr>("/root/UiMgr");
        UiMgr.Register(this);
    }
    
    public override void _ExitTree()
    {
        base._ExitTree();
        if (Buffs != null)
        {
            Buffs.CollectionChanged -= OnBuffsChanged;
        }
    }
    
    public void Setup(Dictionary<string, object> args)
    {
        if (args["buffs"] is ObservableCollection<BaseBuff> buffs && buffs != Buffs)
        {
            Buffs = buffs;
            Buffs.CollectionChanged += OnBuffsChanged;
        }
    }
    
    public void ClearChildren()
    {
        foreach (var child in GetChildren())
        {
            child.QueueFree();
        }
    }

    protected void OnBuffsChanged(object sender, NotifyCollectionChangedEventArgs args)
    {
        
        switch (args.Action)
        {
            case NotifyCollectionChangedAction.Add:
                if (args.NewItems != null)
                    foreach (var buff in args.NewItems)
                    {
                        var buffNode = BuffPrefab.Instantiate<BuffNode>();
                        AddChild(buffNode);
                        buffNode.Setup(new Dictionary<string, object>()
                        {
                            { "buff", buff },
                            { "container", this }
                        });
                    }
                break;
            case NotifyCollectionChangedAction.Remove:
                if (args.OldItems != null && args.OldItems[0] is BaseBuff removedBuff)
                {
                    var removedCardNode = GetChild<BuffNode>(args.OldStartingIndex);
                    removedCardNode.QueueFree();
                }
                break;
            case NotifyCollectionChangedAction.Replace:
                if (args.OldItems != null && args.OldItems[0] is BaseBuff replacedBuff && args.NewItems != null)
                {
                    var replacedBuffNode = GetChild<BuffNode>(args.OldStartingIndex);
                    replacedBuffNode.Buff.Value = args.NewItems[0] as BaseBuff;
                }
                break;
            case NotifyCollectionChangedAction.Reset:
                ClearChildren();
                break;
        }
    }
    
    
}