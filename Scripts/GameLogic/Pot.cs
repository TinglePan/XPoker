using System.Collections.Generic;
using System.Linq;
using XCardGame.Scripts.HandEvaluate;

namespace XCardGame.Scripts.GameLogic;

public class Pot
{
    private Hand _hand;
    
    public Dictionary<PokerPlayer, int> Bets;
    public int Total => Bets.Values.Sum();
    public int MainPotAmount;
    public int HighestBet => Bets.Values.Max();
    
    private List<SidePot> _sidePots;
    
    public Pot(Hand hand)
    {
        _hand = hand;
        Bets = new Dictionary<PokerPlayer, int>();
        _sidePots = new List<SidePot>();
        MainPotAmount = 0;
    }

    public void Reset()
    {
        Bets.Clear();
        _sidePots.Clear();
        MainPotAmount = 0;
    }
    
    public void AddBet(PokerPlayer player, int amount)
    {
        if (!Bets.TryAdd(player, amount))
        {
            Bets[player] += amount;
        }
        MainPotAmount += amount;
    }

    public void CreateSidePot(int amount)
    {
        var sidePot = new SidePot(amount);
        _sidePots.Add(sidePot);
    }

    public void SplitPot()
    {
        var orderedSidePot = _sidePots.OrderBy(sidePot => sidePot.CreationAmount);
        for (int i = 0; i < _sidePots.Count; i++)
        {
            foreach (var player in _hand.Players)
            {
                if (player.RoundBetAmount.Value >= _sidePots[i].CreationAmount)
                {
                    _sidePots[i].QualifiedPlayers.Add(player);
                }
            }
            _sidePots[i].Value = _sidePots[i].QualifiedPlayers.Count * _sidePots[i].CreationAmount;
            MainPotAmount -= _sidePots[i].Value;
        }
    }
    
    public void Settlement(Dictionary<PokerPlayer, CompletedHandStrength> handStrengths)
    {
        SplitPot();
        var orderedHandStrengths = handStrengths.OrderByDescending(pair => pair.Value).ToList();
        foreach (var sidePot in _sidePots)
        {
            foreach (var (player, _) in orderedHandStrengths)
            {
                if (sidePot.QualifiedPlayers.Contains(player))
                {
                    sidePot.GrantTo(player);
                }
            }
        }

        foreach (var (player, _) in orderedHandStrengths)
        {
            if (Bets[player] == HighestBet)
            {
                GrantTo(player);
                break;
            }
        }
    }
    
    public void GrantTo(PokerPlayer player)
    {
        player.NChipsInHand.Value += MainPotAmount;
    }
    
}