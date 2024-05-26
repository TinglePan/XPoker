using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Godot;
using XCardGame.Scripts.Cards;

using XCardGame.Scripts.Common.Constants;
using XCardGame.Scripts.Nodes;

namespace XCardGame.Scripts.GameLogic;

public partial class CardPile: Node2D, ISetup
{
    [Export]
    public CardNode TopCardNode;
    [Export]
    public PackedScene CardPrefab;
    
    public bool HasSetup { get; set; }

    public List<Deck> SourceDecks;
    public List<MarkerCard> ExcludedCards;
    public List<MarkerCard> CardList;
    public List<MarkerCard> DealingCards;
    public List<MarkerCard> DealtCards;
    
    public CardPile()
    {
        HasSetup = false;
        SourceDecks = new List<Deck>();
        CardList = new List<MarkerCard>();
        DealingCards = new List<MarkerCard>();
        DealtCards = new List<MarkerCard>();
    }
    
    public CardPile(List<Deck> srcDecks, List<MarkerCard> excludedCards = null)
    {
        HasSetup = false;
        SourceDecks = srcDecks;
        ExcludedCards = excludedCards;
        CardList = new List<MarkerCard>();
        DealingCards = new List<MarkerCard>();
        DealtCards = new List<MarkerCard>();
        Reset();
    }

    public override void _Ready()
    {
        base._Ready();
        HasSetup = false;
        CardList = new List<MarkerCard>();
        DealingCards = new List<MarkerCard>();
        DealtCards = new List<MarkerCard>();
    }

    public void Setup(Dictionary<string, object> args)
    {
        SourceDecks = (List<Deck>)args["sourceDecks"];
        ExcludedCards = (List<MarkerCard>)args["excludedCards"];
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
        foreach (var deck in SourceDecks)
        {
            MixIn(deck, ExcludedCards);
        }
        Shuffle();
        ResetTopCard();
    }
    
    public void MixIn(Deck deck, List<MarkerCard> excludedCards = null)
    {
        foreach (var card in deck.CardList.OfType<MarkerCard>())
        {
            if (excludedCards != null && excludedCards.Contains(card))
            {
                continue;
            }
            CardList.Add(card);
        }
    }

    public void Shuffle()
    {
        // TODO: Play shuffle animation
        DealingCards.Clear();
        DealtCards.Clear();
        foreach (var card in CardList)
        {
            DealingCards.Add(card);
        }
        Random rng = new Random();
        int n = DealingCards.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            (DealingCards[k], DealingCards[n]) = (DealingCards[n], DealingCards[k]);
        }
    }
    
    public async void DealCardAppend(CardContainer targetContainer)
    {
        var cardNode = CardPrefab.Instantiate<CardNode>();
        cardNode.Setup(new Dictionary<string, object>()
        {
            { "card", TopCardNode.Content.Value },
            { "container", null },
            { "faceDirection", Enums.CardFace.Down }
        });
        cardNode.Position = TopCardNode.Position;
        await targetContainer.AddContentNode(targetContainer.Contents.Count, cardNode, Configuration.DealCardTweenTime);
        cardNode.Container = targetContainer;
        ResetTopCard();
    }

    public async void DealCardReplace(CardNode node)
    {
        var cardNode = CardPrefab.Instantiate<CardNode>();
        cardNode.Setup(new Dictionary<string, object>()
        {
            { "card", TopCardNode.Content.Value },
            { "container", null },
            { "faceDirection", node.FaceDirection }
        });
        cardNode.Position = TopCardNode.Position;
        var replacedContentNode = await node.Container.ReplaceContentNode(node.Container.ContentNodes.IndexOf(node),
            cardNode, Configuration.DealCardTweenTime);
        replacedContentNode.QueueFree();
        ResetTopCard();
    }
    
    public MarkerCard Take(int index = 0)
    {
        if (index >= CardList.Count)
        {
            GD.PrintErr($"CardPile.Deal: index {index} >= CardList.Count {CardList.Count}");
            return null;
        }
        if (index >= DealingCards.Count)
        {
            Shuffle();
        }
        var card = DealingCards[index];
        DealingCards.RemoveAt(index);
        DealtCards.Add(card);
        return card;
    }

    public MarkerCard SearchFirst(Func<MarkerCard, bool> filter)
    {
        for (int i = 0; i < DealingCards.Count; i++)
        {
            var card = DealingCards[i];
            if (filter(card))
            {
                return Take(i);
            }
        }

        return null;
    }
    
    protected void ResetTopCard()
    {
        if (DealingCards.Count == 0)
        {
            Shuffle();
        }
        var topCard = Take();
        TopCardNode.Setup(new Dictionary<string, object>()
        {
            { "card", topCard },
            { "Container", null },
            { "faceDirection", Enums.CardFace.Down }
        });
    }
}