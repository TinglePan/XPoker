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

    public Dictionary<CardNode, CardContainer> CardPositionBeforeResolve;
    
    public Engage(GameMgr gameMgr, CompletedHand playerHand, CompletedHand enemyHand, Enums.EngageRole tieRole = Enums.EngageRole.Attacker)
    {
        GameMgr = gameMgr;
        Battle = gameMgr.CurrentBattle;
        HandEvaluator = Battle.HandEvaluator;
        var player = Battle.Player;
        var enemy = Battle.Enemy;
        TieRole = tieRole;
        var compareRes = HandEvaluator.Compare(playerHand, enemyHand);

        switch (compareRes)
        {
            case > 0:
                player.RoundRole.Value = Enums.EngageRole.Attacker;
                enemy.RoundRole.Value = Enums.EngageRole.Defender;
                break;
            case < 0:
                player.RoundRole.Value = Enums.EngageRole.Defender;
                enemy.RoundRole.Value = Enums.EngageRole.Attacker;
                break;
            default:
                player.RoundRole.Value = Enums.EngageRole.Attacker;
                enemy.RoundRole.Value = Enums.EngageRole.Attacker;
                break;
        }

        CardPositionBeforeResolve = new Dictionary<CardNode, CardContainer>();
    }
    
    public async Task Resolve()
    {
        var player = Battle.Player;
        var enemy = Battle.Enemy;
        if (player.RoundRole.Value == enemy.RoundRole.Value || player.RoundRole.Value == Enums.EngageRole.Defender)
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
                // await Utils.Wait(Battle, Configuration.AnimateCardTransformInterval);
            }
            await Task.WhenAll(tasks);
        }
        var tasks = new List<Task>();
        hand.Sort();
        
        tasks.Add(PrepareCards(hand.PrimaryCards, Battle.ResolveCardContainer.CardContainers[0]));
        if (hand.Kickers != null)
        {
            tasks.Add(PrepareCards(hand.Kickers, Battle.ResolveCardContainer.CardContainers[1]));
        }
        await Task.WhenAll(tasks);
    }

    protected async Task ResolveEntity(BattleEntity entity)
    {
        await PrepareEntity(entity, Battle.RoundHands[entity]);
        var timer = Battle.GetTree().CreateTimer(Configuration.DelayBetweenResolveSteps);
        await Battle.ToSignal(timer, Timer.SignalName.Timeout);
        var resolveCardContainer = Battle.ResolveCardContainer.CardContainers[0];
        foreach (var cardNode in resolveCardContainer.ContentNodes)
        {
            await cardNode.TweenSelect(true, Configuration.SelectTweenTime);
            var card = cardNode.Content.Value;
            card.Resolve(Battle, this, entity);
            await GameMgr.BattleLog.HandleLogEntries();
        }
        // await Battle.RoleMarkers[entity].TweenEmphasize(false, Configuration.EmphasizeTweenTime);

    }

    protected async Task ClearResolveArea()
    {
        async Task ClearCardContainer(CardContainer targetContainer)
        {
            var tasks = new List<Task>();
            foreach (var cardNode in targetContainer.ContentNodes.ToList())
            {
                tasks.Add(targetContainer.MoveCardNodeToContainer(cardNode, CardPositionBeforeResolve[cardNode]));
                cardNode.IsSelected = false;
                // await Utils.Wait(Battle, Configuration.AnimateCardTransformInterval);
            }
            await Task.WhenAll(tasks);
        }
        
        var tasks = new List<Task>();
        
        foreach (var cardContainer in Battle.ResolveCardContainer.CardContainers)
        {
            tasks.Add(ClearCardContainer(cardContainer));
        }
        await Task.WhenAll(tasks);
    }
}