using System.Collections.Generic;
using Godot;

namespace XCardGame.Scripts.Common.Constants;

public static class Configuration
{
    public static readonly int CompletedHandCardCount = 5;
    public static readonly float RevealTweenTime = 1f;
    public static readonly float RevealDuration = 2f;
    public static readonly float FlipTweenTime = 0.5f;
    public static readonly float TapTweenTime = 0.5f;
    public static readonly float NegateTweenTime = 0.5f;
    public static readonly float SwapCardTweenTime = 0.5f;
    public static readonly float InvalidConfirmTweenTime = 0.5f;
    public static readonly Vector2 CardSize = new Vector2(48, 68);
    public static readonly int CardContainerSeparation = 12;
    public static readonly int VulnerableMultiplier = 50;
    public static readonly int ShockDecreaseDamageDealtMultiplierPerStack = -10;
    public static readonly int ShockIncreaseDamageReceivedMultiplierPerStack = 10;
    public static readonly int ShockMaxStack = 10;
    public static readonly float AnimateCardTransformInterval = 0.1f;
    public static readonly float ContentContainerAdjustTweenTime = 0.2f;
}