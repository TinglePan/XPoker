using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace XCardGame.Scripts;

public class Pot
{


    class SidePot
    {
        public int CreationAmount;
        public int Value;
        public HashSet<PokerPlayer> QualifiedPlayers;

        public SidePot(int creationAmount)
        {
            CreationAmount = creationAmount;
            Value = 0;
            QualifiedPlayers = new HashSet<PokerPlayer>();
        }
    }
    
    private Hand _hand;
    
    public Dictionary<PokerPlayer, int> Bets;
    public int Total => Bets.Values.Sum();
    
    private List<SidePot> _sidePots;
    
    public Pot(Hand hand)
    {
        _hand = hand;
        Bets = new Dictionary<PokerPlayer, int>();
        _sidePots = new List<SidePot>();
    }

    public void Reset()
    {
        Bets.Clear();
        _sidePots.Clear();
    }
    
    public void AddBet(PokerPlayer player, int amount)
    {
        if (!Bets.TryAdd(player, amount))
        {
            Bets[player] += amount;
        }
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
                if (player.RoundBetAmount >= _sidePots[i].CreationAmount)
                {
                    _sidePots[i].QualifiedPlayers.Add(player);
                }
            }
        }
        
    }
    
}