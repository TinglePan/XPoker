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
    public ObservableProperty<Enums.CardSuit> Suit;
    public ObservableProperty<bool> IsFocused;
    public ObservableProperty<bool> IsSelected;
    
    public BattleEntity Owner;
    public CardNode Node;
    
    public Enums.CardColor CardColor => Suit.Value switch
    {
        Enums.CardSuit.Spades => Enums.CardColor.Black,
        Enums.CardSuit.Clubs => Enums.CardColor.Black,
        Enums.CardSuit.Hearts => Enums.CardColor.Red,
        Enums.CardSuit.Diamonds => Enums.CardColor.Red,
        _ => Enums.CardColor.None
    };
    
    public BaseCard(string name, string description, Enums.CardFace face, Enums.CardSuit suit, BattleEntity owner=null)
    {
        Name = name;
        Description = description;
        Face = new ObservableProperty<Enums.CardFace>(nameof(Face), this, face);
        Suit = new ObservableProperty<Enums.CardSuit>(nameof(Suit), this, suit);
        IsFocused = new ObservableProperty<bool>(nameof(IsFocused), this, false);
        IsSelected = new ObservableProperty<bool>(nameof(IsSelected), this, false);
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

    public virtual void Flip(Battle battle, BattleEntity flippedBy)
    {
        Face.Value = Face.Value == Enums.CardFace.Up ? Enums.CardFace.Down : Enums.CardFace.Up;
    }
}