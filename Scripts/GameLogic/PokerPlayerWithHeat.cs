using System;
using System.Collections.Generic;
using Godot;
using XCardGame.Scripts.Common.Constants;
using XCardGame.Scripts.Common.DataBinding;
using XCardGame.Scripts.HandEvaluate;

namespace XCardGame.Scripts.GameLogic;

public partial class PokerPlayerWithHeat: PokerPlayer
{
    public Action OnOverheat;
    
    public ObservableProperty<int> Heat;
    public ObservableProperty<int> HeatThreshold;
    public Dictionary<Enums.HandRank, int> HandRankTemperatureMap;

    public override void Setup(Dictionary<string, object> args)
    {
        base.Setup(args);
        HandRankTemperatureMap = args["handRankTemperatureMap"] as Dictionary<Enums.HandRank, int>;
        Heat = new ObservableProperty<int>(nameof(Heat), this, 0);
        HeatThreshold = new ObservableProperty<int>(nameof(HeatThreshold), this, args.TryGetValue("temperatureThreshold", out var arg) ? (int)arg : 100);
        Heat.DetailedValueChanged += OnHeatChanged;
    }
    
    public void OnWinHand(CompletedHandStrength completedHandStrength, PokerPlayerWithHeat opponent)
    {
        if (HandRankTemperatureMap.TryGetValue(completedHandStrength.Rank, out var temperature))
        {
            opponent.Heat.Value += temperature;
        }
    }
    
    protected void OnHeatChanged(object sender, ValueChangedEventDetailedArgs<int> args)
    {
        if (args.NewValue >= HeatThreshold.Value)
        {
            OnOverheat?.Invoke();
            GD.Print($"{Creature.Name} is overheating");
        }
    }
    
}