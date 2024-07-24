using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot;
using XCardGame.Common;
using XCardGame.Ui;

namespace XCardGame;

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
    }

    public virtual void Setup(object o)
    {
        var args = (SetupArgs)o;
        Battle = GameMgr.CurrentBattle;
        SourceDecks = args.SourceDecks;
        DealCardPile.Setup(new CardPile.SetupArgs
        {
            TopCardFaceDirection = Enums.CardFace.Down,
        });
        DiscardCardPile.Setup(new CardPile.SetupArgs
        {
            TopCardFaceDirection = Enums.CardFace.Up,
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
        foreach (var card in deck.CardList)
        {
            DealCardPile.Cards.Add(card);
        }
    }
    

    public async Task AnimateShuffle()
    {
        // GD.Print("animate shuffle");
        var shuffleAnimateCardCount = Mathf.Min(DiscardCardPile.Cards.Count, Configuration.ShuffleAnimateCards);
        var takeCountForEachAnimatedShuffleCard = DiscardCardPile.Cards.Count / shuffleAnimateCardCount;
        var tasks = new List<Task>();
        for (int i = 0; i < shuffleAnimateCardCount; i++)
        {
            var card = await DrawCardFromPile(DiscardCardPile);
            if (card == null) return;
            var cardNode = CreateCardNodeOnPile(card, DiscardCardPile);
            cardNode.Reparent(DealCardPile);
            tasks.Add(cardNode.AnimateTransform(DealCardPile.TopCard.Position, DealCardPile.TopCard.RotationDegrees, 
                Configuration.AnimateCardTransformInterval, Configuration.CardMoveTweenPriority, () =>
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
                }));
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
            if (card is BaseInteractCard interactCard && ((InteractCardDef)interactCard.Def).IsInnate)
            {
                DealCardPile.Cards.Remove(card);
                var cardNode = CreateCardNodeOnPile(card, DealCardPile);
                Battle.OnDealCard?.Invoke(Battle, cardNode);
                if (interactCard is BaseRuleCard)
                {
                    Battle.RuleCardContainer.ContentNodes.Add(cardNode);
                    tasks.Add(cardNode.TweenControl.WaitTransformComplete());
                } else if (interactCard is BaseItemCard && Battle.ItemCardContainer.ContentNodes.Count < Battle.Player.ItemPocketSize.Value)
                {
                    Battle.ItemCardContainer.ContentNodes.Add(cardNode);
                    tasks.Add(cardNode.TweenControl.WaitTransformComplete());
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
    
    public async Task DealCardIntoContainer(CardContainer targetContainer, bool collectItem = true)
    {
        var cardNode = await AnimateDrawCard();
        // if (collectItem && targetContainer.ShouldCollectDealtItemAndRuleCards)
        // {
        //     if (cardNode.Content.Value is BaseRuleCard)
        //     {
        //         shouldReDeal = true;
        //         Battle.RuleCardContainer.ContentNodes.Add(cardNode);
        //     } else if (cardNode.Content.Value is BaseItemCard)
        //     {
        //         shouldReDeal = true;
        //         Battle.ItemCardContainer.ContentNodes.Add(cardNode);
        //     }
        //     else
        //     {
        //         targetContainer.ContentNodes.Add(cardNode);
        //     }
        // }
        // else
        // {
        //     targetContainer.ContentNodes.Add(cardNode);
        // }
        targetContainer.ContentNodes.Add(cardNode);
        await cardNode.TweenControl.WaitTransformComplete();
        var shouldReDeal = false;
        if (collectItem && targetContainer.ShouldCollectDealtItemAndRuleCards)
        {
            if (cardNode.Content.Value is BaseRuleCard)
            {
                shouldReDeal = true;
                await targetContainer.MoveCardNodeToContainer(cardNode, Battle.RuleCardContainer);
            } else if (cardNode.Content.Value is BaseItemCard)
            {
                shouldReDeal = true;
                await targetContainer.MoveCardNodeToContainer(cardNode, Battle.ItemCardContainer);
            }
        }
        if (shouldReDeal)
        {
            await DealCardIntoContainer(targetContainer, collectItem);
        }
    }

    public async Task DealCardAndReplace(CardNode node, bool collectItem = true)
    {
        var cardNode = await AnimateDrawCard();
        if (node.CurrentContainer.Value != null)
        {
            var cardContainer = (CardContainer)node.CurrentContainer.Value; 
            var index = cardContainer.ContentNodes.IndexOf(node);
            await node.AnimateLeaveBattle();
            cardContainer.ContentNodes.Insert(index, cardNode);
            await cardNode.TweenControl.WaitTransformComplete();
            var card = (BaseCard)cardNode.Content.Value;
            card?.OnDealt?.Invoke(card);
            var shouldReDeal = false;
            if (collectItem && cardContainer.ShouldCollectDealtItemAndRuleCards)
            {
                if (cardNode.Content.Value is BaseRuleCard)
                {
                    shouldReDeal = true;
                    await cardContainer.MoveCardNodeToContainer(cardNode, Battle.RuleCardContainer);
                } else if (cardNode.Content.Value is BaseItemCard)
                {
                    shouldReDeal = true;
                    await cardContainer.MoveCardNodeToContainer(cardNode, Battle.ItemCardContainer);
                }
            }
            if (shouldReDeal)
            {
                await DealCardIntoContainer(cardContainer, collectItem);
            }
        }
        else
        {
            var parent = node.GetParent();
            await node.AnimateLeaveBattle();
            cardNode.Reparent(parent);
            await cardNode.AnimateTransform(node.Position, node.RotationDegrees, 
                Configuration.AnimateCardTransformInterval,
                conflictTweenAction:TweenControl.ConflictTweenAction.InterruptContinue);
        }
        
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
        // GD.Print($"animate discard {node}, time:  {Time.GetTicksMsec()}");
        if (node.CurrentContainer != null)
        {
            node.CurrentContainer.Value.ContentNodes.Remove(node);
        }
        node.Reparent(DiscardCardPile);
        await node.AnimateTransform(DiscardCardPile.TopCard.Position, DiscardCardPile.TopCard.RotationDegrees, 
            Configuration.AnimateCardTransformInterval, priority: Configuration.CardMoveTweenPriority);
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
        // GD.Print($"discard {cardNode}");
        DiscardCardPile.Cards.Insert(0, cardNode.Card);
        var card = (BaseCard)cardNode.Content.Value;
        card?.OnDiscard?.Invoke(card);
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
        card.Setup(new BaseCard.SetupArgs()
        {
            GameMgr = GameMgr,
            Battle = Battle,
            Node = cardNode
        });
        cardNode.Position = onCardNode.Position;
        return cardNode;
    }
}