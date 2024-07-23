using System.Collections.Generic;
using Godot;
using XCardGame.Scripts.Common;
using XCardGame.Scripts.Common.Constants;

namespace XCardGame.Scripts.Ui;

public partial class SplitCardContainer: Node2D
{
    public class SetupArgs
    {
        public List<CardContainer.SetupArgs> CardContainersSetupArgs;
        public int Separation;
        public Enums.Direction2D8Ways PivotDirection;
    } 
    
    public GameMgr GameMgr;
    
    public PackedScene CardContainerPrefab;
    
    public List<CardContainer> CardContainers;

    public Enums.Direction2D8Ways PivotDirection;
    public int Separation;


    public override void _Ready()
    {
        GameMgr = GetNode<GameMgr>("/root/GameMgr");
        CardContainerPrefab = ResourceCache.Instance.Load<PackedScene>("res://Scenes/CardContainer.tscn");
        CardContainers = new List<CardContainer>();
    }

    public void Setup(object o)
    {
        var args = (SetupArgs)o;
        var cardContainersSetupArgs = args.CardContainersSetupArgs;
        Separation = args.Separation;
        PivotDirection = args.PivotDirection;
        foreach (var cardContainersSetupArg in cardContainersSetupArgs)
        {
            var cardContainer = (CardContainer)Utils.InstantiatePrefab(CardContainerPrefab, this);
            cardContainer.Setup(cardContainersSetupArg);
            CardContainers.Add(cardContainer);
            cardContainer.OnAdjustLayout += AdjustContainers;
        }
    }

    public Vector2 Size()
    {
        var size = Vector2.Zero;
        foreach (var cardContainer in CardContainers)
        {
            size += cardContainer.Size();
        }

        if (CardContainers.Count > 0)
        {
            size += new Vector2((CardContainers.Count - 1) * Separation, 0);
        }

        return size;
    }
    
    protected void AdjustContainers()
    {
        var pivotOffset = GetPivotOffset();
        for (int i = 0; i < CardContainers.Count; i++)
        {
            var childContainerPivotOffset = GetPivotOffsetOfContainer(i);
            CardContainers[i].Position = childContainerPivotOffset - pivotOffset;
        }
    }

    protected Vector2 GetPivotOffset()
    {
        Vector2 pivotOffsetFromBottomLeft = Vector2.Zero;
        var size = Size();
        switch (PivotDirection)
        {
            case Enums.Direction2D8Ways.Neutral:
                pivotOffsetFromBottomLeft = new Vector2(size.X / 2, size.Y / 2);
                break;
            case Enums.Direction2D8Ways.Down:
                pivotOffsetFromBottomLeft = new Vector2(size.X / 2, 0);
                break;
            case Enums.Direction2D8Ways.Left:
                pivotOffsetFromBottomLeft = new Vector2(0, size.Y / 2);
                break;
            case Enums.Direction2D8Ways.Up:
                pivotOffsetFromBottomLeft = new Vector2(size.X / 2, size.Y);
                break;
            case Enums.Direction2D8Ways.Right:
                pivotOffsetFromBottomLeft = new Vector2(size.X, size.Y / 2);
                break;
            case Enums.Direction2D8Ways.DownLeft:
                pivotOffsetFromBottomLeft = new Vector2(0, 0);
                break;
            case Enums.Direction2D8Ways.UpLeft:
                pivotOffsetFromBottomLeft = new Vector2(0, size.Y);
                break;
            case Enums.Direction2D8Ways.UpRight:
                pivotOffsetFromBottomLeft = new Vector2(size.X, size.Y);
                break;
            case Enums.Direction2D8Ways.DownRight:
                pivotOffsetFromBottomLeft = new Vector2(size.X, 0);
                break;
        }
        return pivotOffsetFromBottomLeft;
    }

    protected Vector2 GetPivotOffsetOfContainer(int index)
    {
        Vector2 childContainerOffsetFromBottomLeft = Vector2.Zero;
        for (int i = 0; i < index; i++)
        {
            var size = CardContainers[i].Size();
            childContainerOffsetFromBottomLeft += new Vector2(size.X, 0);
        }
        childContainerOffsetFromBottomLeft += new Vector2(index * Separation, 0);
        var innerPivotOffset = CardContainers[index].GetPivotOffset();
        var childContainerPivotOffset = childContainerOffsetFromBottomLeft + innerPivotOffset;
        return childContainerPivotOffset;
    }
}