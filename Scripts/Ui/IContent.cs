using System;
using System.Collections.Generic;

namespace XCardGame.Scripts.Nodes;

public interface IContent<TContent> where TContent: IContent<TContent>
{
    public HashSet<BaseContentNode<TContent>> Nodes { get; }
    public TContentNode Node<TContentNode>() where TContentNode : BaseContentNode<TContentNode, TContent>;
}