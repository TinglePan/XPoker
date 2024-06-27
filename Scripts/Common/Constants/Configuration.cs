using System.Collections.Generic;
using System.Diagnostics;
using Godot;

namespace XCardGame.Scripts.Common.Constants;

public static class Configuration
{
    public static readonly int CompletedHandCardCount = 5;
    public static readonly int CommunityCardCount = 5;
    public static readonly int DefaultHoleCardCount = 2;
    public static readonly int RoundSkillsCount = 1;

    public static readonly Vector2 SelectedCardOffset = new Vector2(0, -24);
    
    public static readonly float RevealTweenTime = 1f;
    public static readonly float RevealDuration = 2f;
    public static readonly float FlipTweenTime = 0.5f;
    public static readonly float TapTweenTime = 0.5f;
    public static readonly float NegateTweenTime = 0.5f;
    public static readonly float SwapCardTweenTime = 0.5f;
    public static readonly float InvalidConfirmTweenTime = 0.5f;
    public static readonly float EmphasizeTweenTime = 0.3f;
    public static readonly Vector2 CardSize = new Vector2(48, 68);
    public static readonly Vector2 CardContainerSeparation = new Vector2(12, 8);
    public static readonly int ShockDecreaseDamageDealtMultiplierPerStack = -10;
    public static readonly int ShockIncreaseDamageReceivedMultiplierPerStack = 10;
    public static readonly float AnimateCardTransformInterval = 0.1f;
    public static readonly float ContentContainerAdjustTweenTime = 0.2f;
    public static readonly float DelayBetweenResolveSteps = 0.5f;
    public static readonly int ShuffleAnimateCards = 3;
    // public static readonly int FieldCardCountPerRow = 4;
    // public static readonly int SkillCardCountPerRow = 6;
    public static readonly int BuffCountPerRow = 12;
    
    public static readonly int MaxDefence = 999;
    public static readonly int ProgressCountRequiredToWin = 8;

    public static readonly int DefaultRewardCardCount = 3;
    public static readonly int DefaultReRollPrice = 5;
    public static readonly int DefaultReRollPriceIncrease = 5;
    public static readonly int DefaultSkipReward = 30;

    public static readonly int ShopPokerCardCount = 3;
    public static readonly int ShopSkillCardCount = 2;
    public static readonly int ShopAbilityCardCount = 2;
    public static readonly int ShopMarkerCount = 3;
    public static readonly int ShopRemoveCardPriceIncrease = 5;

    public static readonly int CommonBuffMaxStack = 99;
    public static readonly int PowerBasedBuffMaxStack = 999;

    public static readonly int VulnerableMultiplier = 50;
    public static readonly int WeakenMultiplier = 25;
    public static readonly int FragileMultiplier = 50;

    public static readonly Vector4 DefaultContentContainerMargins = new Vector4(8, 4, 8, 4);
}