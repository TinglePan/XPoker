using Godot;
using XCardGame.Scripts.Common;
using XCardGame.Scripts.Common.Constants;
using XCardGame.Scripts.Defs.Def.Card;
using XCardGame.Scripts.Game;
using XCardGame.Scripts.Ui;

namespace XCardGame.Scripts.Cards.InteractCards;

public class BaseInteractCard: BaseCard, IInteractCard
{
    protected bool AlreadyFunctioning;
    
    public BaseInteractCard(BaseCardDef def): base(def)
    {
        AlreadyFunctioning = false;
    }

    public virtual bool CanInteract(CardNode node)
    {
        if (!IsFunctioning()) return false;
        if (node.Container.Value is CardContainer { AllowInteract: false }) return false;
        return true;
    }

    public virtual void Interact(CardNode node)
    {
        
    }
    
    public override void OnStart(Battle battle)
    {
        if (IsFunctioning() && !AlreadyFunctioning)
        {
            AlreadyFunctioning = true;
        }
    }

    public override void OnStop(Battle battle)
    {
        if (AlreadyFunctioning)
        {
            AlreadyFunctioning = false;
        }
    }

    public override async void ChangeRank(int delta)
    {
        var cardNode = Node<CardNode>();
        var currentRankValue = Utils.GetCardRankValue(Rank.Value);
        if (currentRankValue == 1 && delta < 0)
        {
            await GameMgr.AwaitAndDisableProceed(Battle.Dealer.AnimateDiscard(cardNode));
        }
        else
        {
            var resultRankValue = Utils.GetCardRankValue(Rank.Value) + delta;
            resultRankValue = Mathf.Clamp(resultRankValue, 1,
                Utils.GetCardRankValue(Enums.CardRank.King));
            var resultRank = Utils.GetCardRank(resultRankValue);
            Rank.Value = resultRank;
        }
    }
}