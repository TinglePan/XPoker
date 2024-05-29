using System.Collections.Generic;
using XCardGame.Scripts.Cards;
using XCardGame.Scripts.Cards.AbilityCards;
using XCardGame.Scripts.Cards.SkillCards;


namespace XCardGame.Scripts.GameLogic;

public class LevelUpInfo
{
    public int Cost;
    public int Hp;

    public List<BaseCard> GrantCards;
    
    public LevelUpInfo(int cost, int Hp, List<BaseCard> grantCards)
    {
        Cost = cost;
        Hp = Hp;
        GrantCards = grantCards;
    }
}