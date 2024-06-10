using System.Collections;
using System.Collections.Generic;
using Godot;
using XCardGame.Scripts.Common;

namespace XCardGame.Scripts.Nodes;

public abstract partial class ContentContainerWithBorder<TContentNode, TContent>: ContentContainer<TContentNode, TContent>
    where TContentNode: BaseContentNode<TContentNode, TContent>
    where TContent: IContent<TContent>
{
    [Export] public StyleBox StyleBox;
    
    public int ExpectedContentNodeCount;

    public override void _Ready()
    {
        base._Ready();
        Border = GetNode<NinePatchRect>("Border");
    }

    public override void Setup(Dictionary<string, object> args)
    {
        base.Setup(args);
        ExpectedContentNodeCount = (int)args["expectedContentNodeCount"];
        AdjustBorder();
    }

    protected override void OnV2MAddNodes(int startingIndex, IList nodes)
    {
        base.OnV2MAddNodes(startingIndex, nodes);
        AdjustBorder();
    }

    protected override void OnV2MClearNodes()
    {
        base.OnV2MClearNodes();
        AdjustBorder();
    }

    protected override void OnV2MRemoveNodes(int startingIndex, IList nodes)
    {
        base.OnV2MRemoveNodes(startingIndex, nodes);
        AdjustBorder();
    }

    protected override int ActualNodeCount()
    {
        return Mathf.Max(ExpectedContentNodeCount, ContentNodes.Count);
    }

    protected virtual void AdjustBorder()
    {
        if (ActualNodeCount() == 0)
        {
            GD.Print($"hide {Border}");
            Border.Hide();
        }
        else
        {
            GD.Print($"show {Border}");
            var size = Size();
            Border.Size = new Vector2(size.X + StyleBox.ContentMarginLeft + StyleBox.ContentMarginRight, size.Y + StyleBox.ContentMarginBottom + StyleBox.ContentMarginTop);
            // var pivotOffsetFromTopLeft = pivotOffsetFromBottomLeft + new Vector2(0, size.Y);
            Border.Position = -(GetPivotOffset() + new Vector2(StyleBox.ContentMarginLeft, StyleBox.ContentMarginTop));
            Border.Show();
        }
    }
}
    