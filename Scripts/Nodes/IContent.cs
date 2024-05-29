using System;

namespace XCardGame.Scripts.Nodes;

public interface IContent<TNode, TContent> where TNode: BaseContentNode<TNode, TContent> where TContent: IContent<TNode, TContent>
{
    public TNode Node { get; set; }
}