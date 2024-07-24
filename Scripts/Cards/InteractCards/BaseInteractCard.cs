using Godot;
using XCardGame.Common;
using XCardGame.Ui;

namespace XCardGame;

public class BaseInteractCard: BaseCard, IInteractCard
{
    public ObservableProperty<int> Cost;
    
    protected bool AlreadyFunctioning;
    
    public BaseInteractCard(InteractCardDef def): base(def)
    {
        AlreadyFunctioning = false;
        Cost = new ObservableProperty<int>(nameof(Cost), this, def.Cost);
    }

    public virtual bool CanInteract(CardNode node)
    {
        if (!IsFunctioning()) return false;
        if (node.CurrentContainer.Value is CardContainer { AllowInteract: false }) return false;
        if (Cost.Value > Battle.Player.Energy.Value) return false;
        return true;
    }

    public virtual void Interact(CardNode node)
    {
        
    }
    
    public override void OnStartEffect(Battle battle)
    {
        if (IsFunctioning() && !AlreadyFunctioning)
        {
            AlreadyFunctioning = true;
        }
    }

    public override void OnStopEffect(Battle battle)
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
        var resultRankValue = currentRankValue + delta;
        var resultRank = Utils.GetCardRank(Mathf.Clamp(resultRankValue, 1,
            Utils.GetCardRankValue(Enums.CardRank.King)));
        Rank.Value = resultRank;
        if (resultRankValue <= 0)
        {
            await GameMgr.AwaitAndDisableInput(cardNode.AnimateLeaveBattle());
        }
    }
}