using XCardGame.Ui;

namespace XCardGame.Common;

public static class InterfaceContentBoilerPlates
{
    public static TContentNode Node<TContentNode>(IContent content, bool strict = true) where TContentNode : BaseContentNode
    {
        foreach (var node in content.Nodes)
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
}