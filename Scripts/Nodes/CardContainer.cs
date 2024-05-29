using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using Godot;
using XCardGame.Scripts.Cards;
using XCardGame.Scripts.Common.Constants;
using XCardGame.Scripts.GameLogic;

namespace XCardGame.Scripts.Nodes;

public partial class CardContainer: ContentContainer<CardNode, BaseCard>
{
	[Export]
	public PackedScene CardPrefab;

	public Enums.CardFace DefaultCardFaceDirection;
	public Func<int, Enums.CardFace> GetCardFaceDirectionFunc;

	public bool AllowInteract;
	
	protected Battle Battle;
	
	public override void _Ready()
	{
		base._Ready();
		ClearChildren();
	}

	public override void Setup(Dictionary<string, object> args)
	{
		base.Setup(args);
		Battle = GameMgr.CurrentBattle;
		AllowInteract = (bool)args["allowInteract"];
		if (args["cards"] is ObservableCollection<BaseCard> cards && cards != Contents)
		{
			Contents = cards;
			Contents.CollectionChanged += OnContentsChanged;
		}
		Debug.Assert(args.ContainsKey("defaultCardFaceDirection") || args.ContainsKey("getCardFaceDirectionFunc"));
		if (args.TryGetValue("defaultCardFaceDirection", out var arg))
		{
			DefaultCardFaceDirection = (Enums.CardFace)arg;
		}
		if (args.TryGetValue("getCardFaceDirectionFunc", out arg))
		{
			GetCardFaceDirectionFunc = (Func<int, Enums.CardFace>)arg;
		}
	}

	public override void AddContentNode(int index, CardNode node, float tweenTime = 0f)
	{
		base.AddContentNode(index, node, tweenTime);
		node.OriginalFaceDirection = GetCardFaceDirection(index);
		node.FaceDirection.Value = node.OriginalFaceDirection;
	}

	public override void OnM2VAddContents(int startingIndex, IList contents)
	{
		EnsureSetup();
		var index = startingIndex;
		foreach (var content in contents)
		{
			if (content is BaseCard card && (index >= ContentNodes.Count || ContentNodes[index].Content.Value != content))
			{
				var cardNode = CardPrefab.Instantiate<CardNode>();
				AddChild(cardNode);
				var faceDirection = GetCardFaceDirection(index);
				cardNode.Setup(new Dictionary<string, object>()
				{
					{ "card", card },
					{ "container", this },
					{ "faceDirection", faceDirection }
				});
				card.Setup(new Dictionary<string, object>()
				{
					{ "gameMgr", GameMgr },
					{ "battle", Battle },
					{ "node", cardNode }
				});
				AddContentNode(index, cardNode);
				index++;
			}
		}
	}

	protected Enums.CardFace GetCardFaceDirection(int index)
	{
		return GetCardFaceDirectionFunc?.Invoke(index) ?? DefaultCardFaceDirection;
	}
}
