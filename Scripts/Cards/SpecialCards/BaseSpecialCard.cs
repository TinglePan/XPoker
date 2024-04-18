using Godot;
using XCardGame.Scripts.Common.Constants;

namespace XCardGame.Scripts.Cards.SpecialCards;

public class BaseSpecialCard: BaseCard
{
    public PokerPlayer Owner;
    
    public string IconPath;
    
    public BaseSpecialCard(GameMgr gameMgr, string name, string description, PokerPlayer player, Enums.CardFace face, string iconPath) : base(gameMgr, name, description, face)
    {
        Owner = player;
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