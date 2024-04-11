using System;
using System.Collections.Generic;
using Godot;
using XCardGame.Scripts.Common.DataBinding;

namespace XCardGame.Scripts.Ui;


public partial class RaiseUi: BaseUi, ISetup
{
    [Export] public NumberLineEdit AmountLineEdit;
    [Export] public HScrollBar AmountBar;
    
    [Export] public Button AllInButton;

    [Export] public HBoxContainer BetButtons;
    
    // UnOpened
    [Export] public BaseButton TwoPointFiveBigBlindButton;
    [Export] public BaseButton ThreeBigBlindButton;
    [Export] public BaseButton FourBigBlindButton;
    [Export] public BaseButton FiveBigBlindButton;
    
    // Opened
    [Export] public BaseButton OneFourthPotButton;
    [Export] public BaseButton HalfPotButton;
    [Export] public BaseButton TwoThirdsPotButton;
    [Export] public BaseButton OnePotButton;

    private int _startAmount;
    // private int _potAmount;
    // private int _playerNChips;
    // private int _limit;
    
    private int _actualLimit;
    
    private ObservableProperty<int> _amount;

    private Dictionary<BaseButton, float> _unOpenedButtonsRatio;
    private Dictionary<BaseButton, float> _openedButtonsRatio;
    
    private Dictionary<BaseButton, Action> _buttonPressedCallbacks = new Dictionary<BaseButton, Action>();
    
    public int Amount => _amount.Value;
    
    public override void _Ready()
    {
        _startAmount = 0;
        // _potAmount = 0;
        // _playerNChips = 0;
        // _limit = 0;
        _amount = new ObservableProperty<int>(nameof(_amount), 0);
        _amount.DetailedValueChanged += OnAmountChanged;
        AmountLineEdit.ValueChanged += (from, to) => _amount.Value = AmountLineEdit.Value;
        AmountBar.ValueChanged += to => _amount.Value = (int)to;
        _unOpenedButtonsRatio = new Dictionary<BaseButton, float>()
        {
            { TwoPointFiveBigBlindButton, 2.5f },
            { ThreeBigBlindButton, 3f },
            { FourBigBlindButton, 4f },
            { FiveBigBlindButton, 5f }
        };
        _openedButtonsRatio = new Dictionary<BaseButton, float>()
        {
            { OneFourthPotButton, 0.25f },
            { HalfPotButton, 0.5f },
            { TwoThirdsPotButton, 0.67f },
            { OnePotButton, 1f }
        };
        _buttonPressedCallbacks = new Dictionary<BaseButton, Action>();
    }

    public void Setup(Dictionary<string, object> args)
    {
        var hand = (Hand)args["hand"];
        var player = (PokerPlayer)args["player"];
        var bigBlindAmount = hand.BigBlindAmount;
        var potAmount = hand.Pot.Total;
        var playerNChips = player.NChipsInHand;
        var limit = args.TryGetValue("limit", out var arg) ? (int)arg : 0;
        _startAmount = limit != 0 ?  Mathf.Min(hand.RoundMinRaiseToAmount, limit) : hand.RoundMinRaiseToAmount;
        _actualLimit = limit != 0 ? Mathf.Min(playerNChips, limit) : playerNChips;
        
        var hasOpened = hand.RoundPreviousRaiseAmount > 0;

        if (_startAmount >= playerNChips)
        {
            _amount.Value = playerNChips;
            AmountLineEdit.Editable = false;
            AmountLineEdit.Setup(new Dictionary<string, object>()
            {
                { "min", playerNChips },
                { "max", playerNChips } 
            });
            BetButtons.Hide();
            AmountBar.Hide();
        }
        else
        {
            _amount.Value = _startAmount;
            AmountLineEdit.Editable = true;
            AmountLineEdit.Setup(new Dictionary<string, object>()
            {
                { "min", _startAmount },
                { "max", _actualLimit } 
            });
            BetButtons.Show();
            AmountBar.Show();
            AmountBar.MinValue = _startAmount;
            AmountBar.MaxValue = _actualLimit;
            if (hasOpened)
            {
                foreach (var button in _unOpenedButtonsRatio.Keys)
                {
                    button.Hide();
                }
                foreach (var button in _openedButtonsRatio.Keys)
                {
                    var betAmount = (int)(potAmount * _openedButtonsRatio[button]);
                    SetupBetButton(button, betAmount);
                }
            }
            else
            {
                foreach (var button in _unOpenedButtonsRatio.Keys)
                {
                    var betAmount = (int)(bigBlindAmount * _unOpenedButtonsRatio[button]);
                    SetupBetButton(button, betAmount);
                }
                foreach (var button in _openedButtonsRatio.Keys)
                {
                    button.Hide();
                }
            }
            SetupBetButton(AllInButton, _actualLimit);
            AllInButton.Text = _actualLimit < playerNChips ? "Max" : "All In";
        }
    }
    
    private void OnAmountChanged(object sender, ValueChangedEventDetailedArgs<int> args)
    {
        AmountLineEdit.Value = args.NewValue;
        AmountBar.Value = Mathf.Clamp(args.NewValue, _startAmount, _actualLimit);
    }

    private void SetupBetButton(BaseButton button, int amount)
    {
        Action callback = () => _amount.Value = amount;
        if (_buttonPressedCallbacks.TryGetValue(button, out var pressedCallback))
        {
            button.Pressed -= pressedCallback;
        }
        button.Pressed += callback;
        button.Show();
        button.Disabled = amount > _actualLimit || amount < _startAmount;
        _buttonPressedCallbacks[button] = callback;
    }
}