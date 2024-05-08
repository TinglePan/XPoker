using System.Collections.Generic;
using Godot;

namespace XCardGame.Scripts.Common.Constants;

public static class Configuration
{
    public static readonly int DefaultDealCardCount = 2;
    public static readonly int DefaultDealCommunityCardCount = 5;
    public static readonly int DefaultFaceDownCommunityCardCount = 1;
    public static readonly int DefaultRequiredHoleCardCountMin = 0;
    public static readonly int DefaultRequiredHoleCardCountMax = 2;
    public static readonly int CompletedHandCardCount = 5;
    public static readonly int DefaultCrossTierThreshold = 2;
    public static readonly float RevealFadeInDuration = 1f;
    public static readonly float RevealFadeOutDuration = 0.5f;
    public static readonly float RevealDelayPerCard = 0.2f;
    public static readonly float RevealDuration = 2f;
}