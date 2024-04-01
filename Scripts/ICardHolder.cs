using System;
using System.Collections.Generic;
using Godot;
using XCardGame.Scripts.Cards;
using XCardGame.Scripts.Common.Constants;

namespace XCardGame.Scripts;

public interface ICardHolder
{
    public List<BaseCard> HoleCards { get; }
    public Action<ICardHolder, BaseCard> OnAddHoleCard { get; }
    public Action<ICardHolder, BaseCard> OnRemoveHoleCard { get; }
    public Action<ICardHolder, BaseCard, BaseCard> OnSwapHoleCard { get; }
    public Action<ICardHolder, BaseCard> OnFlipHoleCard { get; }
    
    public void AddHoleCard(BaseCard card);
    public void SwapHoleCard(BaseCard src, BaseCard dst);
}