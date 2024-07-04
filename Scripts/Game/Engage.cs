using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot;
using XCardGame.Scripts.Cards;
using XCardGame.Scripts.Common;
using XCardGame.Scripts.Common.Constants;
using XCardGame.Scripts.HandEvaluate;
using XCardGame.Scripts.Ui;

namespace XCardGame.Scripts.Game;

public class Engage
{
    public GameMgr GameMgr;
    public Battle Battle;
    public CompletedHandEvaluator HandEvaluator;
    
    public Enums.EngageRole TieRole;

    public Dictionary<BattleEntity, CompletedHand> Hands;
    public Dictionary<BattleEntity, Enums.EngageRole> Roles;
    public Dictionary<CardNode, CardContainer> CardPositionBeforeResolve;
    
    public Engage(GameMgr gameMgr, CompletedHand playerHand, CompletedHand enemyHand, Enums.EngageRole tieRole = Enums.EngageRole.Attacker)
    {
        GameMgr = gameMgr;
        Battle = gameMgr.CurrentBattle;
        HandEvaluator = Battle.HandEvaluator;
        Hands = new Dictionary<BattleEntity, CompletedHand>();
        var player = Battle.Player;
        var enemy = Battle.Enemy;
        Hands[player] = playerHand;
        Hands[enemy] = enemyHand;
        TieRole = tieRole;
        var compareRes = HandEvaluator.Compare(playerHand, enemyHand);
        Roles = new Dictionary<BattleEntity, Enums.EngageRole>();

        switch (compareRes)
        {
            case > 0:
                Roles[player] = Enums.EngageRole.Attacker;
                Roles[enemy] = Enums.EngageRole.Defender;
                break;
            case < 0:
                Roles[player] = Enums.EngageRole.Defender;
                Roles[enemy] = Enums.EngageRole.Attacker;
                break;
            default:
                Roles[player] = TieRole;
                Roles[enemy] = TieRole;
                break;
        }

        CardPositionBeforeResolve = new Dictionary<CardNode, CardContainer>();
    }
    
    public async void Resolve()
    {
        var player = Battle.Player;
        var enemy = Battle.Enemy;
        if (Roles[player] == Roles[enemy] || Roles[player] == Enums.EngageRole.Defender)
        {
            await ResolveEntity(player);
            await ClearResolveArea();
            await ResolveEntity(enemy);
        } else
        {
            await ResolveEntity(enemy);
            await ClearResolveArea();
            await ResolveEntity(player);
        }
    }
    
    public async Task PrepareEntity(BattleEntity entity, CompletedHand hand)
    {
        async Task PrepareCards(List<BaseCard> cards, CardContainer targetContainer)
        {
            var tasks = new List<Task>();
            foreach (var card in cards)
            {
                var cardNode = card.Node<CardNode>();
                var sourceContainer = (CardContainer)cardNode.Container.Value;
                CardPositionBeforeResolve[cardNode] = sourceContainer;
                tasks.Add(sourceContainer.MoveCardNodeToContainer(cardNode, targetContainer));
                await Utils.Wait(Battle, Configuration.AnimateCardTransformInterval);
            }
            await Task.WhenAll(tasks);
        }
        var tasks = new List<Task>();
        hand.Sort();

        await Task.WhenAll(tasks);

    }

    protected async Task ResolveEntity(BattleEntity entity)
    {
        await PrepareEntity(entity, Hands[entity]);
        var timer = Battle.GetTree().CreateTimer(Configuration.DelayBetweenResolveSteps);
        await Battle.ToSignal(timer, Timer.SignalName.Timeout);
        var index = 0;
    }

    protected async Task ClearResolveArea()
    {
    }
}