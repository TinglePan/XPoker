using System.Collections.Generic;
using Godot;
using XCardGame.Scripts.Common.Constants;

namespace XCardGame.Scripts;

public class Match: ISetup
{
    public List<Player> Players;
    public int ButtonPlayerIndex;
    public int CurrentBetPlayerIndex;
    public Deck Deck;
    public DealingDeck CurrentDealingDeck;
    public int CurrentBetAmount;
    
    public void Start()
    {
        CurrentDealingDeck.Reset();
        CurrentBetAmount = Configuration.AnteAmount;
        for (int i = 0; i < Configuration.InitialHoleCardCount; i++)
        {
            for (int j = 0; j < Players.Count; j++)
            {
                CurrentDealingDeck.DealCardTo(Players[ButtonPlayerIndex + j]);
            }
        }
        // Play();
        // End();
    }

    public virtual void Setup(Dictionary<string, object> args)
    {
        Players = (List<Player>)args["players"];
        Deck = new Deck();
        CurrentDealingDeck = new DealingDeck(Deck);
        // Setup game
    }

    public void Play()
    {
        // Play game
    }

    public void End()
    {
        // End game
    }
}