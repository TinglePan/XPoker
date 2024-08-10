using XCardGame.Ui;

namespace XCardGame.Common;

public static class InterfaceContentBoilerPlates
{
    public static TContentNode Node<TContentNode>(IContent content, bool strict = false) where TContentNode : BaseContentNode
    {
        foreach (var node in content.Nodes)
        {
            var type = node.GetType();
            if (strict)
            {
                if (type == typeof(TContentNode))
                {
                    return (TContentNode)node;
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