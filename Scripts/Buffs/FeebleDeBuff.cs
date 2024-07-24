﻿using XCardGame.Common;

namespace XCardGame;

public class FeebleDeBuff: BaseBuff
{
    public FeebleDeBuff(int stack) : 
        base("Feeble", $"Reduce power of following skill by 1 per stack", "res://Sprites/BuffIcons/feeble.png", 
            true, stack:stack, maxStack:Configuration.CommonBuffMaxStack, isTemporary:true)
    {
    }
}