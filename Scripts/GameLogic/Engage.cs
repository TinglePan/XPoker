using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Godot;
using XCardGame.Scripts.Buffs;
using XCardGame.Scripts.Cards;
using XCardGame.Scripts.Cards.SkillCards;
using XCardGame.Scripts.Common.Constants;
using XCardGame.Scripts.Effects;
using XCardGame.Scripts.HandEvaluate;
using XCardGame.Scripts.Nodes;

namespace XCardGame.Scripts.GameLogic;

public class Engage
{
    public GameMgr GameMgr;
    public Battle Battle;
    public SkillDisplay SkillDisplay;
    public CompletedHandEvaluator HandEvaluator;
    
    public CompletedHand PlayerHand;
    public CompletedHand EnemyHand;
    public Enums.EngageRole TieRole;
    
    public Enums.EngageRole PlayerRole;
    public Enums.EngageRole EnemyRole;
    
    public Engage(GameMgr gameMgr, CompletedHand playerHand, CompletedHand enemyHand, Enums.EngageRole tieRole = Enums.EngageRole.Attacker)
    {
        GameMgr = gameMgr;
        Battle = gameMgr.CurrentBattle;
        SkillDisplay = Battle.SkillDisplay;
        HandEvaluator = Battle.HandEvaluator;
        PlayerHand = playerHand;
        EnemyHand = enemyHand;
        TieRole = tieRole;
        var compareRes = HandEvaluator.Compare(playerHand, enemyHand);
        if (compareRes > 0)
        {
            PlayerRole = Enums.EngageRole.Attacker;
            EnemyRole = Enums.EngageRole.Defender;
        }
        else if (compareRes < 0)
        {
            PlayerRole = Enums.EngageRole.Defender;
            EnemyRole = Enums.EngageRole.Attacker;
        }
        else
        {
            PlayerRole = TieRole;
            EnemyRole = TieRole;
        }
    }

    public void PrepareRoundSkills()
    {
        void PrepareEntityRoundSkills(BattleEntity entity, CompletedHand hand, Enums.EngageRole role,
            CardContainer roundSkillContainer)
        {
            int index = 0;
            foreach (var cardNode in entity.SkillCardContainer.ContentNodes)
            {
                var skillCard = (BaseSkillCard)cardNode.Content.Value;
                if (skillCard.TriggerHandTier == hand.Tier &&
                    skillCard.Contents.TryGetValue(role, out var content))
                {
                    SkillDisplay.PrepareRoundSkill(cardNode, roundSkillContainer, Configuration.AnimateCardTransformInterval * index);
                }
                index++;
            }
        }
        
        PrepareEntityRoundSkills(Battle.Player, PlayerHand, PlayerRole, SkillDisplay.PlayerRoundSkillContainer);
        PrepareEntityRoundSkills(Battle.Enemy, EnemyHand, EnemyRole, SkillDisplay.EnemyRoundSkillContainer);
    }

    public void Resolve()
    {
        void ResolveEntityRoundSkills(BattleEntity entity, CompletedHand hand, Enums.EngageRole role)
        {
            var skillResolver = new SkillResolver(GameMgr, this);
            var self = entity;
            var opponent = self == Battle.Player ? Battle.Enemy : Battle.Player;
            foreach (var cardNode in entity.SkillCardContainer.ContentNodes)
            {
                var skillCard = (BaseSkillCard)cardNode.Content.Value;
                if (skillCard.TriggerHandTier == hand.Tier &&
                    skillCard.Contents.TryGetValue(role, out var content))
                {
                    skillResolver.Resolve(skillCard, content, hand, self, opponent);
                }
            }
        }
        if (PlayerRole == EnemyRole || PlayerRole == Enums.EngageRole.Defender)
        {
            ResolveEntityRoundSkills(Battle.Player, PlayerHand, PlayerRole);
            ResolveEntityRoundSkills(Battle.Enemy, EnemyHand, EnemyRole);
        } else
        {
            ResolveEntityRoundSkills(Battle.Enemy, EnemyHand, EnemyRole);
            ResolveEntityRoundSkills(Battle.Player, PlayerHand, PlayerRole);
        }
    }
}