using System;
using System.Collections.Generic;
using Godot;
using XCardGame.Scripts.Common.DataBinding;

namespace XCardGame.Scripts.Ui;


public partial class RaiseUi: BaseUi, ISetup
{
    [Signal]
    public delegate void ExitEventHandler(int amount);
    
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
    
    [Export] public BaseButton ConfirmButton;
    [Export] public BaseButton CancelButton;

    private int _startAmount;
    // private int _potAmount;
    // private int _playerNChips;
    // private int _limit;
    
    private int _actualLimit;
    
    private ObservableProperty<int> _amount;
    private Hand _hand;

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
        ConfirmButton.Pressed += () => EmitSignal(SignalName.Exit, _amount.Value);
        CancelButton.Pressed += () => EmitSignal(SignalName.Exit, 0);
    }

    public void Setup(Dictionary<string, object> args)
    {
        _hand = (Hand)args["hand"];
        var player = (PokerPlayer)args["player"];
        var bigBlindAmount = _hand.BigBlindAmount;
        var potAmount = _hand.Pot.Total;
        var canRaiseToAmount = player.NChipsInHand.Value + player.RoundBetAmount.Value;
        var limit = args.TryGetValue("limit", out var arg) ? (int)arg : 0;
        _startAmount = limit != 0 ? Mathf.Min(_hand.RoundMinRaiseToAmount, limit) : _hand.RoundMinRaiseToAmount;
        _actualLimit = limit != 0 ? Mathf.Min(canRaiseToAmount, limit) : canRaiseToAmount;
        
        var hasOpened = _hand.RoundPreviousRaiseAmount > 0;

        if (_startAmount >= canRaiseToAmount)
        {
            _amount.Value = canRaiseToAmount;
            AmountLineEdit.Editable = false;
            AmountLineEdit.Setup(new Dictionary<string, object>()
            {
                { "min", canRaiseToAmount },
                { "max", canRaiseToAmount } 
            });
            BetButtons.Hide();
            AmountBar.Hide();
        }
        else
        {
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
            _amount.Value = _startAmount;
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
            Action callback = () => _amount.Value = _actualLimit;
            if (_buttonPressedCallbacks.TryGetValue(AllInButton, out var pressedCallback))
            {
                AllInButton.Pressed -= pressedCallback;
            }
            AllInButton.Pressed += callback;
            AllInButton.Show();
            AllInButton.Text = _actualLimit < canRaiseToAmount ? "Max" : "All In";
            _buttonPressedCallbacks[AllInButton] = callback;
        }
    }
    
    private void OnAmountChanged(object sender, ValueChangedEventDetailedArgs<int> args)
    {
        AmountLineEdit.Value = args.NewValue;
        AmountBar.Value = args.NewValue;
        foreach (var button in _unOpenedButtonsRatio.Keys)
        {
            var betAmount = (int)(_hand.BigBlindAmount * _unOpenedButtonsRatio[button]);
            button.Disabled = _amount.Value + betAmount > _actualLimit;
        }
        foreach (var button in _openedButtonsRatio.Keys)
        {
            var betAmount = (int)(_hand.Pot.Total * _openedButtonsRatio[button]);
            button.Disabled = _amount.Value + betAmount > _actualLimit;
        }
    }

    private void SetupBetButton(BaseButton button, int amount)
    {
        Action callback = () => _amount.Value = Mathf.Min(amount + _amount.Value, _actualLimit);
        if (_buttonPressedCallbacks.TryGetValue(button, out var pressedCallback))
        {
            button.Pressed -= pressedCallback;
        }
        button.Pressed += callback;
        button.Show();
        button.Disabled = _amount.Value + amount > _actualLimit;
        _buttonPressedCallbacks[button] = callback;
    }
}