﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using Godot;
using XCardGame.Common;
using XCardGame.Ui;

namespace XCardGame;

public partial class CardPile: Node2D
{
    public class SetupArgs
    {
        public List<BaseCard> Cards;
        public bool CountAsField;
        public Enums.CardFace TopCardFaceDirection;
    }
    
    public PackedScene CardPrefab;
    
    public CardNode TopCard { get; private set; }
    public NinePatchRect PileImage { get; private set; }
    public ObservableCollection<BaseCard> Cards { get; private set; }
    public bool CountAsField { get; set; }
    public Enums.CardFace TopCardFaceDirection;

    public bool HasSetup { get; set; }

    public override void _Ready()
    {
        base._Ready();
        TopCard = GetNode<CardNode>("Card");
        PileImage = GetNode<NinePatchRect>("PileImage");
        Cards = new ObservableCollection<BaseCard>();
        Cards.CollectionChanged += OnCardsChanged;
    }

    public virtual void Setup(object o)
    {
        var args = (SetupArgs)o;
        if (args.Cards != null)
        {
            foreach (var card in args.Cards)
            {
                Cards.Add(card);
            }
        }
        CountAsField = args.CountAsField;
        TopCardFaceDirection = args.TopCardFaceDirection;
        TopCard.Setup(new CardNode.SetupArgs
        {
            FaceDirection = TopCardFaceDirection,
            HasPhysics = true,
        });
    }

    public void EnsureSetup()
    {
        if (!HasSetup)
        {
            GD.PrintErr($"{this} not setup yet");
        }
    }
    
    public CardNode CreateCardNodeOnPile(BaseCard card)
    {
        var cardNode = CardPrefab.Instantiate<CardNode>();
        AddChild(cardNode);
        cardNode.Setup(new CardNode.SetupArgs()
        {
            Content = card,
            FaceDirection = TopCardFaceDirection,
            HasPhysics = true,
        });
        cardNode.Position = TopCard.Position;
        return cardNode;
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
    
    protected void AdjustPileImage()
    {
        var count = Cards.Count;
        if (count == 0)
        {
            PileImage.Hide();
        }
        else
        {
            PileImage.Show();
        }
        TopCard.Position = GetTopCardOffset(count);
    }

    protected Vector2 GetTopCardOffset(int count)
    {
        return Configuration.PiledCardOffsetMax * Mathf.Clamp((float)count / Configuration.PileCardCountAtMaxOffset, 0, 1);
    }

    protected void OnCardsChanged(object sender, NotifyCollectionChangedEventArgs args)
    {
        switch (args.Action)
        {
            case NotifyCollectionChangedAction.Add:
            case NotifyCollectionChangedAction.Remove:
            case NotifyCollectionChangedAction.Reset:
                CheckTopCard();
                AdjustPileImage();
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
            TopCard.Content.Value = Cards[0];
        }
        else
        {
            TopCard.Hide();
        }
    }
}