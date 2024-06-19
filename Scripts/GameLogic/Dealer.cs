using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Godot;
using XCardGame.Scripts.Cards;
using XCardGame.Scripts.Common;
using XCardGame.Scripts.Common.Constants;
using XCardGame.Scripts.Nodes;

namespace XCardGame.Scripts.GameLogic;

public partial class Dealer: Node2D, ISetup
{
    public CardPile DealCardPile;
    public CardPile DiscardCardPile;
    public PackedScene CardPrefab;
    
    public bool HasSetup { get; set; }

    public List<Deck> SourceDecks;

    protected GameMgr GameMgr;
    protected Battle Battle;

    public override void _Ready()
    {
        base._Ready();
        GameMgr = GetNode<GameMgr>("/root/GameMgr");
        DealCardPile = GetNode<CardPile>("DealPile");
        DiscardCardPile = GetNode<CardPile>("DiscardPile");
        CardPrefab = ResourceCache.Instance.Load<PackedScene>("res://Scenes/Card.tscn");
        HasSetup = false;
        Battle = GameMgr.CurrentBattle;
        SourceDecks = new List<Deck>();
    }

    public void Setup(Dictionary<string, object> args)
    {
        SourceDecks = (List<Deck>)args["sourceDecks"];
        DealCardPile.Setup(new Dictionary<string, object>()
        {
            { "cards", new List<BaseCard>() },
            { "topCardFaceDirection", Enums.CardFace.Down }
        });
        DiscardCardPile.Setup(new Dictionary<string, object>()
        {
            { "cards", new List<BaseCard>() },
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
    

    public async Task AnimateShuffle()
    {
        // GD.Print("animate shuffle");
        var shuffleAnimateCardCount = Mathf.Min(DiscardCardPile.Cards.Count, Configuration.ShuffleAnimateCards);
        var takeCountForEachAnimatedShuffleCard = DiscardCardPile.Cards.Count / shuffleAnimateCardCount;
        for (int i = 0; i < shuffleAnimateCardCount; i++)
        {
            var delay = Configuration.AnimateCardTransformInterval * i;
            var timer = GetTree().CreateTimer(delay);
            await ToSignal(timer, Timer.SignalName.Timeout);
            var card = await DrawCardFromPile(DiscardCardPile);
            if (card == null) return;
            var cardNode = CreateCardNodeOnPile(card, DiscardCardPile);
            cardNode.Reparent(DealCardPile);
            cardNode.TweenTransform(DealCardPile.TopCard.Position, DealCardPile.TopCard.RotationDegrees, Configuration.AnimateCardTransformInterval);
            await ToSignal(cardNode.TweenControl.GetTween("transform"), Tween.SignalName.Finished);
            DealCardPile.Cards.Add(card);
            var cards = DiscardCardPile.TakeN(takeCountForEachAnimatedShuffleCard);
            foreach (var takenCard in cards)
            {
                DealCardPile.Cards.Add(takenCard);
            }
            if (DiscardCardPile.Cards.Count == 0)
            {
                Shuffle();
            }
            cardNode.QueueFree();
        }
    }
    
    public void Shuffle()
    {
        var cards = DealCardPile.Cards.ToList();
        foreach (var card in DiscardCardPile.Cards)
        {
            cards.Add(card);
        }
        DiscardCardPile.Cards.Clear();
        int n = cards.Count;
        while (n > 1)
        {
            n--;
            int k = GameMgr.Rand.Next(n + 1);
            (cards[k], cards[n]) = (cards[n], cards[k]);
        }
        DealCardPile.Cards.Clear();
        foreach (var card in cards)
        {
            DealCardPile.Cards.Add(card);
        }
    }
    
    public async void DealCardIntoContainer(CardContainer targetContainer, float delay = 0f)
    {
        if (delay > 0)
        {
            var timer = GetTree().CreateTimer(delay);
            await ToSignal(timer, Timer.SignalName.Timeout);
        }
        var card = await DrawCardFromPile(DealCardPile);
        if (card == null) return;
        var cardNode = CreateCardNodeOnPile(card, DealCardPile);
        targetContainer.ContentNodes.Add(cardNode);
    }

    public async void DealCardAndReplace(CardNode node, float delay = 0f)
    {
        if (delay > 0)
        {
            var timer = GetTree().CreateTimer(delay);
            await ToSignal(timer, Timer.SignalName.Timeout);
        }
        var card = await DrawCardFromPile(DealCardPile);
        if (card == null) return;
        var cardNode = CreateCardNodeOnPile(card, DealCardPile);
        if (node.Container != null)
        {
            var cardContainer = (CardContainer)node.Container; 
            var index = cardContainer.ContentNodes.IndexOf(node);
            AnimateDiscard(node);
            cardContainer.ContentNodes.Insert(index, cardNode);
            await ToSignal(cardNode.TweenControl.GetTween("transform"), Tween.SignalName.Finished);
        }
        else
        {
            cardNode.TweenTransform(node.Position, node.RotationDegrees, Configuration.AnimateCardTransformInterval);
            await ToSignal(node.TweenControl.GetTween("transform"), Tween.SignalName.Finished);
            node.Content.Value = cardNode.Content.Value;
            cardNode.QueueFree();
        }
    }

    public async void AnimateDiscard(CardNode node, float delay = 0f)
    {
        if (delay > 0)
        {
            var timer = GetTree().CreateTimer(delay);
            await ToSignal(timer, Timer.SignalName.Timeout);
        }
        
        // GD.Print($"animate discard {node}");
        node.Container.Contents.Remove(node.Content.Value);
        node.Reparent(DiscardCardPile);
        node.TweenTransform(DiscardCardPile.TopCard.Position, DiscardCardPile.TopCard.RotationDegrees,
            Configuration.AnimateCardTransformInterval, () => Discard(node));
    }

    protected async Task<BaseCard> DrawCardFromPile(CardPile pile)
    {
        var card = pile.Take();
        if (card == null)
        {
            if (pile == DealCardPile)
            {
                await AnimateShuffle();
                card = pile.Take();
            }
            if (card == null)
            {
                GD.Print("No more cards to deal");
            }
        }
        return card;
    }

    protected void Discard(CardNode cardNode)
    {
        // GD.Print($"discard {cardNode}");
        DiscardCardPile.Cards.Insert(0, cardNode.Content.Value);
        cardNode.QueueFree();
    }
    
    protected CardNode CreateCardNodeOnPile(BaseCard card, CardPile pile)
    {
        var cardNode = CardPrefab.Instantiate<CardNode>();
        pile.AddChild(cardNode);
        cardNode.Setup(new Dictionary<string, object>()
        {
            { "card", card },
            { "container", null },
            { "faceDirection", pile.TopCardFaceDirection }
        });
        cardNode.Position = pile.TopCard.Position;
        return cardNode;
    }
}