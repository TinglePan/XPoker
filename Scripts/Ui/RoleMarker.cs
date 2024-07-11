﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Godot;
using XCardGame.Scripts.Common;
using XCardGame.Scripts.Common.Constants;
using XCardGame.Scripts.Common.DataBinding;

using Battle = XCardGame.Scripts.Game.Battle;
using BattleEntity = XCardGame.Scripts.Game.BattleEntity;

namespace XCardGame.Scripts.Ui;

public partial class RoleMarker: Node2D, ISetup
{
    public Sprite2D SwordIcon;
    public Sprite2D ShieldIcon;
    public Battle Battle;
    public BattleEntity Entity;
    
    public bool HasSetup { get; set; }

    public ObservableProperty<Enums.EngageRole> Role;
    public ObservableProperty<bool> IsEmphasized;

    protected Vector2 InitPosition;

    public override void _Ready()
    {
        base._Ready();
        SwordIcon = GetNode<Sprite2D>("Sword");
        ShieldIcon = GetNode<Sprite2D>("Shield");
        Role = new ObservableProperty<Enums.EngageRole>(nameof(Role), this, Enums.EngageRole.None);
        Role.DetailedValueChanged += OnRoleChanged;
        Role.FireValueChangeEventsOnInit();
        IsEmphasized = new ObservableProperty<bool>(nameof(IsEmphasized), this, false);
        IsEmphasized.DetailedValueChanged += OnIsEmphasizedChanged;
        InitPosition = Position;
    }

    public void Setup(Dictionary<string, object> args)
    {
        Battle = (Battle)args["battle"];
        Entity = (BattleEntity)args["entity"];
        HasSetup = true;
    }
    
    public void EnsureSetup()
    {
        if (!HasSetup)
        {
            GD.PrintErr($"{this} not setup yet");
        }
    }

    public async Task TweenEmphasize(bool to, float tweenTime)
    {
        if (IsEmphasized.Value == to) return;
        var newTween = CreateTween();
        var offset = Configuration.SelectedCardOffset;
        newTween.TweenProperty(this, "position", to ? InitPosition + offset : InitPosition, tweenTime).SetTrans(Tween.TransitionType.Linear).SetEase(Tween.EaseType.Out);
        await ToSignal(newTween, Tween.SignalName.Finished);
        IsEmphasized.Value = to;
    }
    
    protected void OnRoleChanged(object sender, ValueChangedEventDetailedArgs<Enums.EngageRole> args)
    {
        if (args.NewValue == Enums.EngageRole.Attacker)
        {
            SwordIcon.Show();
            ShieldIcon.Hide();
        } else if (args.NewValue == Enums.EngageRole.Defender)
        {
            SwordIcon.Hide();
            ShieldIcon.Show();
        }
        else
        {
            SwordIcon.Hide();
            ShieldIcon.Hide();
        }
    }

    protected void OnIsEmphasizedChanged(object sender, ValueChangedEventDetailedArgs<bool> args)
    {
        if (args.NewValue)
        {
            Position = InitPosition + Configuration.SelectedCardOffset;
        }
        else
        {
            Position = InitPosition;
        }
    }

}