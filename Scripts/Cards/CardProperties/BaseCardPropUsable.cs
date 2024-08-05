using System.Collections.Generic;
using System.Threading.Tasks;
using XCardGame.Common;
using XCardGame.TimingInterfaces;
using XCardGame.Ui;

namespace XCardGame.CardProperties;

public abstract class BaseCardPropUsable: BaseCardProp, ICardReset, ICardUse
{
    public ObservableProperty<int> Cost;
    
    protected GameMgr GameMgr => Card.GameMgr;
    protected Battle Battle => Card.Battle;
    
    public BaseCardPropUsable(BaseCard card) : base(card)
    {
        Cost = new ObservableProperty<int>(nameof(Cost), this, Card.Def.Cost);
    }

    public void OnReset()
    {
        Cost.Value = Card.Def.Cost;
    }
    
    public virtual bool CanUse()
    {
        if (Battle.Player.Energy.Value < Card.Def.Cost) return false;
        return true;
    }

    public virtual async void Use()
    {
        await CardNode.AnimateSelect(true, Configuration.SelectTweenTime);
    }
    
    public virtual void QueryConfirm()
    {
        GameMgr.InputMgr.SwitchToInputHandler(GetInputHandler());
    }

    public virtual Task Effect(List<CardNode> targets)
    {
        Battle.Player.Energy.Value -= Card.Def.Cost;
        return Task.CompletedTask;
    }
    
    protected virtual BaseInputHandler GetInputHandler()
    {
        return new BaseUsableCardInputHandler(GameMgr, CardNode);
    }
}