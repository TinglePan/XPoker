using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using Godot;
using XCardGame.Scripts.Cards;
using XCardGame.Scripts.Common.Constants;
using XCardGame.Scripts.Nodes;

namespace XCardGame.Scripts.GameLogic;

public partial class CardPile: Node2D, ISetup
{
    public CardNode TopCard;
    public NinePatchRect PileImage;
    public ObservableCollection<BaseCard> Cards;
    public Enums.CardFace TopCardFaceDirection;

    public bool HasSetup { get; set; }

    public override void _Ready()
    {
        base._Ready();
        TopCard = GetNode<CardNode>("Card");
        PileImage = GetNode<NinePatchRect>("PileImage");
    }

    public void Setup(Dictionary<string, object> args)
    {
        Cards = (ObservableCollection<BaseCard>)args["cards"];
        Cards.CollectionChanged += OnCardsChanged;
        TopCardFaceDirection = (Enums.CardFace)args["topCardFaceDirection"];
    }

    public void EnsureSetup()
    {
        if (!HasSetup)
        {
            GD.PrintErr($"{this} not setup yet");
        }
    }
    
    public BaseCard Take(int index = 0)
    {
        var card = Peek(index);
        if (card != null)
        {
            Cards.RemoveAt(index);
        }

        return card;
    }

    public List<BaseCard> TakeN(int n)
    {
        var res = new List<BaseCard>();
        for (int i = 0; i < n; i++)
        {
            var card = Take();
            if (card != null)
            {
                res.Add(card);
            }
            else
            {
                break;
            }
        }
        return res;
    }

    public BaseCard Take(Func<BaseCard, bool> filter)
    {
        var card = Cards.FirstOrDefault(filter);
        if (card != null)
        {
            Cards.Remove(card);
        }
        return card;
    }
    

    public BaseCard Peek(int index = 0)
    {
        if (index >= Cards.Count)
        {
            return null;
        }
        return Cards[index];
    }
    
    public BaseCard Peek(Func<BaseCard, bool> filter)
    {
        return Cards.FirstOrDefault(filter);
    }

    protected void OnCardsChanged(object sender, NotifyCollectionChangedEventArgs args)
    {
        switch (args.Action)
        {
            case NotifyCollectionChangedAction.Add:
            case NotifyCollectionChangedAction.Remove:
            case NotifyCollectionChangedAction.Reset:
                CheckTopCard();
                CheckPile();
                break;
            case NotifyCollectionChangedAction.Replace:
                CheckTopCard();
                break;
        }
        
        
    }

    protected void CheckTopCard()
    {
        if (Cards.Count > 0)
        {
            TopCard.Show();
            if (TopCard.HasSetup)
            {
                TopCard.Content.Value = Cards[0];
            }
            else
            {
                TopCard.Setup(new Dictionary<string, object>()
                {
                    { "card", Cards[0] },
                    { "container", null },
                    { "faceDirection", TopCardFaceDirection }
                });
            }
        }
        else
        {
            TopCard.Hide();
        }
    }

    protected void CheckPile()
    {
        switch (Cards.Count)
        {
            // NYI: update pile sprite according to left card count
            case 0:
                PileImage.Hide();
                break;
            default:
                PileImage.Show();
                break;
        }
    }
}