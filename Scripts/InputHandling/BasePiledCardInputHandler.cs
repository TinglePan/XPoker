using System.Threading.Tasks;
using Godot;
using XCardGame.Common;
using XCardGame.Ui;

namespace XCardGame;

public abstract class BasePiledCardInputHandler<TOriginateCard>: BaseInputHandler where TOriginateCard: BaseCard
{
    public PiledCardNode OriginateCardNode;
    public TOriginateCard OriginateCard;
    
    protected BasePiledCardInputHandler(GameMgr gameMgr, PiledCardNode originate) : base(gameMgr)
    {
        OriginateCardNode = originate;
        OriginateCard = (TOriginateCard)originate.Content.Value;
    }

    public override async Task OnEnter()
    {
        await base.OnEnter();
        await OriginateCardNode.AnimateSelect(true, Configuration.SelectTweenTime);
        await OriginateCardNode.ToSignal(OriginateCardNode.AnimationPlayer, "animation_finished");
    }

    public override async Task OnExit()
    {
        await base.OnExit();
        await OriginateCardNode.AnimateSelect(false, Configuration.SelectTweenTime);
    }
}