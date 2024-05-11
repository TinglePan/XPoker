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

    public void Flip()
    {
        Face.Value = Face.Value == Enums.CardFace.Up ? Enums.CardFace.Down : Enums.CardFace.Up;
    }
    
    public void Disposal()
    {
        OnDisposalFromField(Battle);
        Node.QueueFree();
        Node = null;
    }
    
    public virtual void OnAppearInField(Battle battle)
    {
        
    }

    public virtual void OnDisposalFromField(Battle battle)
    {
        
    }
    
    protected void OnFaceChanged(object sender, ValueChangedEventDetailedArgs<Enums.CardFace> args)
    {
        if (args.NewValue == Enums.CardFace.Up)
        {
            OnAppearInField(Battle);
        }
    }
}