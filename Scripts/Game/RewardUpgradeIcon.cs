using System.Collections.Generic;
using Godot;
using XCardGame.Scripts.Common;
using XCardGame.Scripts.Common.DataBinding;
using XCardGame.Scripts.Ui;

namespace XCardGame.Scripts.Game;

public class RewardUpgradeIcon: IContentContractGetIcon<Texture2D>
{
    public string IconPath;
    public HashSet<BaseContentNode> Nodes { get; }
    
    public RewardUpgradeIcon(string iconPath)
    {
        Nodes = new HashSet<BaseContentNode>();
        IconPath = iconPath;
    }
    
    
    public TContentNode Node<TContentNode>() where TContentNode : BaseContentNode
    {
        foreach (var node in Nodes)
        {
            if (node is TContentNode contentNode)
            {
                return contentNode;
            }
        }
        return null;
    }

    public Texture2D GetIcon()
    {
        return ResourceCache.Instance.Load<Texture2D>(IconPath);
    }
}