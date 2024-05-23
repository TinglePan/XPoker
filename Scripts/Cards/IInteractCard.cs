using System.Collections.Generic;
using XCardGame.Scripts.Common.Constants;

namespace XCardGame.Scripts.Cards;

public interface IInteractCard
{
    public List<Enums.CardInteractions> Interactions { get; }
    public bool CanInteract();

    public Enums.CardInteractions DefaultInteraction();

    public void Interact();
}