using System;
using Godot;
using XCardGame.Scripts.Common.Constants;
using XCardGame.Scripts.Common.DataBinding;
using XCardGame.Scripts.Ui;

namespace XCardGame.Scripts.Cards;

public class BaseCard
{
    public string Name;
    public string Description;
    public ObservableProperty<Enums.CardFace> Face;
    
    public CardNode Node;

    protected GameMgr GameMgr;
    
    public BaseCard(GameMgr gameMgr, string name, string description, Enums.CardFace face)
    {
        GameMgr = gameMgr;
        Name = name;
        Description = description;
        Face = new ObservableProperty<Enums.CardFace>(nameof(Face), this, face);
    }
    
    public BaseCard(BaseCard card)
    {
        GameMgr = card.GameMgr;
        Name = card.Name;
        Description = card.Description;
        Face = new ObservableProperty<Enums.CardFace>(nameof(Face), this, card.Face.Value);
    }
    
    public override string ToString()
    {
        return $"{Description}({Face.Value})";
    }

    public void Flip()
    {
        Face.Value = Face.Value == Enums.CardFace.Up ? Enums.CardFace.Down : Enums.CardFace.Up;
    }
    
    public virtual void OnFocused()
    {
        GD.Print($"{this} focused");
    }
    
    public virtual void OnLoseFocus()
    {
        GD.Print($"{this} lose focus");
    }
}