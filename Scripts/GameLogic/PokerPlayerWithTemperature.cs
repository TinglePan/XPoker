using System;
using System.Collections.Generic;
using Godot;
using XCardGame.Scripts.Common.Constants;
using XCardGame.Scripts.Common.DataBinding;
using XCardGame.Scripts.HandEvaluate;

namespace XCardGame.Scripts.GameLogic;

public partial class PokerPlayerWithTemperature: PokerPlayer
{
    public Action OnOverheat;
    
    public ObservableProperty<int> Temperature;
    public ObservableProperty<int> TemperatureThreshold;
    public Dictionary<Enums.HandRank, int> HandRankTemperatureMap;

    public override void Setup(Dictionary<string, object> args)
    {
        base.Setup(args);
        HandRankTemperatureMap = args["handRankTemperatureMap"] as Dictionary<Enums.HandRank, int>;
        Temperature = new ObservableProperty<int>(nameof(Temperature), this, 0);
        TemperatureThreshold = new ObservableProperty<int>(nameof(TemperatureThreshold), this, args.TryGetValue("temperatureThreshold", out var arg) ? (int)arg : 100);
        Temperature.DetailedValueChanged += OnTemperatureChanged;
    }
    
    public void OnWinHand(CompletedHandStrength completedHandStrength, PokerPlayerWithTemperature opponent)
    {
        if (HandRankTemperatureMap.TryGetValue(completedHandStrength.Rank, out var temperature))
        {
            opponent.Temperature.Value += temperature;
        }
    }
    
    protected void OnTemperatureChanged(object sender, ValueChangedEventDetailedArgs<int> args)
    {
        if (args.NewValue >= TemperatureThreshold.Value)
        {
            OnOverheat?.Invoke();
            GD.Print($"{Creature.Name} is overheating");
        }
    }
    
}