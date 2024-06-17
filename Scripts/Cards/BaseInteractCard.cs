using System.Collections.Generic;
using XCardGame.Scripts.Common.Constants;
using XCardGame.Scripts.Defs;
using XCardGame.Scripts.GameLogic;

namespace XCardGame.Scripts.Cards;

public abstract class BaseInteractCard: BaseCard, IInteractCard
{
    public List<Enums.CardInteractions> Interactions { get; }
    
    public BaseInteractCard(BaseCardDef def) : base(def)
    {
        Interactions = new List<Enums.CardInteractions>();
    }
    
    public abstract bool CanInteract();
    public Enums.CardInteractions DefaultInteraction() => Interactions.Count == 1 ? Interactions[0] : Enums.CardInteractions.None;

    public virtual void Interact()
    {
        
    }
}