using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using Godot;
using XCardGame.Common;
using XCardGame.Ui;

namespace XCardGame;

public abstract partial class BaseCardPile: Node2D
{
    
    public class SetupArgs
    {
        public GameMgr GameMgr;
        public Battle Battle;
        public List<BaseCard> Cards;
        public Enums.GrowDirection DefaultAddCardDirection; 
    }

    public GameMgr GameMgr;
    public Battle Battle;
    public CardNode TopCardNode;
    public NinePatchRect PileImage;
    public ObservableCollection<BaseCard> Cards;
    public Enums.GrowDirection DefaultAddCardDirection;

    public override void _Ready()
    {
        base._Ready();
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
        GameMgr = args.GameMgr;
        Battle = args.Battle;
        DefaultAddCardDirection = args.DefaultAddCardDirection;
    }
    
    public CardNode CreateCardNodeOnPile(BaseCard card)
    {
        var cardNode = Battle.InstantiateCardNode(card, this);
        cardNode.Setup(new CardNode.SetupArgs()
        {
            Content = card,
            FaceDirection = TopCardNode.FaceDirection.Value,
            HasPhysics = true,
        });
        cardNode.Position = TopCardNode.Position;
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

    public void AddCard(BaseCard card, int index = -1)
    {
        if (index >= 0)
        {
            Cards.Insert(index, card);
        }
        else
        {
            if (DefaultAddCardDirection == Enums.GrowDirection.FromBegin)
            {
                Cards.Insert(0, card);
            }
            else
            {
                Cards.Add(card);
            }
        }
    }
    
    protected void OnCardsChanged(object sender, NotifyCollectionChangedEventArgs args)
    {
        Adjust();
    }

    protected abstract void Adjust();
}