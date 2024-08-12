﻿using System;
using System.Threading.Tasks;
using Godot;
using XCardGame.Common;
using XCardGame.Ui;

namespace XCardGame;

public partial class SelectRewardUpgrade: Control
{
    public class SetupArgs
    {
        public int RewardCardCount;
    }
    public class SelectRewardUpgradeInputHandler : BaseInputHandler
    {
        protected Battle Battle;
        protected SelectRewardUpgrade SelectRewardUpgrade;

        public SelectRewardUpgradeInputHandler(GameMgr gameMgr, SelectRewardUpgrade selectRewardUpgrade) : base(gameMgr)
        {
            SelectRewardUpgrade = selectRewardUpgrade;
        }
        
        public override async Task OnEnter()
        {
            await base.OnEnter();
            Battle = GameMgr.CurrentBattle;
            foreach (var node in SelectRewardUpgrade.Container.ContentNodes)
            {
                node.OnMousePressed += OnNodePressed;
            }
        }
        
        public override async Task OnExit()
        {
            await base.OnExit();
            foreach (var node in SelectRewardUpgrade.Container.ContentNodes)
            {
                node.OnMousePressed -= OnNodePressed;
            }
        }

        public async void OnNodePressed(BaseContentNode node, MouseButton mouseButton)
        {
            if (mouseButton == MouseButton.Left)
            {
                var iconNode = (IconNode)node;
                await SelectRewardUpgrade.Select(iconNode);
                SelectRewardUpgrade.Quit();
            }
        }
    }
    
    public GameMgr GameMgr;
    public Battle Battle;
    
    public IconContainer Container;
    public AnimationPlayer AnimationPlayer;
    
    public Action OnQuit;

    public override void _Ready()
    {
        base._Ready();
        GameMgr = GetNode<GameMgr>("/root/GameMgr");
        AnimationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
    }

    public void Setup(object o)
    {
        Battle = GameMgr.CurrentBattle;
        Container.Setup(new BaseContentContainer.SetupArgs
        {
            ContentNodeSize = Configuration.CardSize,
            Separation = Configuration.CardContainerSeparation,
            PivotDirection = Enums.Direction2D8Ways.Neutral,
            HasName = true,
            ContainerName = Utils._("Select an upgrade..."),
            Margins = Configuration.DefaultContentContainerMargins
        });
        Container.Contents.Add(new RewardUpgradeIcon("res://Sprites/RewardIcons/max_hp.png"));
        Container.Contents.Add(new RewardUpgradeIcon("res://Sprites/RewardIcons/max_energy.png"));
        
        // Container.Contents.Add(new RewardUpgradeIcon("res://Sprites/RewardIcons/base_power.png"));
        // Container.Contents.Add(new RewardUpgradeIcon("res://Sprites/RewardIcons/pocket_size.png"));
        
        GameMgr.InputMgr.SwitchToInputHandler(new SelectRewardUpgradeInputHandler(GameMgr, this));
    }

    public async Task Select(IconNode iconNode)
    {
        // cardNode.AnimateFlip(Enums.CardFace.Down);
        // cardNode.IsBought.Value = true;
        // var card = cardNode.Card;
        // card.OwnerEntity = Battle.Player;
        // AnimationPlayer.Play("close");
        // await ToSignal(AnimationPlayer, AnimationMixer.SignalName.AnimationFinished);
    }

    public void Quit()
    {
        OnQuit?.Invoke();
        GameMgr.InputMgr.QuitCurrentInputHandler();
        GameMgr.QuitCurrentScene();
    }
}