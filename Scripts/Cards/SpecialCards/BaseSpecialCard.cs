using Godot;
using XCardGame.Scripts.Common.Constants;

namespace XCardGame.Scripts.Cards.SpecialCards;

public class BaseSpecialCard: BaseCard
{
    public GameLogic.PokerPlayer Owner;
    
    public string IconPath;
    
    public BaseSpecialCard(GameMgr gameMgr, string name, string description, GameLogic.PokerPlayer owner, Enums.CardFace face, string iconPath) : base(gameMgr, name, description, face)
    {
        Owner = owner;
        IconPath = iconPath;
    }

    public virtual bool CanActivate()
    {
        return false;
    }
    
    public virtual void Activate()
    {
        GD.Print($"Special Card {this} Activated");
    }
    
    
}