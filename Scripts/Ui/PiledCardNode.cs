using System.Collections.Specialized;
using System.Linq;
using Godot;
using XCardGame.Common;

namespace XCardGame.Ui;

public partial class PiledCardNode: CardNode
{
    public AttachedCardPile CardPile;

    public override void _Ready()
    {
        base._Ready();
        CardPile = GetNode<AttachedCardPile>("CardPile");
    }
    
    public override void Setup(object o)
    {
        base.Setup(o);
        CardPile.Setup(new AttachedCardPile.SetupArgs
        {
            GameMgr = GameMgr,
            Battle = Battle,
            TopCardNode = this,
        });
    }
}