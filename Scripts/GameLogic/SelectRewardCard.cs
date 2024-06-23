using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
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
            GameMgr.QuitCurrentScene();
        }
    }
    
    public PackedScene CardPrefab;
    public GameMgr GameMgr;
    public Battle Battle;
    public BaseButton SkipButton;
    public Label SkipRewardLabel;
    public BaseButton ReRollButton;
    public Label ReRollPriceLabel;
    public CardContainer CardContainer;
    public AnimationPlayer AnimationPlayer;
    
    public List<BaseCardDef> AllRewardCardDefs;
    public Dictionary<int, List<BaseCardDef>> RewardCardDefPool;
    
    public Type RewardCardDefType;
    public ObservableProperty<int> ReRollPrice;
    public int ReRollPriceIncrease;
    public ObservableProperty<int> SkipReward;
    public ObservableCollection<BaseCard> RewardCards;
    public Action CardSelected;
    
    public bool HasSetup { get; set; }

    public override void _Ready()
    {
        base._Ready();
        GameMgr = GetNode<GameMgr>("/root/GameMgr");
        SkipButton = GetNode<BaseButton>("Skip");
        SkipRewardLabel = GetNode<Label>("Skip/Price");
        
        ReRollButton = GetNode<BaseButton>("ReRoll");
        ReRollPriceLabel = GetNode<Label>("ReRoll/Price");
        CardContainer = GetNode<CardContainer>("CardsAnchor/Cards");
        CardPrefab = ResourceCache.Instance.Load<PackedScene>("res://Scenes/Card.tscn");
        AnimationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
        ReRollPrice = new ObservableProperty<int>(nameof(ReRollPrice), this, 0);
        ReRollPrice.DetailedValueChanged += OnReRollPriceChanged;
        ReRollButton.Pressed += ReRoll;
        SkipReward = new ObservableProperty<int>(nameof(SkipReward), this, 0);
        SkipReward.DetailedValueChanged += OnSkipRewardChanged;
        SkipButton.Pressed += Skip;
        RewardCards = new ObservableCollection<BaseCard>();
        RewardCardDefPool = new Dictionary<int, List<BaseCardDef>>();
    }

    public void Setup(Dictionary<string, object> args)
    {
        Battle = GameMgr.CurrentBattle;
        var rewardCardCount = (int)args["rewardCardCount"];
        RewardCardDefType = (Type)args["rewardCardDefType"];
        CardContainer.Setup(new Dictionary<string, object>()
        {
            { "allowInteract", true },
            { "cards", RewardCards },
            { "contentNodeSize", Configuration.CardSize },
            { "separation", Configuration.CardContainerSeparation },
            { "pivotDirection", Enums.Direction2D8Ways.Neutral },
            { "nodesPerRow", rewardCardCount },
            { "hasBorder", true },
            { "expectedContentNodeCount", Configuration.ShopPokerCardCount },
            { "hasName", true },
            { "containerName", "Select a card..."},
            { "defaultCardFaceDirection", Enums.CardFace.Up },
            { "getCardFaceDirectionFunc", null },
        });
        AllRewardCardDefs = FilterCardDefs(CardDefs.All(), RewardCardDefType);
        RewardCardDefPool = new Dictionary<int, List<BaseCardDef>>();
        foreach (var cardDef in AllRewardCardDefs)
        {
            RewardCardDefPool.TryAdd(cardDef.Rarity, new List<BaseCardDef>());
            RewardCardDefPool[cardDef.Rarity].Add(cardDef);
        }
        ReRollPriceIncrease = (int)args["reRollPriceIncrease"];
        SkipReward.Value = (int)args["skipReward"];
        ReRoll();
        GameMgr.InputMgr.SwitchToInputHandler(new SelectRewardCardInputHandler(GameMgr, this));
    }

    public void EnsureSetup()
    {
        if (!HasSetup)
        {
            GD.PrintErr($"{this} not setup yet");
        }
    }

    public void Skip()
    {
        Battle.Player.Credit.Value += SkipReward.Value;
        GameMgr.InputMgr.QuitCurrentInputHandler();
        GameMgr.QuitCurrentScene();
    }

    public void ReRoll()
    {
        if (Battle.Player.Credit.Value >= ReRollPrice.Value)
        {
            var rarities = new Dictionary<int, int>();
            for (int i = 0; i < Configuration.DefaultRewardCardCount; i++)
            {
                var rarity = RandRarity();
                rarities.TryAdd(rarity, 0);
                rarities[rarity]++;
            }

            var index = 0;
            foreach (var (rarity, count) in rarities)
            {
                if (RewardCardDefPool.TryGetValue(rarity, out var value))
                {
                    foreach (var cardDef in Utils.RandMFrom(value, count, GameMgr.Rand))
                    {
                        var card = CardFactory.CreateInstance(cardDef.ConcreteClassPath, cardDef);
                        if (index < RewardCards.Count)
                        {
                            RewardCards[index] = card;
                        }
                        else
                        {
                            RewardCards.Add(card);
                        }
                        index++;
                    }
                }
            }
            foreach (var cardDef in Utils.RandMFrom(AllRewardCardDefs, Configuration.DefaultRewardCardCount - index, GameMgr.Rand))
            {
                var card = CardFactory.CreateInstance(cardDef.ConcreteClassPath, cardDef);
                if (index < RewardCards.Count)
                {
                    RewardCards[index] = card;
                }
                else
                {
                    RewardCards.Add(card);
                }

                index++;
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
        // AnimationPlayer.Play("close");
        // await ToSignal(AnimationPlayer, AnimationMixer.SignalName.AnimationFinished);
        CardSelected?.Invoke();
    }

    protected int RandRarity()
    {
        var progress = GameMgr.ProgressCounter.Value;
        var thresholds = Thresholds.RarityThresholdAtProgress[progress];
        return Utils.RandOnThresholds(thresholds, GameMgr.Rand);
    }

    protected List<BaseCardDef> FilterCardDefs(List<BaseCardDef> cardDefs, int rarity)
    {
        return cardDefs.FindAll(x => rarity == x.Rarity).ToList();
    }
    
    protected List<BaseCardDef> FilterCardDefs(List<BaseCardDef> cardDefs, Type type)
    {
        return cardDefs.FindAll(x => x.GetType().IsAssignableTo(type)).ToList();
    }
    
    protected List<BaseCardDef> FilterCardDefs(List<BaseCardDef> cardDefs, Type type, int rarity)
    {
        return cardDefs.FindAll(x => x.GetType() == type && rarity == x.Rarity).ToList();
    }
    
    protected void OnReRollPriceChanged(object sender, ValueChangedEventDetailedArgs<int> args)
    {
        if (args.NewValue <= Battle.Player.Credit.Value)
        {
            ReRollPriceLabel.Text = $"-{args.NewValue}";
        }
        else
        {
            ReRollPriceLabel.Text = "Not enough credit";
        }
    }

    protected void OnSkipRewardChanged(object sender, ValueChangedEventDetailedArgs<int> args)
    {
        SkipRewardLabel.Text = $"+{args.NewValue}";
    }
}