using System;
using System.Collections.Generic;

using Godot;
using XCardGame.Scripts.Cards;
using XCardGame.Scripts.Common.Constants;

namespace XCardGame.Scripts;

public partial class Player: Node, ICardHolder, ISetup
{
    [Signal]
    public delegate void BeforeBetEventHandler();
    [Signal]
    public delegate void AfterBetEventHandler(int betAmount);
    
    public List<BaseCard> HoleCards { get; private set; }
    public Action<ICardHolder, BaseCard> OnAddHoleCard { get; }
    public Action<ICardHolder, BaseCard> OnRemoveHoleCard { get; }
    public Action<ICardHolder, BaseCard, BaseCard> OnSwapHoleCard { get; }
    
    public Action<ICardHolder, BaseCard> OnFlipHoleCard { get; }

    public int NChips;
    
    public override void _Ready()
    {
        HoleCards = new List<BaseCard>();
        NChips = 0;
    }

    public void Setup(Dictionary<string, object> args)
    {
        NChips = (int)args["nChips"];
    }
    
    public void AddHoleCard(BaseCard card)
    {
        OnAddHoleCard?.Invoke(this, card);
    }
    
    public void RemoveHoleCard(BaseCard card)
    {
        OnRemoveHoleCard?.Invoke(this, card);
    }

    public void SwapHoleCard(BaseCard src, BaseCard dst)
    {
        OnSwapHoleCard?.Invoke(this, src, dst);
        OnAddHoleCard?.Invoke(this, dst);
        OnRemoveHoleCard?.Invoke(this, src);
    }

    public void FlipHoleCard(BaseCard card)
    {
        card.Face = card.Face == Enums.CardFace.Down ? Enums.CardFace.Up : Enums.CardFace.Down;
        OnFlipHoleCard?.Invoke(this, card);
    }

    public void Bet()
    {
        EmitSignal(SignalName.BeforeBet);
        EmitSignal(SignalName.AfterBet, 1);
    }
}