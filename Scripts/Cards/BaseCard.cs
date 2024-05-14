using System;
using System.Collections.Generic;
using Godot;
using XCardGame.Scripts.Common.Constants;
using XCardGame.Scripts.Common.DataBinding;
using XCardGame.Scripts.GameLogic;
using XCardGame.Scripts.Ui;

namespace XCardGame.Scripts.Cards;

public class BaseCard: ISetup, ILifeCycleTriggeredInBattle
{
    public string Name;
    public string Description;
    public string TexturePath;
    public ObservableProperty<Enums.CardFace> Face;
    public ObservableProperty<Enums.CardSuit> Suit;
    public ObservableProperty<Enums.CardRank> Rank;
    public ObservableProperty<bool> IsNegated;
    
    public BattleEntity Owner;

    public CardNode Node;
    public GameMgr GameMgr;
    public Battle Battle;
    
    public Enums.CardColor CardColor => Suit.Value switch
    {
        Enums.CardSuit.Spades => Enums.CardColor.Black,
        Enums.CardSuit.Clubs => Enums.CardColor.Black,
        Enums.CardSuit.Hearts => Enums.CardColor.Red,
        Enums.CardSuit.Diamonds => Enums.CardColor.Red,
        _ => Enums.CardColor.None
    };
    
    public BaseCard(string name, string description, string texturePath, Enums.CardFace face,
        Enums.CardSuit suit = Enums.CardSuit.None, Enums.CardRank rank = Enums.CardRank.None, BattleEntity owner=null)
    {
        Name = name;
        Description = description;
        TexturePath = texturePath;
        Face = new ObservableProperty<Enums.CardFace>(nameof(Face), this, face);
        Suit = new ObservableProperty<Enums.CardSuit>(nameof(Suit), this, suit);
        Rank = new ObservableProperty<Enums.CardRank>(nameof(Rank), this, rank);
        IsNegated = new ObservableProperty<bool>(nameof(IsNegated), this, false);
        Owner = owner;
    }
    
    public override string ToString()
    {
        return $"{Description}({Face.Value})";
    }

    public virtual void Setup(Dictionary<string, object> args)
    {
        GameMgr = (GameMgr)args["gameMgr"];
        Node = (CardNode)args["node"];
        Battle = GameMgr.CurrentBattle;
        Face.DetailedValueChanged += OnFaceChanged;
        Face.FireValueChangeEventsOnInit();
    }

    public virtual void Flip()
    {
        Face.Value = Face.Value == Enums.CardFace.Up ? Enums.CardFace.Down : Enums.CardFace.Up;
    }
    
    public void Disposal()
    {
        Node.QueueFree();
        Node = null;
        OnDisappear(Battle);
    }

    public virtual void OnAppear(Battle battle)
    {
        
    }

    public virtual void OnDisappear(Battle battle)
    {
        
    }
    
    public virtual void OnDisposal(Battle battle)
    {
        OnDisappear(battle);
    }
    
    protected virtual void OnFaceChanged(object sender, ValueChangedEventDetailedArgs<Enums.CardFace> args)
    {
        if (args.NewValue == Enums.CardFace.Up)
        {
            OnAppear(Battle);
        } else if (args is { NewValue: Enums.CardFace.Down, OldValue: Enums.CardFace.Up })
        {
            OnDisappear(Battle);
        }
    }
}