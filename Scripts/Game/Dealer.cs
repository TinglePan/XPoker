using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot;
using XCardGame.Scripts.Cards;
using XCardGame.Scripts.Cards.InteractCards;
using XCardGame.Scripts.Common;
using XCardGame.Scripts.Common.Constants;
using XCardGame.Scripts.Defs.Def.Card;
using XCardGame.Scripts.Ui;

namespace XCardGame.Scripts.Game;

public partial class Dealer: Node2D
{

    public class SetupArgs
    {
        public List<Deck> SourceDecks;
    }
    
    public CardPile DealCardPile;
    public CardPile DiscardCardPile;
    public PackedScene CardPrefab;
    

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
        SourceDecks = new List<Deck>();
    }

    public void Setup(SetupArgs args)
    {
        Battle = GameMgr.CurrentBattle;
        SourceDecks = args.SourceDecks;
        DealCardPile.Setup(new CardPile.SetupArgs
        {
            TopCardFaceDirection = Enums.CardFace.Down,
        });
        DiscardCardPile.Setup(new CardPile.SetupArgs
        {
            TopCardFaceDirection = Enums.CardFace.Down,
        });
        Reset();
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
        foreach (var card in deck.CardList.OfType<PokerCard>())
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
            var card = await DrawCardFromPile(DiscardCardPile);
            if (card == null) return;
            var cardNode = CreateCardNodeOnPile(card, DiscardCardPile);
            cardNode.Reparent(DealCardPile);
            cardNode.AnimateTransform(DealCardPile.TopCard.Position, DealCardPile.TopCard.RotationDegrees, 
                Configuration.AnimateCardTransformInterval, () =>
                {
                    DealCardPile.Cards.Add(card);
                    var cards = DiscardCardPile.TakeN(takeCountForEachAnimatedShuffleCard);
                    foreach (var takenCard in cards)
                    {
                        DealCardPile.Cards.Add(takenCard);
                    }
                    cardNode.QueueFree();
                    if (DiscardCardPile.Cards.Count == 0)
                    {
                        Shuffle();
                    }
                });
            await Utils.Wait(this, Configuration.AnimateCardTransformInterval);
            // await ToSignal(cardNode.TweenControl.GetTween("transform"), Tween.SignalName.Finished);
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

    public async Task DealInnateCards()
    {
        var tasks = new List<Task>();
        foreach (var card in DealCardPile.Cards)
        {
            if (card is BaseInteractCard interactCard && ((InteractCardDef)interactCard.Def).IsInnate)
            {
                var cardNode = CreateCardNodeOnPile(card, DealCardPile);
                Battle.OnDealCard?.Invoke(Battle, cardNode);
                if (interactCard is BaseRuleCard)
                {
                    Battle.RuleCardContainer.ContentNodes.Add(cardNode);
                    tasks.Add(cardNode.TweenControl.WaitComplete("transform"));
                } else if (interactCard is BaseItemCard && Battle.ItemCardContainer.ContentNodes.Count < Battle.Player.ItemPocketSize.Value)
                {
                    Battle.ItemCardContainer.ContentNodes.Add(cardNode);
                    tasks.Add(cardNode.TweenControl.WaitComplete("transform"));
                }
            }
        }
        await Task.WhenAll(tasks);
    }

    public async Task CreateCardAndPutInto(BaseCard card, CardNode onCardNode, Enums.CardFace faceDirection, CardContainer targetContainer)
    {
        var cardNode = CreateCardNodeOnCardNode(card, onCardNode, faceDirection);
        targetContainer.ContentNodes.Add(cardNode);
        await cardNode.TweenControl.WaitComplete("transform");
    }
    
    public async Task DealCardIntoContainer(CardContainer targetContainer)
    {
        var cardNode = await AnimateDrawCard();
        
        targetContainer.ContentNodes.Add(cardNode);
        await cardNode.TweenControl.WaitComplete("transform");
    }

    public async Task DealCardAndReplace(CardNode node)
    {
        var cardNode = await AnimateDrawCard();
        var tasks = new List<Task>();
        if (node.CurrentContainer.Value != null)
        {
            var cardContainer = (CardContainer)node.CurrentContainer.Value; 
            var index = cardContainer.ContentNodes.IndexOf(node);
            tasks.Add(AnimateDiscard(node));
            cardContainer.ContentNodes.Insert(index, cardNode);
            tasks.Add(cardNode.TweenControl.WaitComplete("transform"));
        }
        else
        {
            var parent = node.GetParent();
            tasks.Add(AnimateDiscard(node));
            cardNode.Reparent(parent);
            tasks.Add(cardNode.AnimateTransform(node.Position, node.RotationDegrees, Configuration.AnimateCardTransformInterval,
                conflictTweenAction:TweenControl.ConflictTweenAction.Finish));
        }
        await Task.WhenAll(tasks);
    }

    public async Task<CardNode> AnimateDrawCard()
    {
        var card = await DrawCardFromPile(DealCardPile);
        if (card == null) return null;
        var cardNode = CreateCardNodeOnPile(card, DealCardPile);
        Battle.OnDealCard?.Invoke(Battle, cardNode);
        return cardNode;
    }

    public async Task AnimateDiscard(CardNode node)
    {
        // GD.Print($"animate discard {node}");
        if (node.CurrentContainer != null)
        {
            node.CurrentContainer.Value.Contents.Remove(node.Content.Value);
        }
        node.Reparent(DiscardCardPile);
        node.AnimateTransform(DiscardCardPile.TopCard.Position, DiscardCardPile.TopCard.RotationDegrees, 
            Configuration.AnimateCardTransformInterval);
        await node.TweenControl.WaitComplete("transform");
        Discard(node);
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
        GD.Print($"discard {cardNode}");
        DiscardCardPile.Cards.Insert(0, cardNode.Card);
        cardNode.QueueFree();
    }
    
    protected CardNode CreateCardNodeOnPile(BaseCard card, CardPile pile)
    {
        var cardNode = CardPrefab.Instantiate<CardNode>();
        pile.AddChild(cardNode);
        cardNode.Setup(new CardNode.SetupArgs()
        {
            Content = card,
            FaceDirection = pile.TopCardFaceDirection,
            HasPhysics = true,
        });
        cardNode.Position = pile.TopCard.Position;
        return cardNode;
    }

    protected CardNode CreateCardNodeOnCardNode(BaseCard card, CardNode onCardNode, Enums.CardFace faceDirection)
    {
        var cardNode = CardPrefab.Instantiate<CardNode>();
        onCardNode.GetParent().AddChild(cardNode);
        cardNode.Setup(new CardNode.SetupArgs()
        {
            Content = card,
            FaceDirection = faceDirection,
            HasPhysics = true,
        });
        cardNode.Position = onCardNode.Position;
        return cardNode;
    }
}