using System.Collections.Generic;
using XCardGame.Scripts.Common.Constants;
using XCardGame.Scripts.Defs;
using XCardGame.Scripts.GameLogic;

namespace XCardGame.Scripts.Cards.AbilityCards;

public class BaseAbilityCard: BaseCard, IInteractCard
{

    public readonly AbilityCardDef AbilityCardDef;
    
    public BaseAbilityCard(AbilityCardDef def): base(def)
    {
        AbilityCardDef = def;
        Interactions = new List<Enums.CardInteractions>();
    }

    public List<Enums.CardInteractions> Interactions { get; }
    public virtual bool CanInteract()
    {
        return false;
    }

    public Enums.CardInteractions DefaultInteraction()
    {
        return Interactions.Count == 1 ? Interactions[0] : Enums.CardInteractions.None;
    }

    public virtual void Interact()
    {
        
    }
}