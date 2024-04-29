using Godot;
using XCardGame.Scripts.Common.Constants;

namespace XCardGame.Scripts.Cards.AbilityCards;

public class BaseAbilityCard: BaseCard
{
    public string IconPath;
    
    protected GameMgr GameMgr;
    
    public BaseAbilityCard(GameMgr gameMgr, string name, string description, Enums.CardFace face, GameLogic.BattleEntity owner, string iconPath) : base(name, description, face, owner)
    {
        GameMgr = gameMgr;
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