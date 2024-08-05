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
    
    public TContentNode Node<TContentNode>(bool strict = true) where TContentNode : BaseContentNode
    {
        foreach (var node in Nodes)
        {
            if (strict)
            {
                if (node is TContentNode contentNode)
                {
                    return contentNode;
                }
            }
            else
            {
                if (node.GetType().IsAssignableTo(typeof(TContentNode)))
                {
                    return (TContentNode)node;
                }
            }
        }
        return null;
    }

    public Texture2D GetIcon()
    {
        return ResourceCache.Instance.Load<Texture2D>(IconPath);
    }
}