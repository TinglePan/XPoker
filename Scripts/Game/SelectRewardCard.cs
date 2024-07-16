﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Godot;
using XCardGame.Scripts.Cards;
using XCardGame.Scripts.Common;
using XCardGame.Scripts.Common.Constants;
using XCardGame.Scripts.Common.DataBinding;
using XCardGame.Scripts.Defs;
using XCardGame.Scripts.Defs.Def.Card;
using XCardGame.Scripts.Defs.Tables;
using XCardGame.Scripts.InputHandling;
using XCardGame.Scripts.Ui;

namespace XCardGame.Scripts.Game;

public partial class SelectRewardCard: Control
{
    public class SelectRewardCardInputHandler : BaseInputHandler
    {
        protected Battle Battle;
        protected SelectRewardCard SelectRewardCard;

        public SelectRewardCardInputHandler(GameMgr gameMgr, SelectRewardCard selectRewardCard) : base(gameMgr)
        {
            SelectRewardCard = selectRewardCard;
            SelectRewardCard.ReRollButton.Pressed += SelectRewardCard.ReRoll;
            SelectRewardCard.SkipButton.Pressed += SelectRewardCard.Skip;
        }
        
        public override void OnEnter()
        {
            base.OnEnter();
            Battle = GameMgr.CurrentBattle;
            foreach (var cardNode in SelectRewardCard.CardContainer.ContentNodes)
            {
                cardNode.OnMousePressed += OnCardNodePressed;
            }
        }
        
        public override void OnExit()
        {
            base.OnExit();
            foreach (var cardNode in SelectRewardCard.CardContainer.ContentNodes)
            {
                cardNode.OnMousePressed -= OnCardNodePressed;
            }
        }

        public async void OnCardNodePressed(BaseContentNode node, MouseButton mouseButton)
        {
            if (mouseButton == MouseButton.Left)
            {
                var cardNode = (CardNode)node;
                await SelectRewardCard.Select(cardNode);
                SelectRewardCard.Quit();
            }
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
    public Action OnQuit;
    
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
        SkipReward = new ObservableProperty<int>(nameof(SkipReward), this, 0);
        SkipReward.DetailedValueChanged += OnSkipRewardChanged;
        RewardCardDefPool = new Dictionary<int, List<BaseCardDef>>();
    }

    public void Setup(Dictionary<string, object> args)
    {
        Battle = GameMgr.CurrentBattle;
        var rewardCardCount = (int)args["rewardCardCount"];
        if (args.TryGetValue("rewardCardDefType", out var arg))
        {
            RewardCardDefType = (Type)arg;
        }
        CardContainer.Setup(new CardContainer.SetupArgs
        {
            ContentNodeSize = Configuration.CardSize,
            Separation = Configuration.CardContainerSeparation,
            PivotDirection = Enums.Direction2D8Ways.Neutral,
            HasName = true,
            ContainerName = Utils._("Select a card..."),
            DefaultCardFaceDirection = Enums.CardFace.Up,
            Margins = Configuration.DefaultContentContainerMargins,
            OnlyDisplay = true,
        });
        AllRewardCardDefs = FilterCardDefs(CardDefs.All(), x => x.GetType().IsAssignableTo(typeof(InteractCardDef)));
        
        if (RewardCardDefType != null)
        {
            AllRewardCardDefs = FilterCardDefs(AllRewardCardDefs, x => x.GetType() == RewardCardDefType);
        }
        
        RewardCardDefPool = new Dictionary<int, List<BaseCardDef>>();
        foreach (var cardDef in AllRewardCardDefs)
        {
            RewardCardDefPool.TryAdd(cardDef.Rarity, new List<BaseCardDef>());
            RewardCardDefPool[cardDef.Rarity].Add(cardDef);
        }
        ReRollPrice.Value = (int)args["reRollPrice"];
        ReRollPriceIncrease = (int)args["reRollPriceIncrease"];
        SkipReward.Value = (int)args["skipReward"];
        if (args.TryGetValue("defs", out var value))
        {
            int index = 0;
            foreach (var cardDef in (List<BaseCardDef>)value)
            {
                var card = CardFactory.CreateInstance(cardDef.ConcreteClassPath, cardDef);
                if (index < CardContainer.Contents.Count)
                {
                    CardContainer.Contents[index] = card;
                }
                else
                {
                    CardContainer.Contents.Add(card);
                }
                index++;
            }
        }
        else
        {
            RandRewardCards();
        }
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
        Quit();
    }

    public void ReRoll()
    {
        if (Battle.Player.Credit.Value >= ReRollPrice.Value)
        {
            RandRewardCards();
            Battle.Player.Credit.Value -= ReRollPrice.Value;
            ReRollPrice.Value += ReRollPriceIncrease;
        }
    }

    public async Task Select(CardNode cardNode)
    {
        // cardNode.AnimateFlip(Enums.CardFace.Down);
        // cardNode.IsBought.Value = true;
        var card = cardNode.Card;
        card.OwnerEntity = Battle.Player;
        // AnimationPlayer.Play("close");
        // await ToSignal(AnimationPlayer, AnimationMixer.SignalName.AnimationFinished);
    }

    public void Quit()
    {
        OnQuit?.Invoke();
        GameMgr.InputMgr.QuitCurrentInputHandler();
        GameMgr.QuitCurrentScene();
    }

    protected void RandRewardCards()
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
                    if (index < CardContainer.Contents.Count)
                    {
                        CardContainer.Contents[index] = card;
                    }
                    else
                    {
                        CardContainer.Contents.Add(card);
                    }
                    index++;
                }
            }
        }
        foreach (var cardDef in Utils.RandMFrom(AllRewardCardDefs, Configuration.DefaultRewardCardCount - index, GameMgr.Rand))
        {
            var card = CardFactory.CreateInstance(cardDef.ConcreteClassPath, cardDef);
            if (index < CardContainer.Contents.Count)
            {
                CardContainer.Contents[index] = card;
            }
            else
            {
                CardContainer.Contents.Add(card);
            }

            index++;
        }
    }

    protected int RandRarity()
    {
        var progress = GameMgr.ProgressCounter.Value;
        var odds = RarityOddByProgress.Content[progress];
        return Utils.RandOnOdds(odds, GameMgr.Rand);
    }

    protected List<BaseCardDef> FilterCardDefs(List<BaseCardDef> cardDefs, Func<BaseCardDef, bool> filterFunc)
    {
        return cardDefs.FindAll(new Predicate<BaseCardDef>(filterFunc)).ToList();
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