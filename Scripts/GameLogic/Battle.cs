using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Godot;
using XCardGame.Scripts.Cards;
using XCardGame.Scripts.Cards.PokerCards;
using XCardGame.Scripts.Common.Constants;
using XCardGame.Scripts.HandEvaluate;

namespace XCardGame.Scripts.GameLogic;

public partial class Battle: Node, ISetup
{
    public Action OnNewRound;
    public Action OnBattleFinished;
    
    public int RoundCount;
    public PlayerBattleEntity Player;
    public List<BattleEntity> Entities;
    public DealingDeck DealingDeck;
    public ObservableCollection<BaseCard> CommunityCards;
    public int DealCommunityCardCount;
    public int FaceDownCommunityCardCount;
    public CompletedHandEvaluator HandEvaluator;

    public bool HasTurnedTables;
    public bool ShouldCalculateOut;
    
    private GameMgr _gameMgr;
    
    public override void _Ready()
    {
        _gameMgr = GetNode<GameMgr>("/root/GameMgr");
        CommunityCards = new ObservableCollection<BaseCard>();
    }

    public virtual void Setup(Dictionary<string, object> args)
    {
        Player = args["player"] as PlayerBattleEntity;
        Entities = args["entities"] as List<BattleEntity> ?? new List<BattleEntity>();
        DealCommunityCardCount = args.TryGetValue("dealCommunityCardCount", out var arg) ? (int)arg : Configuration.DefaultDealCommunityCardCount;
        FaceDownCommunityCardCount = args.TryGetValue("faceDownCommunityCardCount", out arg) ? (int)arg : Configuration.DefaultFaceDownCommunityCardCount;
        HandEvaluator = new CompletedHandEvaluator(Configuration.CompletedHandCardCount, Configuration.DefaultRequiredHoleCardCountMin, Configuration.DefaultRequiredHoleCardCountMax);
        HasTurnedTables = false;
    }
    
    public void Start()
    {
        Reset();
        DealingDeck.Shuffle();
        DealCards();
    }
    
    public void NewRound()
    {
        RoundCount++;
        foreach (var entity in Entities)
        {
            entity.RoundReset();
        }
        CommunityCards.Clear();
        DealCards();
    }
    
    public void Reset()
    {
        DealingDeck = new DealingDeck();
        foreach (var entity in Entities)
        {
            entity.Reset();
            DealingDeck.MixIn(entity.Deck);
        }
        RoundCount = 0;
        CommunityCards.Clear();
        HasTurnedTables = false;
    }

    public void DealCards()
    {
        foreach (var entity in Entities)
        {
            for (int i = 0; i < entity.DealCardCount; i++)
            {
                var card = DealingDeck.Deal(entity);
                card.Face.Value = entity == Player ? Enums.CardFace.Up : Enums.CardFace.Down;
                entity.HoleCards.Add(card);
            }
        }
        for (int i = 0; i < DealCommunityCardCount; i++)
        {
            var card = DealingDeck.Deal();
            card.Face.Value = i < DealCommunityCardCount - FaceDownCommunityCardCount ? Enums.CardFace.Up : Enums.CardFace.Down;
            CommunityCards.Add(card);
        }
    }
    
    public void ShowDown()
    {
        void FlipFaceDownCards(IEnumerable<BaseCard> cards)
        {
            foreach (var card in cards)
            {
                if (card.Face.Value == Enums.CardFace.Down)
                {
                    card.Flip(this, null);
                }
            }
        }
        
        // var startTime = Time.GetTicksUsec();

        
        var handStrengths = new Dictionary<BattleEntity, CompletedHand>();
        foreach (var entity in Entities)
        {
            var bestHand = HandEvaluator.EvaluateBestHand(CommunityCards.OfType<BasePokerCard>().ToList(),
                entity.HoleCards.OfType<BasePokerCard>().ToList());
            handStrengths.Add(entity, bestHand);
        }

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
                var handStr = handStrengths[entity];
                var otherHandStr = handStrengths[otherEntity];
                if (HasTurnedTables) (handStr, otherHandStr) = (otherHandStr, handStr);
                
                if (handStr.CompareTo(otherHandStr) >= 0)
                {
                    entity.Attack(otherEntity, handStr, otherHandStr);
                }
                if (handStr.CompareTo(otherHandStr) <= 0)
                {
                    otherEntity.Attack(entity, otherHandStr, handStr);
                }
            }
        }
        
        foreach (var entity in Entities)
        {
            FlipFaceDownCards(entity.HoleCards);
        }
        FlipFaceDownCards(CommunityCards);
        // var endTime = Time.GetTicksUsec();
        // GD.Print($"Hand evaluation time: {endTime - startTime} us");
        // GD.Print($"{Players[0]} Best Hand: {playerBestHand.Rank}, {string.Join(",", playerBestHand.PrimaryCards)}, Kickers: {string.Join(",", playerBestHand.Kickers)}");
        // GD.Print($"{Players[1]} Best Hand: {opponentBestHand.Rank}, {string.Join(",", opponentBestHand.PrimaryCards)}, Kickers: {string.Join(",", opponentBestHand.Kickers)}");
    }


    public void OnEntityWin(BattleEntity e)
    {
    }

    public void OnEntityDefeated(BattleEntity e)
    {
        GD.Print($"{e} defeated");
    }

}