using XCardGame.Scripts.Common.Constants;
using XCardGame.Scripts.Common.DataBinding;
using XCardGame.Scripts.GameLogic;

namespace XCardGame.Scripts.Cards.PokerCards;

public class RevealTriggeredPokerCard: BaseEventTriggeredPokerCard
{
    public Battle Battle;
    
    public RevealTriggeredPokerCard(Enums.CardSuit cardSuit, Enums.CardFace face, Enums.CardRank rank,
        BattleEntity owner = null, bool suitAsSecondComparer = false) : base(cardSuit, face, rank, owner, suitAsSecondComparer)
    {
        Face.DetailedValueChanged += OnFaceChanged;
    }
    
    public virtual void OnReveal()
    {
        
    }

    public override void AfterDealCard(Battle battle, BattleEntity entity)
    {
        Battle = battle;
        if (Face.Value == Enums.CardFace.Up)
        {
            OnReveal();
        }
    }

    protected void OnFaceChanged(object sender, ValueChangedEventDetailedArgs<Enums.CardFace> args)
    {
        if (args.NewValue == Enums.CardFace.Up)
        {
            OnReveal();
        }
    }
}