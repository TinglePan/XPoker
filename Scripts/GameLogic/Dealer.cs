using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using Godot;
using XCardGame.Scripts.Cards;

using XCardGame.Scripts.Common.Constants;
using XCardGame.Scripts.Nodes;

namespace XCardGame.Scripts.GameLogic;

public partial class Dealer: Node2D, ISetup
{
    [Export] public CardPile DealCardPile;
    [Export] public CardPile DiscardCardPile;
    [Export] public PackedScene CardPrefab;
    
    public bool HasSetup { get; set; }

    public List<Deck> SourceDecks;

    protected GameMgr GameMgr;
    protected Battle Battle;

    public override void _Ready()
    {
        base._Ready();
        HasSetup = false;
        GameMgr = GetNode<GameMgr>("/root/GameMgr");
        Battle = GameMgr.CurrentBattle;
        SourceDecks = new List<Deck>();
    }

    public void Setup(Dictionary<string, object> args)
    {
        SourceDecks = (List<Deck>)args["sourceDecks"];
        DealCardPile.Setup(new Dictionary<string, object>()
        {
            { "cards", new ObservableCollection<BaseCard>() },
            { "topCardFaceDirection", Enums.CardFace.Down }
        });
        DiscardCardPile.Setup(new Dictionary<string, object>()
        {
            { "cards", new ObservableCollection<BaseCard>() },
            { "topCardFaceDirection", Enums.CardFace.Up }
        });
        Reset();
        HasSetup = true;
    }
    
    public void EnsureSetup()
    {
        if (!HasSetup)
        {
            GD.PrintErr($"{this} not setup yet");
        }
    }

    public void Reset()
    {
        DealCardPile.Cards.Clear();
        foreach (var deck in SourceDecks)
        {
            MixIn(deck);
        }
        Shuffle();
    }
    
    public void MixIn(Deck deck)
    {
        foreach (var card in deck.CardList.OfType<MarkerCard>())
        {
            DealCardPile.Cards.Add(card);
        }
    }

    public void Shuffle()
    {
        // TODO: Play shuffle animation
        var cards = DealCardPile.Cards.ToList();
        cards.AddRange(DiscardCardPile.Cards);
        int n = DealCardPile.Cards.Count;
        while (n > 1)
        {
            n--;
            int k = GameMgr.Rand.Next(n + 1);
            (cards[k], cards[n]) = (cards[n], cards[k]);
        }
        DiscardCardPile.Cards.Clear();
        DealCardPile.Cards.Clear();
        foreach (var card in cards)
        {
            DealCardPile.Cards.Add(card);
        }
    }
    
    public CardNode DealCardIntoContainer(CardContainer targetContainer, Action callback, float delay = 0f)
    {
        var cardNode = DealCardIntoContainer(targetContainer, delay);
        if (cardNode != null)
        {
            cardNode.TransformTweenControl.Callback.Value = callback;
        }
        return cardNode;
    }
    
    public CardNode DealCardIntoContainer(CardContainer targetContainer, float delay = 0f)
    {
        var card = DrawCard();
        if (card == null) return null;
        var cardNode = CardPrefab.Instantiate<CardNode>();
        DealCardPile.AddChild(cardNode);
        cardNode.Setup(new Dictionary<string, object>()
        {
            { "card", card },
            { "container", null },
            { "faceDirection", Enums.CardFace.Down }
        });
        cardNode.Position = DealCardPile.TopCard.Position;
        if (delay > 0)
        {
            var timer = GetTree().CreateTimer(delay);
            timer.Timeout += () =>
            {
                targetContainer.ContentNodes.Insert(targetContainer.ContentNodes.Count, cardNode);
            };
        }
        else
        {
            targetContainer.ContentNodes.Insert(targetContainer.ContentNodes.Count, cardNode);
        }
        return cardNode;
    }

    public CardNode DealCardAndReplace(CardNode node, Action callback, float delay = 0f)
    {
        var cardNode = DealCardAndReplace(node, delay);
        if (cardNode != null)
        {
            cardNode.TransformTweenControl.Callback.Value = callback;
        }
        return cardNode;
    }
    
    public CardNode DealCardAndReplace(CardNode node, float delay = 0f)
    {
        void ReplaceWithCardNode(CardNode cardNode)
        {
            cardNode.TweenTransform(node.Position, node.RotationDegrees, Configuration.AnimateCardTransformInterval);
            cardNode.TransformTweenControl.Callback.Value = () =>
            {
                node.Content.Value = cardNode.Content.Value;
                cardNode.QueueFree();
            };
        }
        
        var card = DrawCard();
        if (card == null) return null;
        var cardNode = CardPrefab.Instantiate<CardNode>();
        DealCardPile.AddChild(cardNode);
        
        cardNode.Setup(new Dictionary<string, object>()
        {
            { "card", DealCardPile.Take() },
            { "container", null },
            { "faceDirection", node.FaceDirection.Value }
        });
        cardNode.Position = DealCardPile.TopCard.Position;
        if (node.Container != null)
        {
            var index = node.Container.ContentNodes.IndexOf(node);
            AnimateDiscard(node, delay);
            node.Container.ContentNodes.Insert(index, cardNode);
            return cardNode;
        }
        else
        {
            if (delay > 0)
            {
                var timer = GetTree().CreateTimer(delay);
                timer.Timeout += () => ReplaceWithCardNode(cardNode);
            }
            else
            {
                ReplaceWithCardNode(cardNode);
            }
            return node;
        }
    }

    public void AnimateDiscard(CardNode node, float delay = 0f)
    {
        void Run()
        {
            node.Container.Contents.Remove(node.Content.Value);
            node.Reparent(DiscardCardPile);
            node.TweenTransform(DiscardCardPile.TopCard.Position, DiscardCardPile.TopCard.RotationDegrees,
                Configuration.AnimateCardTransformInterval, () => Discard(node));
        }
        GD.Print($"animate discard {node}");
        if (delay > 0)
        {
            var timer = GetTree().CreateTimer(delay);
            timer.Timeout += Run;
        }
        else
        {
            Run();
        }
    }

    protected BaseCard DrawCard()
    {
        var card = DealCardPile.Take();
        if (card == null)
        {
            Shuffle();
            card = DealCardPile.Take();
            if (card == null)
            {
                GD.Print("No more cards to deal");
            }
        }
        return card;
    }

    protected void Discard(CardNode cardNode)
    {
        GD.Print($"discard {cardNode}");
        DiscardCardPile.Cards.Insert(0, cardNode.Content.Value);
        cardNode.QueueFree();
    }
}