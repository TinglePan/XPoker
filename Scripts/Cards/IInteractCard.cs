using CardNode = XCardGame.Ui.CardNode;

namespace XCardGame;

public interface IInteractCard
{
    // public List<Enums.CardInteractions> Interactions { get; }
    public bool CanInteract(CardNode node);
    // public Enums.CardInteractions DefaultInteraction();

    public void Interact(CardNode node);
}