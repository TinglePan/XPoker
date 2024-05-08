using XCardGame.Scripts.Cards.PokerCards;
using XCardGame.Scripts.HandEvaluate;
using XCardGame.Scripts.Ui;

namespace XCardGame.Scripts.GameLogic;

public interface IGameEventTriggered
{

    public void AfterAddedToContainer(Battle battle, CardContainer container);

    public void BeforeDealCard(Battle battle, BattleEntity entity);

    public void AfterDealCard(Battle battle, BattleEntity entity);

    public void OnRoundStart(Battle battle);

    public void OnRoundEnd(Battle battle);

    public void BeforeShowDown(Battle battle);

    public void AfterShowDown(Battle battle);
    
    public void OnHoleCardChanged(Battle battle, BattleEntity entity, int index, BasePokerCard from, BasePokerCard to);

    public void OnCommunityCardChanged(Battle battle, int index, BasePokerCard from, BasePokerCard to);
}