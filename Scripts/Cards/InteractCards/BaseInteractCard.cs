using System;
using Godot;
using XCardGame.Common;
using XCardGame.Ui;

namespace XCardGame;

public class BaseInteractCard: BaseCard, IInteractCard
{
    public ObservableProperty<int> Cost;
    public Action<BaseInteractCard> OnOverload;
    
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

    public override void ChangeRank(int delta)
    {
        if (Rank.Value != Enums.CardRank.None)
        {
            base.ChangeRank(delta);
            if (Rank.Value == Enums.CardRank.None)
            {
                OnOverload?.Invoke(this);
            }
        }
    }

    protected virtual async void OnRoundEnd(Battle battle)
    {
        if (Rank.Value == Enums.CardRank.None)
        {
            var cardNode = Node<CardNode>();
            await GameMgr.AwaitAndDisableInput(cardNode?.AnimateLeaveBattle());
        }
    }
}