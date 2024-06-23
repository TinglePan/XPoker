using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Godot;
using XCardGame.Scripts.Cards.SkillCards;
using XCardGame.Scripts.Common;
using XCardGame.Scripts.GameLogic;
using XCardGame.Scripts.Nodes;

namespace XCardGame.Scripts.InputHandling;

public class PrepareRoundSkillInputHandler: BaseInputHandler
{

    public Battle Battle;
    public BattleEntity Player;
    public CardContainer SkillCardContainer;
    public BaseButton ProceedButton;

    public List<CardNode> SelectedSkillCardNodes;

    public int SelectedSkillCardBlackJackValueSum;
    
    public PrepareRoundSkillInputHandler(GameMgr gameMgr) : base(gameMgr)
    {
        SelectedSkillCardNodes = new List<CardNode>();
    }
    
    public override void OnEnter()
    {
        base.OnEnter();
        Battle = GameMgr.CurrentBattle;
        ProceedButton = Battle.ProceedButton;
        Player = Battle.Player;
        SkillCardContainer = Player.SkillCardContainer;
        foreach (var node in SkillCardContainer.ContentNodes)
        {
            node.OnPressed += OnCardNodePressed;
        }

        ProceedButton.Pressed += Confirm;
    }

    // protected void AddSkillCard()
    // {
    //     var playerSkillCardContainer = GameMgr.CurrentBattle.Player.SkillCardContainer;
    //     playerSkillCardContainer.Contents.Add(new DualWieldSkillCard(Enums.CardSuit.Spades, Enums.CardRank.Two, GameMgr.CurrentBattle.Player));
    // }
    
    public override void OnExit()
    {
        base.OnExit();
        foreach (var node in SkillCardContainer.ContentNodes)
        {
            node.OnPressed -= OnCardNodePressed;
        }
        var skillDisplay = Battle.SkillDisplay;
        var engage = Battle.RoundEngage;
        engage.PrepareEntityRoundSkills(Player, engage.Hands[Player], engage.Roles[Player], skillDisplay.PlayerRoundSkillContainer);
        ProceedButton.Pressed -= Confirm;
    }
    
    protected void OnCardNodePressed(CardNode node)
    {
        if (node.Content.Value is BaseSkillCard card)
        {
            if (SelectedSkillCardNodes.Contains(node))
            {
                SelectedSkillCardNodes.Remove(node);
                SelectedSkillCardBlackJackValueSum -= Utils.GetCardBlackJackValue(card.Rank.Value).Min();
            }
            else if (CanUseSkillCard(card, out var blackJackValue))
            {
                SelectedSkillCardNodes.Add(node);
                SelectedSkillCardBlackJackValueSum += blackJackValue;
                node.AnimateSelectWithOrder(SelectedSkillCardNodes.Count);
            }
        }
    }

    protected bool CanUseSkillCard(BaseSkillCard skillCardToUse, out int blackJackValue)
    {
        blackJackValue = Utils.GetCardBlackJackValue(skillCardToUse.Rank.Value).Min();
        return blackJackValue + SelectedSkillCardBlackJackValueSum <= Player.Speed;
    }

    protected void Confirm()
    {
        GameMgr.InputMgr.QuitCurrentInputHandler();
        Battle.Proceed();
    }
}