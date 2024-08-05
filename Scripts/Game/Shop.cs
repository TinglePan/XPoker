using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using XCardGame.Common;
using XCardGame.Ui;

namespace XCardGame;


public partial class Shop: Control
{

    public class ShopInputHandler : BaseInputHandler
    {
        protected Battle Battle;
        protected PlayerBattleEntity Player;
        protected Shop Shop;
        public List<CardContainer> CardContainers;

        public ShopInputHandler(GameMgr gameMgr, Shop shop, List<CardContainer> cardContainers) : base(gameMgr)
        {
            CardContainers = cardContainers;
            Shop = shop;
        }
        
        public override void OnEnter()
        {
            base.OnEnter();
            Battle = GameMgr.CurrentBattle;
            Player = Battle.Player;
            foreach (var cardContainer in CardContainers)
            {
                foreach (var cardNode in cardContainer.ContentNodes)
                {
                    cardNode.OnMousePressed += OnCardNodePressed;
                }
            }
        }
        
        public override void OnExit()
        {
            base.OnExit();
            foreach (var cardContainer in CardContainers)
            {
                foreach (var cardNode in cardContainer.ContentNodes)
                {
                    cardNode.OnMousePressed -= OnCardNodePressed;
                }
            }
        }

        public void OnCardNodePressed(BaseContentNode node, MouseButton mouseButton)
        {
            if (mouseButton == MouseButton.Left)
            {
                var cardNode = (CardNode)node;
                if (Player.CanBuy(cardNode))
                {
                    var card = cardNode.Card;
                    Player.Credit.Value -= card.Def.BasePrice;
                    Shop.AnimateBought(cardNode);
                }
            }
        }
    }
    
    public PackedScene CardPrefab;
    public GameMgr GameMgr;
    public Battle Battle;
    public TextureButton CloseButton;
    public TextureButton PowerUpButton;
    public Label PowerUpPriceLabel;
    public TextureButton RemoveCardsButton;
    public Label RemoveCardsPriceLabel;
    public CardContainer PokerCardContainer;
    public CardContainer SkillCardContainer;
    public CardContainer AbilityCardContainer;
    
    // public ObservableCollection<BaseCard> PokerCards;
    // public ObservableCollection<BaseCard> SkillCards;
    // public ObservableCollection<BaseCard> AbilityCards;
    // public ObservableCollection<BaseCardMarker> Markers;

    public List<CardDef> AllCardDefs; 
    
    public bool HasSetup { get; set; }

    public override void _Ready()
    {
        base._Ready();
        GameMgr = GetNode<GameMgr>("/root/GameMgr");
        CloseButton = GetNode<TextureButton>("Panel/Close");
        PowerUpButton = GetNode<TextureButton>("Panel/PowerUp");
        PowerUpPriceLabel = GetNode<Label>("Panel/PowerUp/Price");
        RemoveCardsButton = GetNode<TextureButton>("Panel/RemoveCards");
        RemoveCardsPriceLabel = GetNode<Label>("Panel/RemoveCards/Price");
        PokerCardContainer = GetNode<CardContainer>("Panel/PokerCardContainer");
        SkillCardContainer = GetNode<CardContainer>("Panel/SkillCardContainer");
        AbilityCardContainer = GetNode<CardContainer>("Panel/AbilityCardContainer");
        CardPrefab = ResourceCache.Instance.Load<PackedScene>("res://Scenes/Card.tscn");
    }

    public void Setup(object o)
    {
        Battle = GameMgr.CurrentBattle;
        AllCardDefs = CardDefs.All();
    }

    public void EnsureSetup()
    {
        if (!HasSetup)
        {
            GD.PrintErr($"{this} not setup yet");
        }
    }

    public void Refresh()
    {
        var rarity = RandRarity();
        var cardDefs = FilterCardDefs(rarity);
        // foreach (var cardDef in RandNCardDefsOfType(cardDefs, Configuration.ShopAbilityCardCount, typeof(AbilityCardDef)))
        // {
        //     var card = new BaseInteractCard((AbilityCardDef)cardDef);
        //     AbilityCards.Add(card);
        // }
        //
        // foreach (var cardDef in RandNCardDefsOfType(cardDefs, Configuration.ShopAbilityCardCount, typeof(SkillCardDef)))
        // {
        //     var card = new BaseSkillCard((SkillCardDef)cardDef);
        //     SkillCards.Add(card);
        // }

        // TODO: Should I include poker cards in shop? What should I do with poker card decks?
    }

    public void AnimateBought(CardNode cardNode)
    {
        // cardNode.AnimateFlip(Enums.CardFace.Down);
        // cardNode.IsBought.Value = true;
        // var card = cardNode.Content.Value;
        // if (cardNode.Container.Value == AbilityCardContainer)
        // {
        //     Battle.Player.AbilityCards.Add(card);
        // } else if (cardNode.Container.Value == SkillCardContainer)
        // {
        //     Battle.Player.SkillCards.Add(card);
        // }
    }

    protected int RandRarity()
    {
        var progress = GameMgr.ProgressCounter.Value;
        var odds = RarityOddByProgress.Content[progress];
        return Utils.RandOnOdds(odds, GameMgr.Rand);
    }

    protected List<CardDef> FilterCardDefs(int rarity)
    {
        return AllCardDefs.FindAll(x => rarity == x.Rarity).ToList();
    }
    
    protected List<CardDef> FilterCardDefs(Type type)
    {
        return AllCardDefs.FindAll(x => x.GetType() == type).ToList();
    }
    
    protected List<CardDef> FilterCardDefs(Type type, int rarity)
    {
        return AllCardDefs.FindAll(x => x.GetType() == type && rarity == x.Rarity).ToList();
    }

    protected List<CardDef> RandNCardDefsOfType(List<CardDef> cardDefs, int n, Type type)
    {
        var filteredCardDefs = cardDefs.FindAll(x => x.GetType() == type).ToList();
        return Utils.RandMFrom(filteredCardDefs, n, GameMgr.Rand);
    }
}