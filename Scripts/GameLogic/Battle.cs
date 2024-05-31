using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using Godot;
using XCardGame.Scripts.Buffs;
using XCardGame.Scripts.Cards;
using XCardGame.Scripts.Common.Constants;
using XCardGame.Scripts.Effects;
using XCardGame.Scripts.HandEvaluate;
using XCardGame.Scripts.Nodes;

namespace XCardGame.Scripts.GameLogic;

public partial class Battle: ManagedNode2D, ISetup
{

    public enum State
    {
        Finished,
        BeforeDealCards,
        BeforeShowDown,
        AfterShowDown
    }
    
    [Export] public Dealer Dealer;
    [Export] public CardContainer CommunityCardContainer;
    [Export] public CardContainer FieldCardContainer;
    [Export] public Godot.Collections.Array<BattleEntity> Entities;

    public BaseButton ProceedButton;
    
    public PlayerBattleEntity Player => Entities[0] as PlayerBattleEntity;
    
    public bool HasSetup { get; set; }
    
    public Action<Battle> OnBattleProceed;
    public Action<Battle> AfterDealCards;
    public Action<Battle> OnRoundStart;
    public Action<Battle> OnRoundEnd;
    public Action<Battle> BeforeShowDown;
    public Action<Battle> BeforeEngage;
    public Action<Battle, Attack> BeforeApplyDamage;
    public Action<Battle> OnBattleFinished;

    public ObservableCollection<BaseCard> CommunityCards;
    public ObservableCollection<BaseCard> FieldCards;
    
    public CompletedHandEvaluator HandEvaluator;
    
    public int DealCommunityCardCount;
    public int FaceDownCommunityCardCount;
    public int RequiredHoleCardCountMin;
    public int RequiredHoleCardCountMax;
    
    public int RoundCount;
    public Dictionary<BattleEntity, CompletedHand> RoundHands;
    
    public List<BaseEffect> Effects;
    public State CurrentState;
    
    
    public override void _Ready()
    {
        base._Ready();
        HasSetup = false;
        RoundHands = new Dictionary<BattleEntity, CompletedHand>();
        CommunityCards = new ObservableCollection<BaseCard>();
        FieldCards = new ObservableCollection<BaseCard>();
    }

    public virtual void Setup(Dictionary<string, object> args)
    {
        DealCommunityCardCount = (int)args["dealCommunityCardCount"];
        FaceDownCommunityCardCount = (int)args["faceDownCommunityCardCount"];
        RequiredHoleCardCountMin = (int)args["requiredHoleCardCountMin"];
        RequiredHoleCardCountMax = (int)args["requiredHoleCardCountMax"];
        HandEvaluator = new CompletedHandEvaluator(Configuration.CompletedHandCardCount, 
            RequiredHoleCardCountMin, RequiredHoleCardCountMax);
        
        
        CommunityCardContainer.Setup(new Dictionary<string, object>()
        {
            { "allowInteract", false },
            { "cards", CommunityCards },
            { "contentNodeSize", Configuration.CardSize },
            { "separation", Configuration.CardContainerSeparation },
            { "getCardFaceDirectionFunc", (Func<int, Enums.CardFace>)GetCommunityCardFaceDirectionFunc }
        });
        
        FieldCardContainer.Setup(new Dictionary<string, object>()
        {
            { "allowInteract", true },
            { "cards", FieldCards },
            { "contentNodeSize", Configuration.CardSize },
            { "separation", Configuration.CardContainerSeparation },
            { "defaultCardFaceDirection", Enums.CardFace.Up } 
        });
        
        var entitiesSetupArgs = (List<Dictionary<string, object>>)args["entities"];
        for (int i = 0; i < Entities.Count; i++)
        {
            var entity = Entities[i];
            entity.Setup(entitiesSetupArgs[i]);
            entity.OnDefeated += OnEntityDefeated;
            entity.AbilityCards.CollectionChanged += OnEntityAbilityCardsChanged;
            foreach (var abilityCard in entity.AbilityCards)
            {
                FieldCards.Add(abilityCard);
            }
        }
        
        Dealer.Setup(new Dictionary<string, object>()
        {
            { "sourceDecks" , Entities.Select(e => e.Deck).ToList() },
            { "excludedCards" , null }
        });
        
        // Player.Setup((Dictionary<string, object>)args["playerSetupArgs"]);
        // var iEnemy = 0;
        // foreach (var entity in Entities)
        // {
        //     if (entity is PlayerBattleEntity) continue;
        //     entity.Setup(((List<Dictionary<string, object>>)args["enemySetupArgs"])[iEnemy]);
        //     iEnemy++;
        // }
        
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

    public void Start()
    {
        Reset();
        Dealer.Shuffle();
        NewRound();
    }

    public void Proceed()
    {
        switch (CurrentState)
        {
            case State.Finished:
                Start();
                DealCards();
                break;
            case State.BeforeDealCards:
                DealCards();
                break;
            case State.BeforeShowDown:
                ShowDown();
                break;
            case State.AfterShowDown:
                NewRound();
                break;
        }
        OnBattleProceed?.Invoke(this);
    }
    
    public void NewRound()
    {
        RoundCount++;
        OnRoundStart?.Invoke(this);
        foreach (var entity in Entities)
        {
            entity.RoundReset();
        }
        foreach (var cardNode in CommunityCardContainer.ContentNodes)
        {
            Dealer.AnimateDiscard(cardNode);
        }
        // CommunityCardContainer.ContentNodes.Clear();
        RoundHands.Clear();
        CurrentState = State.BeforeDealCards;
    }
    
    public void Reset()
    {
        RoundCount = 0;
        CurrentState = State.Finished;
        foreach (var entity in Entities)
        {
            entity.Reset();
        }
        CommunityCardContainer.ContentNodes.Clear();
        Dealer.Reset();
    }

    public void DealCards()
    {
        foreach (var entity in Entities)
        {
            for (int i = 0; i < entity.DealCardCount; i++)
            {
                Dealer.DealCardIntoContainer(entity.HoleCardContainer);
            }
        }
        for (int i = 0; i < DealCommunityCardCount; i++)
        {
            Dealer.DealCardIntoContainer(CommunityCardContainer);
        }
        AfterDealCards?.Invoke(this);
        CurrentState = State.BeforeShowDown;
    }
    
    public void ShowDown()
    {
        BeforeShowDown?.Invoke(this);
        
        // var startTime = Time.GetTicksUsec();

        foreach (var entity in Entities)
        {
            var bestHand = 
                HandEvaluator.EvaluateBestHand(CommunityCardContainer.Contents.ToList(),
                    entity.HoleCardContainer.Contents.ToList());
            RoundHands.Add(entity, bestHand);
        }

        foreach (var entity in Entities)
        {
            foreach (var card in entity.HoleCardContainer.Contents)
            {
                // card.Node.TweenFlip(Enums.CardFace.Up, Configuration.FlipTweenTime);
                card.Node.AnimateFlip(Enums.CardFace.Up);
            }
        }
        foreach (var card in CommunityCardContainer.Contents)
        {
            // card.Node.TweenFlip(Enums.CardFace.Up, Configuration.FlipTweenTime);
            card.Node.AnimateFlip(Enums.CardFace.Up);
        }

        BeforeEngage?.Invoke(this);
        
        // TODO: Engage needs rework
        for (int i = 0; i < Entities.Count; i++)
        {
            var entity = Entities[i];
            for (int j = i + 1; j < Entities.Count; j++)
            {
                var otherEntity = Entities[j];
                if (entity.FactionId == otherEntity.FactionId)
                {
                    continue;
                }
                var hand = RoundHands[entity];
                var otherHand = RoundHands[otherEntity];
                
                if (HandEvaluator.Compare(hand, otherHand) >= 0)
                {
                    Attack attack = new Attack(GameMgr, this, entity, otherEntity, hand,
                        otherHand);
                    BeforeApplyDamage?.Invoke(this, attack);
                    attack.Apply();
                }
                if (HandEvaluator.Compare(hand, otherHand) <= 0)
                {
                    Attack attack = new Attack(GameMgr, this, otherEntity, entity, otherHand, 
                        hand);
                    BeforeApplyDamage?.Invoke(this, attack);
                    attack.Apply();
                }
            }
        }
        OnRoundEnd?.Invoke(this);
        CurrentState = State.AfterShowDown;
        // var endTime = Time.GetTicksUsec();
        // GD.Print($"Hand evaluation time: {endTime - startTime} us");
        // GD.Print($"{Players[0]} Best Hand: {playerBestHand.Rank}, {string.Join(",", playerBestHand.PrimaryCards)}, Kickers: {string.Join(",", playerBestHand.Kickers)}");
        // GD.Print($"{Players[1]} Best Hand: {opponentBestHand.Rank}, {string.Join(",", opponentBestHand.PrimaryCards)}, Kickers: {string.Join(",", opponentBestHand.Kickers)}");
    }

    public void InflictBuffOn(BaseBuff buff, BattleEntity target)
    {
        target.BuffContainer.Contents.Add(buff);
    }

    public void OnEntityDefeated(BattleEntity e)
    {
        GD.Print($"{e} defeated");
        if (e == Player)
        {
            GD.Print($"You lose");
            OnBattleFinished?.Invoke(this);
        }
        else
        {
            Entities.Remove(e);
            if (Entities.Count == 1)
            {
                GD.Print($"{Entities[0]} wins");
                OnBattleFinished?.Invoke(this);
            }
        }
    }
    
    public void StartEffect(BaseEffect effect)
    {
        if (!Effects.Contains(effect))
        {
            Effects.Add(effect);
            effect.OnStart(this);
        }
    }

    public void StopEffect(BaseEffect effect)
    {
        if (Effects.Contains(effect))
        {
            effect.OnStop(this);
            Effects.Remove(effect);
        }
    }

    protected Enums.CardFace GetCommunityCardFaceDirectionFunc(int i)
    {
        return i < DealCommunityCardCount - FaceDownCommunityCardCount ? Enums.CardFace.Up : Enums.CardFace.Down;
    }

    protected void OnEntityAbilityCardsChanged(object sender, NotifyCollectionChangedEventArgs args)
    {
        switch (args.Action)
        {
            
            case NotifyCollectionChangedAction.Add:
                if (args.NewItems != null)
                    foreach (var t in args.NewItems)
                    {
                        if (t is BaseCard card)
                        {
                            FieldCards.Add(card);
                        }
                    }
                break;
            case NotifyCollectionChangedAction.Remove:
                if (args.OldItems != null)
                    foreach (var t in args.OldItems)
                    {
                        if (t is BaseCard card)
                        {
                            FieldCards.Remove(card);
                        }
                    }
                break;
            case NotifyCollectionChangedAction.Reset:
                var removedCards = new List<BaseCard>();
                foreach (var fieldCard in FieldCards)
                {
                    var found = false;
                    foreach (var entity in Entities)
                    {
                        if (entity.AbilityCards.Contains(fieldCard))
                        {
                            found = true;
                            break;
                        }
                    }
                    if (!found) removedCards.Add(fieldCard);
                }

                foreach (var removedCard in removedCards)
                {
                    FieldCards.Remove(removedCard);
                }

                break;
        }
    }

}