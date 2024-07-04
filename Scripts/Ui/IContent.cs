using System.Collections.Generic;

namespace XCardGame.Scripts.Ui;

public interface IContent<TContent> where TContent: IContent<TContent>
{
    public HashSet<BaseContentNode<TContent>> Nodes { get; }
    public TContentNode Node<TContentNode>() where TContentNode : BaseContentNode<TContentNode, TContent>;
}