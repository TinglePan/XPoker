namespace XCardGame.Scripts.GameLogic;

public class Creature
{
    public string Name;
    public int NChips;
    
    public Creature(string name, int nChips)
    {
        Name = name;
        NChips = nChips;
    }
    
    public override string ToString()
    {
        return $"{Name}({NChips} chips)";
    }
}