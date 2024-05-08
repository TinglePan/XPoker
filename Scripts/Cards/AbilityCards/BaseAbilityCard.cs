using Godot;
using XCardGame.Scripts.Common.Constants;
using XCardGame.Scripts.GameLogic;

namespace XCardGame.Scripts.Cards.AbilityCards;

public class BaseAbilityCard: BaseCard
{
    public string IconPath;
    
    protected GameMgr GameMgr;
    protected Battle Battle;
    
    public BaseAbilityCard(GameMgr gameMgr, string name, string description, Enums.CardFace face, GameLogic.BattleEntity owner, string iconPath) : base(name, description, face, owner)
    {
        GameMgr = gameMgr;
        Battle = GameMgr.CurrentBattle;
        IconPath = iconPath;
    }

    public virtual bool CanActivate()
    {
        return false;
    }
    
    public virtual void Activate()
    {
        GD.Print($"Ability Card {this} Activated");
    }
    
    
}