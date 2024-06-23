using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Godot;
using XCardGame.Scripts.Buffs;
using XCardGame.Scripts.Cards;
using XCardGame.Scripts.Cards.SkillCards;
using XCardGame.Scripts.Common;
using XCardGame.Scripts.Common.Constants;
using XCardGame.Scripts.Common.DataBinding;
using XCardGame.Scripts.Effects;
using XCardGame.Scripts.Effects.SkillEffects;
using XCardGame.Scripts.HandEvaluate;
using XCardGame.Scripts.Nodes;

namespace XCardGame.Scripts.GameLogic;

public class Engage
{
    public GameMgr GameMgr;
    public Battle Battle;
    public SkillDisplay SkillDisplay;
    public CompletedHandEvaluator HandEvaluator;
    
    public Enums.EngageRole TieRole;

    public Dictionary<BattleEntity, CompletedHand> Hands;
    public Dictionary<BattleEntity, Enums.EngageRole> Roles;
    
    public Engage(GameMgr gameMgr, CompletedHand playerHand, CompletedHand enemyHand, Enums.EngageRole tieRole = Enums.EngageRole.Attacker)
    {
        GameMgr = gameMgr;
        Battle = gameMgr.CurrentBattle;
        SkillDisplay = Battle.SkillDisplay;
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
    }
    
    public void PrepareEntityRoundSkills(BattleEntity entity, CompletedHand hand, Enums.EngageRole role,
        CardContainer roundSkillContainer)
    {
        int index = 0;
        foreach (var cardNode in entity.SkillCardContainer.ContentNodes)
        {
            var skillCard = (BaseSkillCard)cardNode.Content.Value;
            if (skillCard.CanTrigger(role, hand))
            {
                SkillDisplay.PrepareRoundSkill(cardNode, roundSkillContainer, Configuration.AnimateCardTransformInterval * index);
            }
            index++;
        }
    }

    public void PrepareRoundSkills()
    {
        // PrepareEntityRoundSkills(Battle.Player, PlayerHand, PlayerRole, SkillDisplay.PlayerRoundSkillContainer);
        var enemy = Battle.Enemy;
        PrepareEntityRoundSkills(enemy, Hands[enemy], Roles[enemy], SkillDisplay.EnemyRoundSkillContainer);
    }

    public void Resolve()
    {
        void ResolveEntityRoundSkills(BattleEntity entity, CompletedHand hand, Enums.EngageRole role)
        {
            var skillResolver = new SkillResolver(GameMgr, this);
            var opponent = entity == Battle.Player ? Battle.Enemy : Battle.Player;
            if (entity.SkillCardContainer.ContentNodes.Count > 0)
            {
                foreach (var cardNode in entity.SkillCardContainer.ContentNodes)
                {
                    var skillCard = (BaseSkillCard)cardNode.Content.Value;
                    if (skillCard.CanTrigger(role, hand))
                    {
                        skillResolver.Resolve(skillCard, role, hand, entity, opponent);
                    }
                }
            }
            else if (role == Enums.EngageRole.Attacker)
            {
                var defaultAttackEffect = new DamageSkillEffect(Battle, null, hand.Tier, 0, 1);
                defaultAttackEffect.Resolve(skillResolver, hand, entity, opponent);
            }
        }

        var player = Battle.Player;
        var enemy = Battle.Enemy;
        if (Roles[player] == Roles[enemy] || Roles[player] == Enums.EngageRole.Defender)
        {
            ResolveEntityRoundSkills(player, Hands[player], Roles[player]);
            ResolveEntityRoundSkills(enemy, Hands[enemy], Roles[enemy]);
        } else
        {
            ResolveEntityRoundSkills(enemy, Hands[enemy], Roles[enemy]);
            ResolveEntityRoundSkills(player, Hands[player], Roles[player]);
        }
    }

    public bool IsCrit(ObservableCollection<BaseCard> skillCards, int targetValue)
    {
        bool CountCard(int i, int sum)
        {
            if (i >= skillCards.Count)
            {
                if (sum != targetValue) return false;
                return true;
            }
            if (sum >= targetValue)
            {
                return false;
            }
            var card = skillCards[i];

            var cardBlackJackValues = Utils.GetCardBlackJackValue(card.Rank.Value);
            
            foreach (var rankValue in cardBlackJackValues)
            {
                if (CountCard(i + 1, sum + rankValue))
                {
                    return true;
                }
            }
            return false;
        }
        return CountCard(0, 0);
    }

    public int GetBestSum(ObservableCollection<BaseCard> skillCards, int targetValue)
    {
        int CountCard(int i, int sum, int bestSum)
        {
            if (i >= skillCards.Count)
            {
                if (sum <= targetValue && sum > bestSum) return sum;
                return bestSum;
            }
            if (sum >= targetValue)
            {
                return bestSum;
            }
            var card = skillCards[i];

            var cardBlackJackValues = Utils.GetCardBlackJackValue(card.Rank.Value);
            
            foreach (var rankValue in cardBlackJackValues)
            {
                bestSum = CountCard(i + 1, sum + rankValue, bestSum);
            }

            return bestSum;
        }
        return CountCard(0, 0, 0);
    }
}