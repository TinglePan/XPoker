using Godot;
using XCardGame.Common;

namespace XCardGame.Ui;

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
        buff.Nodes.Add(this);
        Icon.Texture = ResourceCache.Instance.Load<Texture2D>(buff.IconPath);
    }

    protected override void OnContentDetached(IContent content)
    {
        base.OnContentDetached(content);
        var buff = (BaseBuff)content;
        buff.Nodes.Remove(this);
        Icon.Texture = null;
    }
}