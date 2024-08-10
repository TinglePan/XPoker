using System.Collections.Generic;
using System.Threading.Tasks;
using XCardGame.CardProperties;
using XCardGame.Common;
using XCardGame.Common.HelperBoilerPlates;
using XCardGame.Ui;

namespace XCardGame;

public class BaseUsableCardInputHandler: BaseInputHandler
{
    public CardInputHandlerHelper Helper;
        
    public BaseUsableCardInputHandler(GameMgr gameMgr, CardNode node) : base(gameMgr)
    {
        Helper = new CardInputHandlerHelper(this, node);
    }
    
    public override async Task AwaitAndDisableInput(Task task)
    {
        await Helper.AwaitAndDisableInput(task);
    }
        
    public override async Task OnEnter()
    {
        var tasks = new List<Task>();
        Helper.OnEnter(Configuration.StandardUsableCardOptionsMenuName);
        Helper.ReBindHandler("Confirm", Confirm);
        tasks.Add(base.OnEnter());
        tasks.Add(Helper.OriginateCardNode.AnimateSelect(true, Configuration.SelectTweenTime));
        await Task.WhenAll(tasks);
    }
        
    public override async Task OnExit()
    {
        await Helper.OriginateCardNode.AnimateSelect(false, Configuration.SelectTweenTime);
        Helper.OnExit();
        await base.OnExit();
    }
    
    protected async void Confirm()
    {
        if (ReceiveInput)
        {
            await AwaitAndDisableInput(Helper.OriginateCard.GetProp<BaseCardPropUsable>().Effect(null));
            Exit();
        }
    }
}