﻿using XCardGame.Scripts.Cards;
using XCardGame.Scripts.Common.Constants;
using XCardGame.Scripts.GameLogic;

namespace XCardGame.Scripts.Buffs;

public class VulnerableDeBuff: BaseBuff
{
    public VulnerableDeBuff(int stack) : base(
        "Vulnerable", $"Increase incoming damage by {Configuration.VulnerableMultiplier} percent, consumes 1 stack on taking effect", 
        "res://Sprites/BuffIcons/vulnerable.png", true, stack:stack, maxStack:Configuration.CommonBuffMaxStack)
    {
    }

    public override void Consume()
    {
        ChangeStack(-1);
    }
}