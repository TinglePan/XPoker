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

public partial class Hand: Node, ISetup
{

    [Signal]
    public delegate void FinishedEventHandler();
    
    private GameMgr _gameMgr;
    public List<PokerPlayer> Players;
    public Deck Deck;
    public Deck.DealingDeck DealingDeck;
    
    public int ButtonPlayerIndex;
    public int ActionPlayerIndex;
    public int LastBetPlayerIndex;
    public int BigBlindAmount;
    public int RaiseToLimit;
    public int RoundCallAmount;
    public int RoundPreviousRaiseAmount;
    public bool RoundHasShortAllIn;
    public int CreateSidePotAtAmount;
    public int RoundCount;
    public Pot Pot;
    public ObservableCollection<BaseCard> CommunityCards;
    
    public bool IsHeadUp => Players.Count <= 2;
    public bool HasOpened => RoundCallAmount > 0;
    
    public int ActingPlayerCount => Players.Count(p => p.CanAct);
    public int InHandPlayerCount => Players.Count(p => p.IsInHand);
    public int RoundStartPlayerIndex => NextNActingPlayerFrom(ButtonPlayerIndex, 1);
    public int RoundMinRaiseToAmount => Mathf.Max(RoundCallAmount + RoundPreviousRaiseAmount, BigBlindAmount); 

    public override void _Ready()
    {
        _gameMgr = GetNode<GameMgr>("/root/GameMgr");
        Players = new List<PokerPlayer>();
        Deck = new Deck(_gameMgr);
        DealingDeck = null;
        ButtonPlayerIndex = 0;
        ActionPlayerIndex = 0;
        LastBetPlayerIndex = 0;
        BigBlindAmount = Configuration.InitialBigBlindAmount;
        RaiseToLimit = Configuration.RaiseLimit;
        RoundCallAmount = 0;
        RoundPreviousRaiseAmount = 0;
        RoundHasShortAllIn = false;
        CreateSidePotAtAmount = 0;
        RoundCount = 0;
        Pot = new Pot(this);
        CommunityCards = new ObservableCollection<BaseCard>();
    }

    public override void _ExitTree()
    {
        if (Players != null)
        {
            foreach (var player in Players)
            {
                player.OnCall -= OnPlayerCall;
                player.OnRaise -= OnPlayerRaise;
                player.OnAllIn -= OnPlayerAllIn;
            }
        }
    }

    public virtual void Setup(Dictionary<string, object> args)
    {
        Players = args["players"] as List<PokerPlayer> ?? new List<PokerPlayer>();
        if (Players.Count < 2)
        {
            GD.PrintErr("Match requires at least 2 players.");
        }
        foreach (var player in Players)
        {
            player.OnCall += OnPlayerCall;
            player.OnRaise += OnPlayerRaise;
            player.OnAllIn += OnPlayerAllIn;
        }
    }
    
    public async void Start()
    {
        Reset();
        foreach (var player in Players)
        {
            player.ResetHandState();
        }

        while (RoundCount <= Configuration.RiverRoundIndex && InHandPlayerCount > 1)
        {
            await StartBettingRound(RoundCount);
            RoundCount++;
        }

        HandDone();
        EmitSignal(SignalName.Finished);
    }
    
    public void Reset()
    {
        foreach (var player in Players)
        {
            player.ResetHandState();
        }
        DealingDeck = Deck.CreateDealingDeck();
        ButtonPlayerIndex = 0;
        CreateSidePotAtAmount = 0;
        RoundCount = 0;
        Pot.Reset();
        CommunityCards.Clear();
        
    }

    public async Task StartBettingRound(int n)
    {
        RoundCallAmount = 0;
        RoundPreviousRaiseAmount = 0;
        RoundHasShortAllIn = false;
        RoundCount = n;
        foreach (var player in Players)
        {
            player.ResetRoundState();
        }

        if (n == 0)
        {

            for (int i = 0; i < Configuration.InitialHoleCardCount; i++)
            {
                for (int j = 0; j < Players.Count; j++)
                {
                    var player = Players[ButtonPlayerIndex + j];
                    bool isFaceDown = player != _gameMgr.PlayerControlledPlayer;
                    isFaceDown = false;
                    var card = DealingDeck.Deal(isFaceDown);
                    player.HoleCards.Add(card);
                    GD.Print($"Dealt hole card {card} to player {player}.");
                }
            }

            foreach (var p in Players)
            {
                GD.Print($"{p} holds: {string.Join(", ", p.HoleCards)}");
            }

            int smallBlindIndexShift = IsHeadUp ? 0 : 1;
            var smallBlindPlayerIndex = NextNActingPlayerFrom(ButtonPlayerIndex, smallBlindIndexShift);
            var bigBlindPlayerIndex = NextNActingPlayerFrom(smallBlindPlayerIndex, 1);

            Players[smallBlindPlayerIndex].BetBlind(BigBlindAmount / 2, true);
            Players[bigBlindPlayerIndex].BetBlind(BigBlindAmount, false);
            ActionPlayerIndex = NextNActingPlayerFrom(bigBlindPlayerIndex, 1);
        }
        else
        {
            var dealCardCount = Configuration.DealCommunityCardCount[n];
            for (int i = 0; i < dealCardCount; i++)
            {
                var card = DealingDeck.Deal(false);
                
                CommunityCards.Add(card);
            }

            GD.Print($"CurrentCommunityCards at round {n} is:");
            foreach (var card in CommunityCards)
            {
                GD.Print($"{card}");
            }
            LastBetPlayerIndex = RoundStartPlayerIndex;
            ActionPlayerIndex = RoundStartPlayerIndex;
        }
        
        // DBG: test drawing hand evaluator performance
        var startTime = Time.GetTicksUsec();
        var evaluator = new DrawingHandEvaluator(CommunityCards.OfType<BasePokerCard>().ToList(), 5, Deck, 5, 0, 2);
        var odd0 = evaluator.EvaluateAverageOdd(Players[0].HoleCards.OfType<BasePokerCard>().ToList(), 100);
        var odd1 = evaluator.EvaluateAverageOdd(Players[1].HoleCards.OfType<BasePokerCard>().ToList(), 100);
        var endTime = Time.GetTicksUsec();
        GD.Print($"Hand evaluation time: {endTime - startTime} us");
        GD.Print($"{Players[0]} estimated odd: {odd0}");
        GD.Print($"{Players[1]} estimated odd: {odd1}");
        while (ActingPlayerCount > 0 && InHandPlayerCount > 1)
        {
            if (ActingPlayerCount == 1 && RoundCallAmount == 0) break;
            if (Players[ActionPlayerIndex].RoundBetAmount.Value < RoundCallAmount || RoundCallAmount == 0)
            {
                await AskForPlayerAction(Players[ActionPlayerIndex]);
            }
            ActionPlayerIndex = NextNActingPlayerFrom(ActionPlayerIndex, 1);
            if (ActionPlayerIndex == LastBetPlayerIndex) break;
        }
    }

    public void HandDone()
    {
        if (RoundCount > Configuration.RiverRoundIndex)
        {
            var handStrengths = ShowDown();
            Pot.Settlement(handStrengths);
        }
        else
        {
            Pot.Settlement(new Dictionary<PokerPlayer, CompletedHandStrength>()
            {
                { Players[NextNActingPlayerFrom(ButtonPlayerIndex, 0)], null }
            });
        }
    }
    
    public Dictionary<PokerPlayer, CompletedHandStrength> ShowDown()
    {
        // Play game
        var communityCards = CommunityCards.OfType<BasePokerCard>().ToList();
        // var startTime = Time.GetTicksUsec();
        var evaluator = new CompletedHandEvaluator(communityCards, 5, 0, 2);
        var playerBestHand = evaluator.EvaluateBestHand(Players[0].HoleCards.OfType<BasePokerCard>().ToList());
        evaluator.Clear();
        var opponentBestHand = evaluator.EvaluateBestHand(Players[1].HoleCards.OfType<BasePokerCard>().ToList());
        // var endTime = Time.GetTicksUsec();
        // GD.Print($"Hand evaluation time: {endTime - startTime} us");
        // GD.Print($"{Players[0]} Best Hand: {playerBestHand.Rank}, {string.Join(",", playerBestHand.PrimaryCards)}, Kickers: {string.Join(",", playerBestHand.Kickers)}");
        // GD.Print($"{Players[1]} Best Hand: {opponentBestHand.Rank}, {string.Join(",", opponentBestHand.PrimaryCards)}, Kickers: {string.Join(",", opponentBestHand.Kickers)}");
        Dictionary<PokerPlayer, CompletedHandStrength> handStrengths = new Dictionary<PokerPlayer, CompletedHandStrength>
        {
            { Players[0], playerBestHand },
            { Players[1], opponentBestHand }
        };
        return handStrengths;
    }


    public void OnPlayerCall(PokerPlayer p, int amountToCall)
    {
        Pot.AddBet(p, amountToCall);
    }
    
    public void OnPlayerRaise(PokerPlayer p, int amountToRaise)
    {
        Pot.AddBet(p, amountToRaise);
        RoundCallAmount = p.RoundBetAmount.Value;
        RoundPreviousRaiseAmount = amountToRaise;
        LastBetPlayerIndex = Players.IndexOf(p);
    }
    
    public void OnPlayerAllIn(PokerPlayer p, int allInAmount)
    {
        if (allInAmount < RoundMinRaiseToAmount && p.RoundLastAction == Enums.PlayerAction.Raise)
        {
            RoundHasShortAllIn = true;
        }
        Pot.CreateSidePot(p.NChipsInPot.Value);
    }

    public void OnPlayerWin(PokerPlayer p)
    {
    }

    public int NextNActingPlayerFrom(int fromIndex, int n)
    {
        var recordIndex = fromIndex;
        var offset = 0;
        var i = 0;
        while (i <= n)
        {
            var playerIndex = (fromIndex + offset) % Players.Count;
            if (Players[playerIndex].CanAct)
            {
                recordIndex = playerIndex;
                i++;
            }
            offset++;
            if (offset >= Players.Count && i == 0)
            {
                break;
            }
        }
        return recordIndex;
    }
    
    public int RoundActingOrder(int playerIndex)
    {
        var i = 0;
        for (int j = 0; j < Players.Count; j++)
        {
            var index = NextNActingPlayerFrom(ButtonPlayerIndex, j);
            if (index == playerIndex)
            {
                return i;
            }

            i++;
        }

        return -1;
    }

    public int ValidStackAmount()
    {
        var amount = 0;
        foreach (var player in Players)
        {
            if (!player.CanAct) continue;
            if (amount <= 0 || player.NChipsInHand.Value < amount)
            {
                amount = player.NChipsInHand.Value;
            }
        }
        return amount;
    }
    
    protected async Task AskForPlayerAction(PokerPlayer p)
    {
        // Ask for player action
        await p.AskForAction(new Dictionary<string, object>()
        {
            { "hand", this },
            { "player", p }
        });
        GD.Print($"Player {p} made a {p.RoundLastAction}.");
    }
}