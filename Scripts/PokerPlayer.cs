using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Godot;
using XCardGame.Scripts.Brain;
using XCardGame.Scripts.Cards;
using XCardGame.Scripts.Cards.PokerCards;
using XCardGame.Scripts.Cards.SpecialCards;
using XCardGame.Scripts.Common.Constants;
using XCardGame.Scripts.Common.DataBinding;

namespace XCardGame.Scripts;

public partial class PokerPlayer: Node, ISetup
{
    
    public Action<PokerPlayer, BasePokerCard> OnAddHoleCard;
    public Action<PokerPlayer, BasePokerCard> OnRemoveHoleCard;
    public Action<PokerPlayer, BasePokerCard, BasePokerCard> OnSwapHoleCard;
    public Action<PokerPlayer, BaseSpecialCard> OnAddSpecialCard;
    public Action<PokerPlayer, BaseSpecialCard> OnRemoveSpecialCard;
    public Action<PokerPlayer> OnFold;
    public Action<PokerPlayer> OnCheck;
    public Action<PokerPlayer, int> OnCall;
    public Action<PokerPlayer, int> OnRaise;
    public Action<PokerPlayer, int> OnAllIn;
    
    [Signal]
    public delegate void BeforeActionEventHandler(PokerPlayer p);
    [Signal]
    public delegate void AfterFoldEventHandler(PokerPlayer p);
    [Signal]
    public delegate void AfterCheckEventHandler(PokerPlayer p);
    [Signal]
    public delegate void AfterCallEventHandler(PokerPlayer p, int amountToCall);
    [Signal]
    public delegate void AfterRaiseEventHandler(PokerPlayer p, int amountToRaise);
    [Signal]
    public delegate void AfterActionEventHandler(PokerPlayer p);
    [Signal]
    public delegate void AfterAllInEventHandler(PokerPlayer p, int amount);
    
    private Hand _hand;
    private BaseBrain _brain;
    public Creature Creature;
    public int PositionIndex;
    public ObservableProperty<int> RoundBetAmount;
    public int RoundLastBetAmount;
    public Enums.PlayerAction RoundLastAction;
    public ObservableProperty<int> NChipsInHand;
    public ObservableProperty<int> NChipsInPot;
    public ObservableCollection<BasePokerCard> HoleCards;
    public ObservableCollection<BaseSpecialCard> SpecialCards;
    
    public bool IsInHand => RoundLastAction != Enums.PlayerAction.Fold;
    public bool HasAllIn => NChipsInHand.Value == 0 && RoundBetAmount.Value > 0;
    public bool CanAct => NChipsInHand.Value > 0 && IsInHand;

    public void Setup(Dictionary<string, object> args)
    {
        Creature = (Creature)args["creature"];
        _hand = (Hand)args["hand"];
        
        var brainScriptPath = (string)args["brainScriptPath"];
        var brainScript = GD.Load<Script>(brainScriptPath);
        var brainNode = new Node();
        ulong objId = brainNode.GetInstanceId();
        brainNode.SetScript(brainScript);
        brainNode = InstanceFromId(objId) as Node;
        AddChild(brainNode);
        _brain = brainNode as BaseBrain;
        _brain?.Setup(new Dictionary<string, object>()
        {
            {"player", this},
        });
        
        PositionIndex = _hand.Players.IndexOf(this);
        
        RoundBetAmount = new ObservableProperty<int>(nameof(RoundBetAmount), 0);
        RoundLastBetAmount = 0;
        RoundLastAction = Enums.PlayerAction.None;
        
        NChipsInHand = new ObservableProperty<int>(nameof(NChipsInHand), Creature.NChips);
        NChipsInPot = new ObservableProperty<int>(nameof(NChipsInPot), 0);
        HoleCards = new ObservableCollection<BasePokerCard>();
        SpecialCards = new ObservableCollection<BaseSpecialCard>();
    }
    
    public void ResetHandState()
    {
        ResetRoundState();
        HoleCards.Clear();
        NChipsInPot.Value = 0;
    }
    
    public void ResetRoundState()
    {
        RoundBetAmount.Value = 0;
        RoundLastBetAmount = 0;
        RoundLastAction = Enums.PlayerAction.None;
    }
    
    public void Call()
    {
        RoundLastAction = Enums.PlayerAction.Call;
        var amountToCall = _hand.RoundCallAmount - RoundBetAmount.Value;
        if (amountToCall >= NChipsInHand.Value)
        {
            amountToCall = NChipsInHand.Value;
            EmitSignal(SignalName.AfterAllIn, this);
        }
        NChipsInHand.Value -= amountToCall;
        RoundBetAmount.Value = amountToCall;
        RoundLastBetAmount = amountToCall;
        NChipsInPot.Value += amountToCall;
        OnCall?.Invoke(this, amountToCall);
        EmitSignal(SignalName.AfterCall, this, amountToCall);
        EmitSignal(SignalName.AfterAction, this);
    }

    public void BetBlind(int amount, bool isSmallBlind)
    {
        if (!isSmallBlind)
        {
            RaiseTo(amount);
        }
        else
        {
            RoundLastAction = Enums.PlayerAction.None;
            var amountToBet = amount - RoundBetAmount.Value;
            if (amountToBet >= NChipsInHand.Value)
            {
                amountToBet = NChipsInHand.Value;
            }
            NChipsInHand.Value -= amountToBet;
            RoundBetAmount.Value += amountToBet;
            RoundLastBetAmount = amountToBet;
            NChipsInPot.Value += amountToBet;
            OnRaise?.Invoke(this, amountToBet);
            EmitSignal(SignalName.AfterRaise, this);
        }
    }
    
    public void RaiseTo(int raiseTo)
    {
        RoundLastAction = Enums.PlayerAction.Raise;
        var amountToRaise = raiseTo - RoundBetAmount.Value;
        if (amountToRaise >= NChipsInHand.Value)
        {
            amountToRaise = NChipsInHand.Value;
            OnAllIn?.Invoke(this, NChipsInHand.Value);
            EmitSignal(SignalName.AfterAllIn, this);
        } else if (amountToRaise > NChipsInHand.Value)
        {
            GD.PrintErr("Raise amount exceeds player's chip count. This case should be filtered out by ui.");
        }
        NChipsInHand.Value -= amountToRaise;
        RoundBetAmount.Value += amountToRaise;
        RoundLastBetAmount = amountToRaise;
        NChipsInPot.Value += amountToRaise;
        OnRaise?.Invoke(this, amountToRaise);
        EmitSignal(SignalName.AfterRaise, this);
        EmitSignal(SignalName.AfterAction, this);
    }
    
    public void Fold()
    {
        RoundLastAction = Enums.PlayerAction.Fold;
        OnFold?.Invoke(this);
        EmitSignal(SignalName.AfterAction, this);
    }
    
    public void Check()
    {
        RoundLastAction = Enums.PlayerAction.Check;
        OnCheck?.Invoke(this);
        EmitSignal(SignalName.AfterAction, this);
    }

    public void SwapHoleCard(BasePokerCard src, BasePokerCard dst)
    {
        GD.Print($"Swap hold card: {src} for {dst}");
        var srcIndex = HoleCards.IndexOf(src);
        HoleCards[srcIndex] = dst;
    }
    
    public async Task AskForAction(Dictionary<string, object> context)
    {
        await _brain.AskForAction(context);
        // GD.Print("Player: Action done.");
    }

    public bool WillBeAllIn(int amount)
    {
        return amount >= NChipsInHand.Value;
    }

    public override string ToString()
    {
        return Creature.ToString();
    }
}