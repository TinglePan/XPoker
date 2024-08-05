using Godot;

namespace XCardGame.TimingInterfaces;

public interface IPlayerInteract
{
    public bool CanInteract();
    public void Interact();
}