using System;
using Godot;
using XCardGame.Scripts.Common.Constants;
using XCardGame.Scripts.Common.DataBinding;
using XCardGame.Scripts.GameLogic;
using XCardGame.Scripts.Ui;

namespace XCardGame.Scripts.Cards;

public class BaseCard
{
    public string Name;
    public string Description;
    public ObservableProperty<Enums.CardFace> Face;
    public BattleEntity Owner;
    
    public CardNode Node;
    
    public BaseCard(string name, string description, Enums.CardFace face, BattleEntity owner=null)
    {
        Name = name;
        Description = description;
        Face = new ObservableProperty<Enums.CardFace>(nameof(Face), this, face);
        Owner = owner;
    }
    
    public BaseCard(BaseCard card)
    {
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