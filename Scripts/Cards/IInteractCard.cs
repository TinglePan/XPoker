using System.Collections.Generic;
using XCardGame.Scripts.Common.Constants;
using XCardGame.Scripts.Nodes;

namespace XCardGame.Scripts.Cards;

public interface IInteractCard
{
    // public List<Enums.CardInteractions> Interactions { get; }
    public bool CanInteract(CardNode node);
    // public Enums.CardInteractions DefaultInteraction();

    public void Interact(CardNode node);
}