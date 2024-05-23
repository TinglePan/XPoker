using System.Collections.Generic;
using XCardGame.Scripts.Common.Constants;

namespace XCardGame.Scripts.Cards;

public abstract class BaseInteractCard: BaseCard, IInteractCard
{
    public List<Enums.CardInteractions> Interactions { get; }
    
    public BaseInteractCard(string name, string description, string iconPath, Enums.CardSuit suit,
        Enums.CardRank rank) : 
        base(name, description, iconPath, suit, rank)
    {
        Interactions = new List<Enums.CardInteractions>();
    }
    
    public abstract bool CanInteract();
    public Enums.CardInteractions DefaultInteraction() => Interactions.Count == 1 ? Interactions[0] : Enums.CardInteractions.None;

    public virtual void Interact()
    {
        
    }
}