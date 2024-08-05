using System.Collections.Generic;

namespace XCardGame.Ui;

public interface IContent
{
    public HashSet<BaseContentNode> Nodes { get; }
    public TContentNode Node<TContentNode>(bool strict = true) where TContentNode : BaseContentNode;
}