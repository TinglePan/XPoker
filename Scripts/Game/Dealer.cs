using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot;
using XCardGame.CardProperties;
using XCardGame.Common;
using XCardGame.Ui;

namespace XCardGame;

public partial class Dealer: Node2D
{

    public class SetupArgs
    {
        public List<Deck> SourceDecks;
    }
    
    public StandaloneCardPile DealCardPile;
    public StandaloneCardPile DiscardCardPile;
    public PackedScene CardPrefab;
    public PackedScene PiledCardPrefab;
    
    public List<Deck> SourceDecks;

    protected GameMgr GameMgr;
    protected Battle Battle;

    public override void _Ready()
    {
        base._Ready();
        GameMgr = GetNode<GameMgr>("/root/GameMgr");
        DealCardPile = GetNode<StandaloneCardPile>("DealPile");
        DiscardCardPile = GetNode<StandaloneCardPile>("DiscardPile");
        CardPrefab = ResourceCache.Instance.Load<PackedScene>("res://Scenes/Card.tscn");
        PiledCardPrefab = ResourceCache.Instance.Load<PackedScene>("res://Scenes/PiledCard.tscn");
    }

    public virtual void Setup(object o)
    {
        var args = (SetupArgs)o;
        Battle = GameMgr.CurrentBattle;
        SourceDecks = args.SourceDecks;
        DealCardPile.Setup(new StandaloneCardPile.SetupArgs
        {
            GameMgr = GameMgr,
            Battle = Battle,
            TopCardFaceDirection = Enums.CardFace.Down,
            DefaultAddCardDirection = Enums.GrowDirection.FromEnd,
        });
        DiscardCardPile.Setup(new StandaloneCardPile.SetupArgs
        {
            GameMgr = GameMgr,
            Battle = Battle,
            TopCardFaceDirection = Enums.CardFace.Up,
            DefaultAddCardDirection = Enums.GrowDirection.FromBegin,
        });
        Reset();
    }

    public void Reset()
    {
        DealCardPile.Cards.Clear();
        DiscardCardPile.Cards.Clear();
        foreach (var deck in SourceDecks)
        {
            MixIn(deck);
        }
        foreach (var card in DealCardPile.Cards)
        {
            card.Reset();
        }
        Shuffle();
    }
    
    public void MixIn(Deck deck)
    {
        foreach (var card in deck.CardList)
        {
            DealCardPile.Cards.Add(card);
        }
    }
    
    public async Task AnimateShuffle()
    {
        // GD.Print("animate shuffle");

        async Task AnimateShuffleOnce(int takeCount)
        {
            var card = DiscardCardPile.Take();
            if (card == null) return;
            await DealDedicatedCardToPile(card, DiscardCardPile, DealCardPile);
            var cards = DiscardCardPile.TakeN(takeCount);
            foreach (var takenCard in cards)
            {
                DealCardPile.Cards.Add(takenCard);
            }
            if (DiscardCardPile.Cards.Count == 0)
            {
                Shuffle();
            }
        }
        
        var shuffleAnimateCardCount = Mathf.Min(DiscardCardPile.Cards.Count, Configuration.ShuffleAnimateCards);
        var takeCountForEachAnimatedShuffleCard = DiscardCardPile.Cards.Count / shuffleAnimateCardCount;
        var tasks = new List<Task>();
        for (int i = 0; i < shuffleAnimateCardCount; i++)
        {
            tasks.Add(AnimateShuffleOnce(takeCountForEachAnimatedShuffleCard));
            await Utils.Wait(this, Configuration.AnimateCardTransformInterval);
            // await ToSignal(cardNode.TweenControl.GetTween("transform"), Tween.SignalName.Finished);
        }
        await Task.WhenAll(tasks);
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
        foreach (var card in DealCardPile.Cards.ToList())
        {
            if (card.Def.IsInnate)
            {
                if (card.Def.IsItem && Battle.ItemCardContainer.ContentNodes.Count < Battle.Player.ItemPocketSize.Value)
                {
                    DealCardPile.Cards.Remove(card);
                    tasks.Add(DealDedicatedCardIntoContainer(card, DealCardPile, Battle.ItemCardContainer));
                } else if (card.Def.IsRule)
                {
                    DealCardPile.Cards.Remove(card);
                    tasks.Add(DealDedicatedCardIntoContainer(card, DealCardPile, Battle.RuleCardContainer));
                }
            }
        }
        await Task.WhenAll(tasks);
    }

    public async Task CreateCardAndPutInto(BaseCard card, CardNode onCardNode, Enums.CardFace faceDirection, CardContainer targetContainer)
    {
        var cardNode = CreateCardNodeOnCardNode(card, onCardNode, faceDirection);
        targetContainer.ContentNodes.Add(cardNode);
        await cardNode.TweenControl.WaitTransformComplete();
    }
    
    public async Task DrawCardIntoContainer(CardContainer targetContainer, int index = -1, bool collectUsable = true)
    {
        var cardNode = await AnimateDrawCard();
        Battle.OnDealCard?.Invoke(cardNode);
        await DealDedicatedCardNodeIntoContainer(cardNode, targetContainer, index, collectUsable);
    }

    public async Task DealDedicatedCardIntoContainer(BaseCard card, BaseCardPile fromPile, CardContainer targetContainer, int index = -1,
        bool collectUsable = true)
    {
        fromPile.Cards.Remove(card);
        var cardNode = fromPile.CreateCardNodeOnPile(card);
        if (fromPile == DealCardPile)
        {
            Battle.OnDealCard?.Invoke(fromPile.CreateCardNodeOnPile(card));
        }
        await DealDedicatedCardNodeIntoContainer(cardNode, targetContainer, index, collectUsable);
    }
    
    public async Task DealDedicatedCardNodeIntoContainer(CardNode node, CardContainer targetContainer, int index = -1,
        bool collectUsable = true)
    {
        if (!Battle.IsFieldContainer((CardContainer)node.CurrentContainer.Value) && Battle.IsFieldContainer(targetContainer))
        {
            node.Card.OnEnterField();
        }
        if (index >= 0)
        {
            targetContainer.ContentNodes.Insert(index, node);
        }
        else
        {
            targetContainer.ContentNodes.Add(node);
        }
        
        await node.TweenControl.WaitTransformComplete();
        // node.PrintPosition = false;
        
        var shouldReDeal = false;
        var reDealIndex = 0;
        if (collectUsable && targetContainer.ShouldCollectDealtItemAndRuleCards)
        {
            (shouldReDeal, reDealIndex) = await CheckReDeal(node, targetContainer);
        }
        if (shouldReDeal)
        {
            await DrawCardIntoContainer(targetContainer, reDealIndex, collectUsable);
        }
    }

    public async Task DrawCardAndReplace(CardNode node, bool collectUsable = true)
    {
        var cardNode = await AnimateDrawCard();
        node.Card.OnEnterField();
        Battle.OnDealCard?.Invoke(cardNode);
        await DealDedicatedCardNodeAndReplace(cardNode, node, collectUsable);
    }

    public async Task DealDedicatedCardAndReplace(BaseCard card, BaseCardPile fromPile, CardNode targetNode, bool collectUsable = true)
    {
        if (!fromPile.Cards.Contains(card)) return;
        fromPile.Cards.Remove(card);
        var cardNode = fromPile.CreateCardNodeOnPile(card);
        if (fromPile == DealCardPile)
        {
            Battle.OnDealCard?.Invoke(cardNode);
        }
        await DealDedicatedCardNodeAndReplace(cardNode, targetNode, collectUsable);
    }

    public async Task DealDedicatedCardNodeAndReplace(CardNode node, CardNode replacedNode, bool collectUsable = true)
    {
        if (!Battle.IsFieldContainer((CardContainer)node.CurrentContainer.Value) && Battle.IsFieldContainer((CardContainer)replacedNode.CurrentContainer.Value))
        {
            node.Card.OnEnterField();
        }
        if (replacedNode.CurrentContainer.Value != null)
        {
            var cardContainer = (CardContainer)replacedNode.CurrentContainer.Value; 
            var index = cardContainer.ContentNodes.IndexOf(replacedNode);
            await replacedNode.AnimateLeaveField();
            cardContainer.ContentNodes.Insert(index, node);
            await node.TweenControl.WaitTransformComplete();
            var shouldReDeal = false;
            var reDealIndex = -1;
            if (collectUsable && cardContainer.ShouldCollectDealtItemAndRuleCards)
            {
                (shouldReDeal, reDealIndex) = await CheckReDeal(node, cardContainer);
            }
            if (shouldReDeal)
            {
                await DrawCardIntoContainer(cardContainer, reDealIndex, collectUsable);
            }
        }
        else
        {
            var parent = replacedNode.GetParent();
            var targetPosition = replacedNode.Position;
            var targetRotation = replacedNode.RotationDegrees;
            await replacedNode.AnimateLeaveField();
            node.Reparent(parent);
            await node.AnimateTransform(targetPosition, targetRotation, 
                Configuration.AnimateCardTransformInterval,
                conflictTweenAction:TweenControl.ConflictTweenAction.InterruptContinue);
        }
    }

    // Note: By default, card dealt into a pile does not redeal
    public async Task DrawCardToPile(BaseCardPile pile)
    {
        if (pile == DealCardPile) return;
        var cardNode = await AnimateDrawCard();
        if (pile != DiscardCardPile)
        {
            Battle.OnDealCard?.Invoke(cardNode);
        }
        await DealDedicatedCardNodeToPile(cardNode, pile);
    }
    
    public async Task DealDedicatedCardToPile(BaseCard card, BaseCardPile fromPile, BaseCardPile pile)
    {
        if (!fromPile.Cards.Contains(card)) return;
        fromPile.Cards.Remove(card);
        var cardNode = fromPile.CreateCardNodeOnPile(card);
        if (fromPile == DealCardPile && pile != DiscardCardPile)
        {
            Battle.OnDealCard?.Invoke(cardNode);
        }
        await DealDedicatedCardNodeToPile(cardNode, pile);
    }
    
    public async Task DealDedicatedCardNodeToPile(CardNode node, BaseCardPile pile)
    {
        if (node.CurrentContainer.Value != null)
        {
            node.CurrentContainer.Value.ContentNodes.Remove(node);
        }
        if (!Battle.IsFieldContainer((CardContainer)node.CurrentContainer.Value) && pile is AttachedCardPile)
        {
            node.Card.OnEnterField();
        }
        node.Reparent(pile);
        // node.TweenControl.StopAll();
        await node.AnimateTransform(pile.TopCardNode.Position, pile.TopCardNode.RotationDegrees, 
            Configuration.AnimateCardTransformInterval, priority: Configuration.CardMoveTweenPriority);
        pile.AddCard(node.Card);
        node.QueueFree();
    }

    public async Task<CardNode> AnimateDrawCard()
    {
        var card = DealCardPile.Take();
        if (card == null)
        {
            await AnimateShuffle();
            card = DealCardPile.Take();
            if (card == null)
            {
                GD.Print("No more cards to deal");
                return null;
            }
        }
        var cardNode = DealCardPile.CreateCardNodeOnPile(card);
        return cardNode;
    }

    public async Task AnimateDiscard(CardNode node)
    {
        GD.Print($"animate discard {node}, time:  {Time.GetTicksMsec()}");
        await DealDedicatedCardNodeToPile(node, DiscardCardPile);
        Discard(node);
    }

    protected void Discard(CardNode cardNode)
    {
        // GD.Print($"discard {cardNode}");
        DiscardCardPile.Cards.Insert(0, cardNode.Card);
        cardNode.QueueFree();
    }

    protected CardNode CreateCardNodeOnCardNode(BaseCard card, CardNode onCardNode, Enums.CardFace faceDirection)
    {
        var cardNode = Battle.InstantiateCardNode(card, onCardNode.GetParent());
        cardNode.Setup(new CardNode.SetupArgs()
        {
            Content = card,
            FaceDirection = faceDirection,
            HasPhysics = true,
        });
        card.Setup(new BaseCard.SetupArgs()
        {
            GameMgr = GameMgr,
            Battle = Battle,
            Node = cardNode
        });
        cardNode.Position = onCardNode.Position;
        return cardNode;
    }

    protected async Task<(bool, int)> CheckReDeal(CardNode node, CardContainer container)
    {
        var shouldReDeal = false;
        var reDealIndex = -1;
        if (node.Content.Value is BaseCard { Def.IsRule: true })
        {
            shouldReDeal = true;
            reDealIndex = container.ContentNodes.IndexOf(node);
            await container.MoveCardNodeToContainer(node, Battle.RuleCardContainer);
        } else if (node.Content.Value is BaseCard { Def.IsItem: true })
        {
            shouldReDeal = true;
            reDealIndex = container.ContentNodes.IndexOf(node);
            await container.MoveCardNodeToContainer(node, Battle.ItemCardContainer);
        }
        return (shouldReDeal, reDealIndex);
    }
}