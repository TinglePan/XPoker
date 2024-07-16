using System.Collections.Generic;

namespace XCardGame.Scripts.Ui;

public interface IContent
{
    public HashSet<BaseContentNode> Nodes { get; }
    public TContentNode Node<TContentNode>() where TContentNode : BaseContentNode;
}