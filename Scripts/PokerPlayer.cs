using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Godot;
using XCardGame.Scripts.Brain;
using XCardGame.Scripts.Cards;
using XCardGame.Scripts.Common.Constants;

namespace XCardGame.Scripts;

public partial class PokerPlayer: Node, ISetup
{
    
    public Action<PokerPlayer, BaseCard> OnAddHoleCard;
    public Action<PokerPlayer, BaseCard> OnRemoveHoleCard;
    public Action<PokerPlayer, BaseCard, BaseCard> OnSwapHoleCard;
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
    public int HandBetAmount;
    public int RoundBetAmount;
    public int RoundLastBetAmount;
    public Enums.PlayerAction RoundLastAction;
    public int NChipsInHand;
    public int NChipsInPot;
    public List<BaseCard> HoleCards;
    
    public bool IsInHand => RoundLastAction != Enums.PlayerAction.Fold;
    public bool HasAllIn => NChipsInHand == 0 && RoundBetAmount > 0;
    public bool CanAct => NChipsInHand > 0 && IsInHand;

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
        
        HandBetAmount = 0;
        RoundBetAmount = 0;
        RoundLastBetAmount = 0;
        RoundLastAction = Enums.PlayerAction.None;
        
        NChipsInHand = Creature.NChips;
        NChipsInPot = 0;
        HoleCards = new List<BaseCard>();
    }
    
    public void Reset()
    {
        ResetRoundState();
        HandBetAmount = 0;
        HoleCards.Clear();
    }
    
    public void ResetRoundState()
    {
        RoundBetAmount = 0;
        RoundLastBetAmount = 0;
        RoundLastAction = Enums.PlayerAction.None;
        NChipsInPot = 0;
    }
    
    public void Call()
    {
        RoundLastAction = Enums.PlayerAction.Call;
        var amountToCall = _hand.RoundCallAmount - RoundBetAmount;
        if (amountToCall >= NChipsInHand)
        {
            amountToCall = NChipsInHand;
            EmitSignal(SignalName.AfterAllIn, this);
        }
        NChipsInHand -= amountToCall;
        RoundBetAmount = amountToCall;
        RoundLastBetAmount = amountToCall;
        NChipsInPot += amountToCall;
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
            var amountToBet = amount - RoundBetAmount;
            if (amountToBet >= NChipsInHand)
            {
                amountToBet = NChipsInHand;
            }
            NChipsInHand -= amountToBet;
            RoundBetAmount += amountToBet;
            RoundLastBetAmount = amountToBet;
            NChipsInPot += amountToBet;
        }
    }
    
    public void RaiseTo(int raiseTo)
    {
        RoundLastAction = Enums.PlayerAction.Raise;
        var amountToRaise = raiseTo - RoundBetAmount;
        if (amountToRaise >= NChipsInHand)
        {
            amountToRaise = NChipsInHand;
            OnAllIn?.Invoke(this, NChipsInHand);
            EmitSignal(SignalName.AfterAllIn, this);
        } else if (amountToRaise > NChipsInHand)
        {
            GD.PrintErr("Raise amount exceeds player's chip count. This case should be filtered out by ui.");
        }
        NChipsInHand -= amountToRaise;
        RoundBetAmount += amountToRaise;
        RoundLastBetAmount = amountToRaise;
        NChipsInPot += amountToRaise;
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
    
    public void AddHoleCard(BaseCard card)
    {
        HoleCards.Add(card);
        OnAddHoleCard?.Invoke(this, card);
    }
    
    public void RemoveHoleCard(BaseCard card)
    {
        HoleCards.Remove(card);
        OnRemoveHoleCard?.Invoke(this, card);
    }

    public void SwapHoleCard(BaseCard src, BaseCard dst)
    {
        var srcIndex = HoleCards.IndexOf(src);
        HoleCards[srcIndex] = dst;
        OnSwapHoleCard?.Invoke(this, src, dst);
    }
    
    public async Task AskForAction(Dictionary<string, object> context)
    {
        await _brain.AskForAction(context);
        // GD.Print("Player: Action done.");
    }

    public bool WillBeAllIn(int amount)
    {
        return amount >= NChipsInHand;
    }

    public override string ToString()
    {
        return Creature.ToString();
    }
}