using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Godot;
using XCardGame.Scripts.Cards;
using XCardGame.Scripts.Cards.AbilityCards;
using XCardGame.Scripts.Cards.SkillCards;
using XCardGame.Scripts.Common;
using XCardGame.Scripts.Common.Constants;
using XCardGame.Scripts.Common.DataBinding;
using XCardGame.Scripts.Defs;
using XCardGame.Scripts.InputHandling;
using XCardGame.Scripts.Nodes;

namespace XCardGame.Scripts.GameLogic;

public partial class SelectRewardCard: Control
{
    public class SelectRewardCardInputHandler : BaseInputHandler
    {
        protected Battle Battle;
        protected SelectRewardCard SelectRewardCard;

        public SelectRewardCardInputHandler(GameMgr gameMgr, SelectRewardCard selectRewardCard) : base(gameMgr)
        {
            SelectRewardCard = selectRewardCard;
        }
        
        public override void OnEnter()
        {
            base.OnEnter();
            Battle = GameMgr.CurrentBattle;
            foreach (var cardNode in SelectRewardCard.CardContainer.ContentNodes)
            {
                cardNode.OnPressed += OnCardNodePressed;
            }
        }
        
        public override void OnExit()
        {
            base.OnExit();
            foreach (var cardNode in SelectRewardCard.CardContainer.ContentNodes)
            {
                cardNode.OnPressed -= OnCardNodePressed;
            }
        }

        public async void OnCardNodePressed(CardNode node)
        {
            await SelectRewardCard.Select(node);
            GameMgr.InputMgr.QuitCurrentInputHandler();
        }
    }
    
    public PackedScene CardPrefab;
    public GameMgr GameMgr;
    public Battle Battle;
    public TextureButton CloseButton;
    public TextureButton ReRollButton;
    public Label ReRollPriceLabel;
    public CardContainer CardContainer;
    public AnimationPlayer AnimationPlayer;
    
    public List<BaseCardDef> AllCardDefs;
    
    public Type RewardCardType;
    public ObservableProperty<int> ReRollPrice;
    public int ReRollPriceIncrease;
    public ObservableCollection<BaseCard> RewardCards;
    public Action CardSelected;
    
    public bool HasSetup { get; set; }

    public override void _Ready()
    {
        base._Ready();
        GameMgr = GetNode<GameMgr>("/root/GameMgr");
        CloseButton = GetNode<TextureButton>("Panel/Close");
        ReRollButton = GetNode<TextureButton>("Panel/ReRoll");
        ReRollPriceLabel = GetNode<Label>("Panel/ReRoll/Price");
        CardContainer = GetNode<CardContainer>("Panel/CardContainer");
        CardPrefab = ResourceCache.Instance.Load<PackedScene>("res://Scenes/Card.tscn");
        AnimationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
        ReRollPrice = new ObservableProperty<int>(nameof(ReRollPrice), this, 0);
        ReRollPrice.DetailedValueChanged += OnReRollPriceChanged;
        RewardCards = new ObservableCollection<BaseCard>();
        ReRollButton.Pressed += ReRoll;
        GameMgr.InputMgr.SwitchToInputHandler(new SelectRewardCardInputHandler(GameMgr, this));
    }

    public void Setup(Dictionary<string, object> args)
    {
        Battle = GameMgr.CurrentBattle;
        var rewardCardCount = (int)args["rewardCardCount"];
        RewardCardType = (Type)args["rewardCardType"];
        CardContainer.Setup(new Dictionary<string, object>()
        {
            { "allowInteract", true },
            { "cards", RewardCards },
            { "contentNodeSize", Configuration.CardSize },
            { "separation", Configuration.CardContainerSeparation },
            { "pivotDirection", Enums.Direction2D8Ways.Left },
            { "nodesPerRow", rewardCardCount },
            { "hasBorder", true },
            { "expectedContentNodeCount", Configuration.ShopPokerCardCount },
            { "hasName", true },
            { "containerName", "Select a card..."},
            { "defaultCardFaceDirection", Enums.CardFace.Up },
            { "getCardFaceDirectionFunc", null },
        });
        AllCardDefs = Defs.Cards.All();
        ReRollPriceIncrease = (int)args["reRollPriceIncrease"];
        ReRoll();
    }

    public void EnsureSetup()
    {
        if (!HasSetup)
        {
            GD.PrintErr($"{this} not setup yet");
        }
    }

    public void ReRoll()
    {
        if (Battle.Player.Credit.Value >= ReRollPrice.Value)
        {
            var rarity = RandRarity();
            var cardDefs = FilterCardDefs(rarity);
            int i = 0;
            foreach (var cardDef in RandNCardDefsOfType(cardDefs, Configuration.ShopAbilityCardCount, RewardCardType))
            {
                var card = new BaseAbilityCard((AbilityCardDef)cardDef);
                if (i < RewardCards.Count)
                {
                    RewardCards[i] = card;
                }
                else
                {
                    RewardCards.Add(card);
                }
            }

            Battle.Player.Credit.Value -= ReRollPrice.Value;
            ReRollPrice.Value += ReRollPriceIncrease;
        }
    }

    public async Task Select(CardNode cardNode)
    {
        // cardNode.AnimateFlip(Enums.CardFace.Down);
        // cardNode.IsBought.Value = true;
        var card = cardNode.Content.Value;
        card.OwnerEntity = Battle.Player;
        if (card is BaseAbilityCard)
        {
            Battle.Player.AbilityCards.Add(card);
        } else if (card is BaseSkillCard)
        {
            Battle.Player.SkillCards.Add(card);
        }
        AnimationPlayer.Play("close");
        await ToSignal(AnimationPlayer, AnimationMixer.SignalName.AnimationFinished);
        CardSelected?.Invoke();
        GameMgr.QuitCurrentScene();
    }

    protected int RandRarity()
    {
        var progress = GameMgr.ProgressCounter.Value;
        var thresholds = Thresholds.RarityThresholdAtProgress[progress];
        return Utils.RandOnThresholds(thresholds, GameMgr.Rand);
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

    protected void OnReRollPriceChanged(object sender, ValueChangedEventDetailedArgs<int> args)
    {
        ReRollPriceLabel.Text = ReRollPrice.Value.ToString();
    }
}