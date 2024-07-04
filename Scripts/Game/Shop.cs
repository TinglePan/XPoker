using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Godot;
using XCardGame.Scripts.Cards;
using XCardGame.Scripts.Cards.CardMarkers;
using XCardGame.Scripts.Common;
using XCardGame.Scripts.Common.Constants;
using XCardGame.Scripts.Defs;
using XCardGame.Scripts.Defs.Def.Card;
using XCardGame.Scripts.Defs.Tables;
using XCardGame.Scripts.InputHandling;
using XCardGame.Scripts.Ui;

namespace XCardGame.Scripts.Game;


public partial class Shop: Control, ISetup
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
                    cardNode.OnPressed += OnCardNodePressed;
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
                    cardNode.OnPressed -= OnCardNodePressed;
                }
            }
        }

        public void OnCardNodePressed(BaseContentNode<BaseCard> node)
        {
            var cardNode = (CardNode)node;
            if (Player.CanBuy(cardNode))
            {
                var card = node.Content.Value;
                Player.Credit.Value -= card.Def.BasePrice;
                Shop.AnimateBought(cardNode);
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
    
    public ObservableCollection<BaseCard> PokerCards;
    public ObservableCollection<BaseCard> SkillCards;
    public ObservableCollection<BaseCard> AbilityCards;
    public ObservableCollection<BaseCardMarker> Markers;

    public List<BaseCardDef> AllCardDefs; 
    
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
        PokerCards = new ObservableCollection<BaseCard>();
        SkillCards = new ObservableCollection<BaseCard>();
        AbilityCards = new ObservableCollection<BaseCard>();
        Markers = new ObservableCollection<BaseCardMarker>();
    }

    public void Setup(Dictionary<string, object> args)
    {
        Battle = GameMgr.CurrentBattle;
        PokerCardContainer.Setup(new Dictionary<string, object>()
        {
            { "allowInteract", false },
            { "cards", PokerCards },
            { "contentNodeSize", Configuration.CardSize },
            { "separation", Configuration.CardContainerSeparation },
            { "pivotDirection", Enums.Direction2D8Ways.Left },
            { "nodesPerRow", Configuration.ShopPokerCardCount },
            { "hasBorder", true },
            { "expectedContentNodeCount", Configuration.ShopPokerCardCount },
            { "hasName", true },
            { "containerName", "Community cards"},
            { "defaultCardFaceDirection", Enums.CardFace.Up },
            { "getCardFaceDirectionFunc", null },
            { "withCardEffect", false }
        });
        SkillCardContainer.Setup(new Dictionary<string, object>()
        {
            { "allowInteract", false },
            { "cards", SkillCards },
            { "contentNodeSize", Configuration.CardSize },
            { "separation", Configuration.CardContainerSeparation },
            { "pivotDirection", Enums.Direction2D8Ways.Left },
            { "nodesPerRow", Configuration.ShopSkillCardCount },
            { "hasBorder", true },
            { "expectedContentNodeCount", Configuration.ShopSkillCardCount },
            { "hasName", true },
            { "containerName", "Skill cards"},
            { "defaultCardFaceDirection", Enums.CardFace.Up },
            { "getCardFaceDirectionFunc", null },
            { "withCardEffect", false }
        });
        AbilityCardContainer.Setup(new Dictionary<string, object>()
        {
            { "allowInteract", false },
            { "cards", AbilityCards },
            { "contentNodeSize", Configuration.CardSize },
            { "separation", Configuration.CardContainerSeparation },
            { "pivotDirection", Enums.Direction2D8Ways.Left },
            { "nodesPerRow", Configuration.ShopAbilityCardCount },
            { "hasBorder", true },
            { "expectedContentNodeCount", Configuration.ShopAbilityCardCount },
            { "hasName", true },
            { "containerName", "Ability cards"},
            { "defaultCardFaceDirection", Enums.CardFace.Up },
            { "getCardFaceDirectionFunc", null },
            { "withCardEffect", false }
        });
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
        cardNode.IsBought.Value = true;
        var card = cardNode.Content.Value;
        // if (cardNode.Container.Value == AbilityCardContainer)
        // {
        //     Battle.Player.AbilityCards.Add(card);
        // } else if (cardNode.Container.Value == SkillCardContainer)
        // {
        //     Battle.Player.SkillCards.Add(card);
        // }
        
        // TODO: poker cards, markers unhandled.
    }

    protected int RandRarity()
    {
        var progress = GameMgr.ProgressCounter.Value;
        var odds = RarityOddByProgress.Content[progress];
        return Utils.RandOnOdds(odds, GameMgr.Rand);
    }

    protected List<BaseCardDef> FilterCardDefs(int rarity)
    {
        return AllCardDefs.FindAll(x => rarity == x.Rarity).ToList();
    }
    
    protected List<BaseCardDef> FilterCardDefs(Type type)
    {
        return AllCardDefs.FindAll(x => x.GetType() == type).ToList();
    }
    
    protected List<BaseCardDef> FilterCardDefs(Type type, int rarity)
    {
        return AllCardDefs.FindAll(x => x.GetType() == type && rarity == x.Rarity).ToList();
    }

    protected List<BaseCardDef> RandNCardDefsOfType(List<BaseCardDef> cardDefs, int n, Type type)
    {
        var filteredCardDefs = cardDefs.FindAll(x => x.GetType() == type).ToList();
        return Utils.RandMFrom(filteredCardDefs, n, GameMgr.Rand);
    }
}