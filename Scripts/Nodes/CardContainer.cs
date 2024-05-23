using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using Godot;
using XCardGame.Scripts.Cards;
using XCardGame.Scripts.Common.Constants;

namespace XCardGame.Scripts.Nodes;

public partial class CardContainer: BaseContentContainer<CardNode, BaseCard>
{
    [Export]
    public PackedScene CardPrefab;

    public Enums.CardFace DefaultDealtCardFaceDirection;
    public Func<int, Enums.CardFace> GetCardFaceDirectionFunc;

    public bool AllowEffect;
    
    public override void _Ready()
    {
        base._Ready();
        ClearChildren();
    }

    public override void Setup(Dictionary<string, object> args)
    {
        base.Setup(args);
        if (args["cards"] is ObservableCollection<BaseCard> cards && cards != Contents)
        {
            Contents = cards;
            Contents.CollectionChanged += OnContentsChanged;
        }

        Debug.Assert(args.ContainsKey("defaultDealtCardFaceDirection") || args.ContainsKey("getCardFaceDirectionFunc"));
        if (args.TryGetValue("defaultDealtCardFaceDirection", out var arg))
        {
            DefaultDealtCardFaceDirection = (Enums.CardFace)arg;
        }
        if (args.TryGetValue("getCardFaceDirectionFunc", out arg))
        {
            GetCardFaceDirectionFunc = (Func<int, Enums.CardFace>)arg;
        }
    }

    public override async void OnM2VAddContents(int startingIndex, IList contents)
    {
        EnsureSetup();
        var index = startingIndex;
        foreach (var content in contents)
        {
            if (content is BaseCard card)
            {
                var cardNode = CardPrefab.Instantiate<CardNode>();
                await AddContentNode(index, cardNode);
                var faceDirection = GetCardFaceDirectionFunc?.Invoke(index) ?? DefaultDealtCardFaceDirection;
                cardNode.Setup(new Dictionary<string, object>()
                {
                    { "card", card },
                    { "container", this },
                    { "faceDirection", faceDirection }
                });
                index++;
            }
        }
    }
}