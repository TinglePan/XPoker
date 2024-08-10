using System.Collections.Generic;
using Godot;
using XCardGame.Common;
using XCardGame.Ui;

namespace XCardGame;

public class RewardUpgradeIcon: IContentContractGetIcon<Texture2D>
{
    public string IconPath;
    public HashSet<BaseContentNode> Nodes { get; }
    
    public RewardUpgradeIcon(string iconPath)
    {
        Nodes = new HashSet<BaseContentNode>();
        IconPath = iconPath;
    }
    
    public TContentNode Node<TContentNode>(bool strict = false) where TContentNode : BaseContentNode
    {
        return InterfaceContentBoilerPlates.Node<TContentNode>(this, strict);
    }

    public Texture2D GetIcon()
    {
        return ResourceCache.Instance.Load<Texture2D>(IconPath);
    }
}