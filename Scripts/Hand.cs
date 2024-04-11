using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot;
using XCardGame.Scripts.Cards;
using XCardGame.Scripts.Common.Constants;
using XCardGame.Scripts.Ui;

namespace XCardGame.Scripts;

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
    public List<BaseCard> CommunityCards;
    
    public bool IsHeadUp => Players.Count <= 2;
    
    public int ActingPlayerCount => Players.Count(p => p.CanAct);
    public int RoundStartPlayerIndex => NextNActingPlayerFrom(ButtonPlayerIndex, 1);
    public int RoundMinRaiseToAmount => RoundCallAmount + RoundPreviousRaiseAmount; 

    public override void _Ready()
    {
        _gameMgr = GetNode<GameMgr>("/root/GameMgr");
        Players = new List<PokerPlayer>();
        Deck = new Deck();
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
        CommunityCards = new List<BaseCard>();
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

        while (RoundCount <= Configuration.RiverRoundIndex && ActingPlayerCount > 1)
        {
            await StartBettingRound(RoundCount);
            RoundCount++;
        }
        if (RoundCount > Configuration.RiverRoundIndex)
        {
            ShowDown();
        }
        else
        {
            GD.Print($"Win by {NextNActingPlayerFrom(ButtonPlayerIndex, 0)}");
        }
        EmitSignal(SignalName.Finished);
    }
    
    public void Reset()
    {
        
        foreach (var player in Players)
        {
            player.Reset();
        }
        DealingDeck = Deck.Deal();
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
                    var card = DealingDeck.Deal();
                    var player = Players[ButtonPlayerIndex + j];
                    player.AddHoleCard(card);
                    GD.Print($"Dealt hole card {card} to player {player}.");
                }
            }

            foreach (var p in Players)
            {
                GD.Print($"{p} holds: {string.Join(", ", p.HoleCards)}");
            }

            int smallBlindIndexShift = IsHeadUp ? 0 : 1;
            var smallBlindPlayerIndex = NextNActingPlayerFrom(ButtonPlayerIndex, smallBlindIndexShift);
            var bigBlindPlayerIndex = NextNActingPlayerFrom(ButtonPlayerIndex, smallBlindIndexShift + 1);

            Players[smallBlindPlayerIndex].BetBlind(BigBlindAmount / 2, true);
            Players[bigBlindPlayerIndex].BetBlind(BigBlindAmount, false);
            ActionPlayerIndex = NextNActingPlayerFrom(ButtonPlayerIndex, smallBlindIndexShift + 2);
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
        
        while (true) {
            if (Players[ActionPlayerIndex].RoundBetAmount < RoundCallAmount || RoundCallAmount == 0)
            {
                await AskForPlayerAction(Players[ActionPlayerIndex]);
            }
            ActionPlayerIndex = NextNActingPlayerFrom(ActionPlayerIndex, 1);
            if (ActionPlayerIndex == LastBetPlayerIndex) break;
        }
    }
    
    public void ShowDown()
    {
        // Play game
        // GD.Print("Showdown:");
        // GD.Print($"CurrentCommunityCards are:");
        // foreach (var card in CommunityCards)
        // {
        //     GD.Print($"{card}");
        // }
        // foreach (var player in Players)
        // {
        //     GD.Print($"{player} holds: {string.Join(", ", player.HoleCards)}");
        // }
        
        var playerEvaluator = new HandEvaluator(Players[0].HoleCards, CommunityCards, 5, 0, 2);
        var playerBestHand = playerEvaluator.EvaluateBestHand();
        var opponentEvaluator = new HandEvaluator(Players[1].HoleCards, CommunityCards, 5, 0, 2);
        var opponentBestHand = opponentEvaluator.EvaluateBestHand();
        GD.Print($"{Players[0]} Best Hand: {playerBestHand.Rank}, {string.Join(",", playerBestHand.PrimaryCards)}, Kickers: {string.Join(",", playerBestHand.Kickers)}");
        GD.Print($"{Players[1]} Best Hand: {opponentBestHand.Rank}, {string.Join(",", opponentBestHand.PrimaryCards)}, Kickers: {string.Join(",", opponentBestHand.Kickers)}");
        var compareRes = playerBestHand.CompareTo(opponentBestHand);
        if (compareRes > 0)
        {
            GD.Print($"{Players[0]} wins.");
        }
        else if (compareRes < 0)
        {
            GD.Print($"{Players[0]} wins.");
        }
        else
        {
            GD.Print("Draw.");
        }
    }


    public void OnPlayerCall(PokerPlayer p, int amountToCall)
    {
        Pot.AddBet(p, amountToCall);
    }
    
    public void OnPlayerRaise(PokerPlayer p, int amountToRaise)
    {
        Pot.AddBet(p, amountToRaise);
        RoundCallAmount += amountToRaise;
        RoundPreviousRaiseAmount = amountToRaise;
        LastBetPlayerIndex = Players.IndexOf(p);
    }
    
    public void OnPlayerAllIn(PokerPlayer p, int allInAmount)
    {
        if (allInAmount < RoundMinRaiseToAmount && p.RoundLastAction == Enums.PlayerAction.Raise)
        {
            RoundHasShortAllIn = true;
        }
        Pot.CreateSidePot(allInAmount);
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
                GD.PrintErr("No acting player found.");
                break;
            }
        }
        return recordIndex;
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