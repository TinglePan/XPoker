using System.Collections.Specialized;
using System.Linq;
using Godot;
using XCardGame.Common;

namespace XCardGame.Ui;

public class PiledCardNode: CardNode
{
    public CardPile CardPile;

    public override void _Ready()
    {
        base._Ready();
        CardPile = GetNode<CardPile>("CardPile");
    }

    public void OnHoverHandler(BaseContentNode node)
    {
        // NYI: add information to side panel
    }

    public void OnUnHoverHandler(BaseContentNode node)
    {
        // NYI: clear side panel
    }

}