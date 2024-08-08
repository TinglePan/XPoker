﻿using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Godot;
using XCardGame.Common;
using XCardGame.Ui;

namespace XCardGame;

public class Engage
{
    public GameMgr GameMgr;
    public Battle Battle;
    public CompletedHandEvaluator HandEvaluator;
    
    public Enums.EngageRole TieRole;

    public Dictionary<CardNode, CardContainer> CardPositionBeforeResolve;
    public List<BaseAgainstEntityEffect> PendingEffects = new List<BaseAgainstEntityEffect>();
    
    public Engage(GameMgr gameMgr, CompletedHand playerHand, CompletedHand enemyHand, Enums.EngageRole tieRole = Enums.EngageRole.Attacker)
    {
        GameMgr = gameMgr;
        Battle = gameMgr.CurrentBattle;
        HandEvaluator = Battle.HandEvaluator;
        var player = Battle.Player;
        var enemy = Battle.Enemy;
        TieRole = tieRole;
        var compareRes = HandEvaluator.Compare(playerHand, enemyHand, Battle.HandTierOrderDescend.ToList());

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
            GD.Print($"before resolve1 {Time.GetTicksMsec()}");
            await ResolveEntity(player);
            GD.Print($"after resolve1 {Time.GetTicksMsec()}");
            await ClearResolveArea();
            GD.Print($"after clear {Time.GetTicksMsec()}");
            if (!(enemy.Hp.Value <= 0 || player.Hp.Value <= 0))
            {
                await ResolveEntity(enemy);
            }
        } else
        {
            await ResolveEntity(enemy);
            await ClearResolveArea();
            if (!(enemy.Hp.Value <= 0 || player.Hp.Value <= 0))
            {
                await ResolveEntity(player);
            }
        }
    }
    
    public async Task PrepareEntity(BattleEntity entity, CompletedHand hand)
    {
        async Task PrepareCards(List<BaseCard> cards, CardContainer targetContainer)
        {
            var tasks = new List<Task>();
            if (cards != null)
            {
                foreach (var card in cards)
                {
                    var cardNode = card.Node<CardNode>();
                    var sourceContainer = (CardContainer)cardNode.CurrentContainer.Value;
                    CardPositionBeforeResolve[cardNode] = sourceContainer;
                    Debug.Assert(sourceContainer != null, "sourceContainer is supposed to be not null");
                    tasks.Add(sourceContainer.MoveCardNodeToContainer(cardNode, targetContainer));
                    // await Utils.Wait(Battle, Configuration.AnimateCardTransformInterval);
                }
            }
            await Task.WhenAll(tasks);
        }
        var tasks = new List<Task>();
        hand.Sort();
        
        tasks.Add(PrepareCards(hand.PrimaryCards, Battle.EngageCardContainer.CardContainers[0]));
        if (hand.Kickers != null)
        {
            tasks.Add(PrepareCards(hand.Kickers, Battle.EngageCardContainer.CardContainers[1]));
        }
        await Task.WhenAll(tasks);
    }

    protected async Task ResolveEntity(BattleEntity entity)
    {
        if (Battle.RoundHands[entity].Tier != Enums.HandTier.None)
        {
            await PrepareEntity(entity, Battle.RoundHands[entity]);
            var timer = Battle.GetTree().CreateTimer(Configuration.DelayBetweenResolveSteps);
            await Battle.ToSignal(timer, Timer.SignalName.Timeout);
            var resolveCardContainer = Battle.EngageCardContainer.CardContainers[0];
            foreach (var node in resolveCardContainer.ContentNodes)
            {
                var cardNode = (CardNode)node;
                await cardNode.AnimateLift(true, Configuration.SelectTweenTime);
                var card = cardNode.Card;
                card.Resolve(entity);
                await ResolvePendingEffects();
                await GameMgr.BattleLog.HandleLogEntries();
            }
        }
        else
        {
            GameMgr.BattleLog.Log($"{entity} skipped.");
        }
        // await Battle.RoleMarkers[entity].TweenEmphasize(false, Configuration.EmphasizeTweenTime);

    }

    protected async Task ClearResolveArea()
    {
        async Task ClearCardContainer(CardContainer targetContainer)
        {
            var tasks = new List<Task>();
            foreach (var node in targetContainer.ContentNodes.ToList())
            {
                var cardNode = (CardNode)node;
                tasks.Add(targetContainer.MoveCardNodeToContainer(cardNode, CardPositionBeforeResolve[cardNode]));
                // cardNode.IsSelected = false;
                // await Utils.Wait(Battle, Configuration.AnimateCardTransformInterval);
            }
            await Task.WhenAll(tasks);
        }
        
        var tasks = new List<Task>();
        
        foreach (var cardContainer in Battle.EngageCardContainer.CardContainers)
        {
            tasks.Add(ClearCardContainer(cardContainer));
        }
        await Task.WhenAll(tasks);
    }

    protected async Task ResolvePendingEffects()
    {
        var tasks = new List<Task>();
        foreach (var effect in PendingEffects)
        {
            tasks.Add(effect.Apply());
        }
        await Task.WhenAll(tasks);
        PendingEffects.Clear();
    }
}