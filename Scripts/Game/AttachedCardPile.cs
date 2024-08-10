using Godot;
using XCardGame.Common;
using XCardGame.Ui;

namespace XCardGame;

public partial class AttachedCardPile: BaseCardPile
{
    public new class SetupArgs: BaseCardPile.SetupArgs
    {
        public PiledCardNode TopCardNode;
    }

    public override void Setup(object o)
    {
        base.Setup(o);
        var args = (SetupArgs)o;
        TopCardNode = args.TopCardNode;
    }
    
    protected override void Adjust()
    {
        var topCardOffset = GetTopCardOffset(Cards.Count);
        TopCardNode.Position = TopCardNode.InitPosition + topCardOffset;
        Position = -topCardOffset;
    }
    
    protected Vector2 GetTopCardOffset(int count)
    {
        return Configuration.PiledCardOffsetMax * Mathf.Clamp((float)count / Configuration.PileCardCountAtMaxOffset, 0, 1);
    }
    
}