using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using Godot;
using Godot.Collections;
using XCardGame.Scripts.Buffs;
using XCardGame.Scripts.Cards;
using XCardGame.Scripts.Common.Constants;
using XCardGame.Scripts.Effects;
using XCardGame.Scripts.HandEvaluate;
using XCardGame.Scripts.Nodes;

namespace XCardGame.Scripts.GameLogic;

public partial class Battle: Node2D, ISetup
{

    public enum State
    {
        Finished,
        BeforeDealCards,
        BeforeShowDown,
        AfterShowDown
    }
    
    public GameMgr GameMgr;
    public Dealer Dealer;
    public CardContainer CommunityCardContainer;
    public CardContainer FieldCardContainer;
    public Array<BattleEntity> Entities;

    public BaseButton ProceedButton;
    
    public PlayerBattleEntity Player => Entities[0] as PlayerBattleEntity;
    
    public bool HasSetup { get; set; }
    
    public Action<Battle> OnBattleProceed;
    public Action<Battle> AfterDealCards;
    public Action<Battle> OnRoundStart;
    public Action<Battle> OnRoundEnd;
    public Action<Battle> BeforeShowDown;
    public Action<Battle> BeforeEngage;
    public Action<Battle, Attack> BeforeApplyAttack;
    public Action<Battle> OnBattleFinished;

    public ObservableCollection<BaseCard> CommunityCards;
    public ObservableCollection<BaseCard> FieldCards;
    
    public CompletedHandEvaluator HandEvaluator;
    
    public int DealCommunityCardCount;
    public int FaceDownCommunityCardCount;
    public int RequiredHoleCardCountMin;
    public int RequiredHoleCardCountMax;
    
    public int RoundCount;
    public System.Collections.Generic.Dictionary<BattleEntity, CompletedHand> RoundHands;
    
    public List<BaseEffect> Effects;
    public State CurrentState;

    
    public override void _Ready()
    {
        base._Ready();
        GameMgr = GetNode<GameMgr>("/root/GameMgr");
        Dealer = GetNode<Dealer>("Dealer");
        CommunityCardContainer = GetNode<CardContainer>("CommunityCards");
        FieldCardContainer = GetNode<CardContainer>("FieldCards");
        HasSetup = false;
        RoundHands = new System.Collections.Generic.Dictionary<BattleEntity, CompletedHand>();
        CommunityCards = new ObservableCollection<BaseCard>();
        FieldCards = new ObservableCollection<BaseCard>();
    }

    public virtual void Setup(System.Collections.Generic.Dictionary<string, object> args)
    {
        DealCommunityCardCount = (int)args["dealCommunityCardCount"];
        FaceDownCommunityCardCount = (int)args["faceDownCommunityCardCount"];
        RequiredHoleCardCountMin = (int)args["requiredHoleCardCountMin"];
        RequiredHoleCardCountMax = (int)args["requiredHoleCardCountMax"];
        HandEvaluator = new CompletedHandEvaluator(Configuration.CompletedHandCardCount, 
            RequiredHoleCardCountMin, RequiredHoleCardCountMax);

        // FIXME: Weird. The exported array suddenly does not work any more. The nodes dropped in editor Array do not exist here. So I have to manually add them.
        Entities = new Array<BattleEntity>
        {
            FindChild("Player") as PlayerBattleEntity,
            FindChild("Enemy") as BattleEntity
        };

        CommunityCardContainer.Setup(new System.Collections.Generic.Dictionary<string, object>()
        {
            { "allowInteract", false },
            { "cards", CommunityCards },
            { "contentNodeSize", Configuration.CardSize },
            { "separation", Configuration.CardContainerSeparation },
            { "pivotDirection", Enums.Direction2D8Ways.Neutral },
            { "nodesPerRow", Configuration.CommunityCardCount },
            { "expectedContentNodeCount", Configuration.CommunityCardCount },
            { "growBorder", false },
            { "containerName", "Community cards"},
            { "getCardFaceDirectionFunc", (Func<int, Enums.CardFace>)GetCommunityCardFaceDirectionFunc }
        });
        
        FieldCardContainer.Setup(new System.Collections.Generic.Dictionary<string, object>()
        {
            { "allowInteract", true },
            { "cards", FieldCards },
            { "contentNodeSize", Configuration.CardSize },
            { "separation", Configuration.CardContainerSeparation },
            { "pivotDirection", Enums.Direction2D8Ways.Neutral },
            { "nodesPerRow", Configuration.FieldCardCountPerRow },
            { "expectedContentNodeCount", 0 },
            { "growBorder", false },
            { "containerName", "Field cards"},
            { "defaultCardFaceDirection", Enums.CardFace.Up } 
        });
        
        var entitiesSetupArgs = (List<System.Collections.Generic.Dictionary<string, object>>)args["entities"];
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
        
        Dealer.Setup(new System.Collections.Generic.Dictionary<string, object>()
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
        var index = 0;
        foreach (var entity in Entities)
        {
            var delay = Configuration.AnimateCardTransformInterval * index;
            index += entity.HoleCards.Count;
            entity.RoundReset(delay);
        }
        foreach (var cardNode in CommunityCardContainer.ContentNodes.ToList())
        {
            Dealer.AnimateDiscard(cardNode, Configuration.AnimateCardTransformInterval * index);
            index++;
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
        var cardDealt = 0;
        foreach (var entity in Entities)
        {
            for (int i = 0; i < entity.DealCardCount; i++)
            {
                Dealer.DealCardIntoContainer(entity.HoleCardContainer, delay: cardDealt * Configuration.AnimateCardTransformInterval);
                cardDealt++;
            }
        }
        for (int i = 0; i < DealCommunityCardCount; i++)
        {
            Dealer.DealCardIntoContainer(CommunityCardContainer, delay: cardDealt * Configuration.AnimateCardTransformInterval);
            cardDealt++;
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
            foreach (var cardNode in entity.HoleCardContainer.ContentNodes)
            {
                if (cardNode.FaceDirection.Value == Enums.CardFace.Down)
                {
                    cardNode.AnimateFlip(Enums.CardFace.Up);
                }
            }
        }
        foreach (var cardNode in CommunityCardContainer.ContentNodes)
        {
            if (cardNode.FaceDirection.Value == Enums.CardFace.Down)
            {
                cardNode.AnimateFlip(Enums.CardFace.Up);
            }
        }

        BeforeEngage?.Invoke(this);
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
                    BeforeApplyAttack?.Invoke(this, attack);
                    attack.Apply();
                }
                if (HandEvaluator.Compare(hand, otherHand) <= 0)
                {
                    Attack attack = new Attack(GameMgr, this, otherEntity, entity, otherHand, 
                        hand);
                    BeforeApplyAttack?.Invoke(this, attack);
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

    public BattleEntity GetOpponentOf(BattleEntity entity)
    {
        foreach (var maybeOpponent in Entities)
        {
            if (maybeOpponent != entity)
            {
                return maybeOpponent;
            }
        }
        return null;
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

    public void InflictBuffOn(BaseBuff buff, BattleEntity target)
    {
        if (target.Buffs.Contains(buff))
        {
            buff.Repeat(this, target);
        }
        else
        {
            target.Buffs.Add(buff);
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