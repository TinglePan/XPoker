using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using Godot;
using XCardGame.Common;
using XCardGame.Ui;

namespace XCardGame;

public partial class StandaloneCardPile: BaseCardPile
{
    public new class SetupArgs: BaseCardPile.SetupArgs
    {
        public Enums.CardFace TopCardFaceDirection;
    }
    
    public Enums.CardFace TopCardFaceDirection;

    public override void _Ready()
    {
        base._Ready();
        TopCardNode = GetNode<CardNode>("TopCard");
    }

    public override void Setup(object o)
    {
        base.Setup(o);
        var args = (SetupArgs)o;
        
        TopCardFaceDirection = args.TopCardFaceDirection;
        TopCardNode.Setup(new CardNode.SetupArgs
        {
            FaceDirection = args.TopCardFaceDirection,
            HasPhysics = true,
        });
    }

    protected override void Adjust()
    {
        if (Cards.Count > 0)
        {
            PileImage.Show();
            TopCardNode.Show();
            TopCardNode.Content.Value = Cards[0];
            TopCardNode.Position = GetTopCardOffset(Cards.Count);
        }
        else
        {
            PileImage.Hide();
            TopCardNode.Hide();
        }
    }
    
    protected Vector2 GetTopCardOffset(int count)
    {
        return Configuration.PiledCardOffsetMax * Mathf.Clamp((float)count / Configuration.PileCardCountAtMaxOffset, 0, 1);
    }
}