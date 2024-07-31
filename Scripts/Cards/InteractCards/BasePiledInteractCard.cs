using XCardGame.Ui;

namespace XCardGame;

public class BasePiledInteractCard: BaseInteractCard
{
    public BasePiledInteractCard(PiledInteractCardDef def) : base(def)
    {
    }

    public override void Setup(object o)
    {
        base.Setup(o);
        var args = o as SetupArgs;
        
    }
}