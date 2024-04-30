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
    [Signal]
    public delegate void FinishedEventHandler();

    public int RoundCount;
    public PlayerBattleEntity Player;
    public List<BattleEntity> Entities;
    public DealingDeck DealingDeck;
    public ObservableCollection<BaseCard> CommunityCards;
    public int DealCommunityCardCount;
    public int FaceDownCommunityCardCount;
    
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
    }
    
    public void Start()
    {
        Reset();
        DealingDeck.Shuffle();
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
        // var startTime = Time.GetTicksUsec();
        var evaluator = new CompletedHandEvaluator(CommunityCards.OfType<BasePokerCard>().ToList(), 5, 0, 2);
        var handStrengths = new Dictionary<BattleEntity, CompletedHand>();
        foreach (var entity in Entities)
        {
            handStrengths.Add(entity, evaluator.EvaluateBestHand(entity.HoleCards.OfType<BasePokerCard>().ToList()));
            evaluator.Reset();
        }

        for (int i = 0; i < Entities.Count; i++)
        {
            var entity = Entities[i];
            var handStr = handStrengths[entity];
            for (int j = i + 1; j < Entities.Count; j++)
            {
                var otherEntity = Entities[j];
                var otherHandStr = handStrengths[otherEntity];
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