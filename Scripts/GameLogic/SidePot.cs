using System.Collections.Generic;

namespace XCardGame.Scripts.GameLogic;

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
    
    public void GrantTo(PokerPlayer player)
    {
        player.NChipsInHand.Value += Value;
    }
}