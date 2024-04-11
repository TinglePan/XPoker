using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Godot;
using XCardGame.Scripts.Brain;
using XCardGame.Scripts.Cards;
using XCardGame.Scripts.Common.Constants;
using XCardGame.Scripts.Ui;

namespace XCardGame.Scripts;

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