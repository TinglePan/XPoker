using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using Godot;
using XCardGame.Scripts.Cards;
using XCardGame.Scripts.Common;
using XCardGame.Scripts.Common.Constants;
using XCardGame.Scripts.GameLogic;

namespace XCardGame.Scripts.Nodes;

public partial class CardContainer: ContentContainer<CardNode, BaseCard>
{
	public PackedScene CardPrefab;
	public Battle Battle;

	public Enums.CardFace DefaultCardFaceDirection;
	public Func<int, Enums.CardFace> GetCardFaceDirectionFunc;

	public bool AllowInteract;
	
	public override void _Ready()
	{
		base._Ready();
		CardPrefab = ResourceCache.Instance.Load<PackedScene>("res://Scenes/Card.tscn");
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

	public async void MoveCardNodesToContainer(CardContainer targetContainer)
	{
		var cardNodes = new List<CardNode>(ContentNodes);
		int index = 0;
		foreach (var cardNode in cardNodes)
		{
			MoveCardNodeToContainer(cardNode, targetContainer, Configuration.AnimateCardTransformInterval * index);
		}
	}
	
	public async void MoveCardNodeToContainer(CardNode skillCardNode, CardContainer targetContainer, float delay = 0f)
	{
		if (delay > 0)
		{
			var timer = GetTree().CreateTimer(delay);
			await ToSignal(timer, Timer.SignalName.Timeout);
		}
		var sourceContainer = skillCardNode.Container;
		sourceContainer.ContentNodes.Remove(skillCardNode);
		targetContainer.ContentNodes.Add(skillCardNode);
	}

	protected override void OnV2MAddNodes(int startingIndex, IList nodes)
	{
		base.OnV2MAddNodes(startingIndex, nodes);
		var index = startingIndex;
		foreach (var node in nodes)
		{
			if (node is CardNode cardNode)
			{
				cardNode.OriginalFaceDirection = GetCardFaceDirection(index);
				cardNode.FaceDirection.Value = cardNode.OriginalFaceDirection;
			}
			index++;
		}
	}

	protected override void OnM2VAddContents(int startingIndex, IList contents)
	{
		EnsureSetup();
		SuppressNotifications = true;
		var index = startingIndex;
		foreach (var content in contents)
		{
			if (content is BaseCard card)
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
				ContentNodes.Insert(index, cardNode);
			}
			index++;
		}
		for (int i = 0; i < ContentNodes.Count; i++)
		{
			AdjustContentNode(i, true);
		}
		SuppressNotifications = false;
	}

	protected Enums.CardFace GetCardFaceDirection(int index)
	{
		return GetCardFaceDirectionFunc?.Invoke(index) ?? DefaultCardFaceDirection;
	}
}
