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
using XCardGame.Scripts.Game;

namespace XCardGame.Scripts.Ui;

public partial class CardContainer: ContentContainer<CardNode, BaseCard>
{
	public PackedScene CardPrefab;
	public Battle Battle;

	public Enums.CardFace DefaultCardFaceDirection; 
	public Func<int, Enums.CardFace> GetCardFaceDirectionFunc;

	public bool AllowInteract;
	public Enums.InteractCardType ExpectedInteractCardType;
	public bool WithCardEffect;
	
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
		if (AllowInteract)
		{
			ExpectedInteractCardType = (Enums.InteractCardType)args["expectedInteractCardType"];
		}
		WithCardEffect = (bool)args["withCardEffect"];
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

	public async void MoveCardNodesToContainer(CardContainer targetContainer, float delay = 0f)
	{
		var tasks = new List<Task>();
		var cardNodes = new List<CardNode>(ContentNodes);
		foreach (var cardNode in cardNodes)
		{
			tasks.Add(MoveCardNodeToContainer(cardNode, targetContainer));
			if (delay > 0)
			{
				await Utils.Wait(this, delay);
			}
		}
		await Task.WhenAll(tasks);
	}
	
	public async Task MoveCardNodeToContainer(CardNode cardNode, CardContainer targetContainer, int index = -1)
	{
		var sourceContainer = cardNode.Container.Value;
		sourceContainer.ContentNodes.Remove(cardNode);
		if (index < 0)
		{
			targetContainer.ContentNodes.Add(cardNode);
		}
		else
		{
			targetContainer.ContentNodes.Insert(index, cardNode);
		}
		await cardNode.TweenControl.WaitComplete("transform");
	}

	protected override void OnV2MAddNodes(int startingIndex, IList nodes)
	{
		base.OnV2MAddNodes(startingIndex, nodes);
		var index = startingIndex;
		foreach (var node in nodes)
		{
			if (node is CardNode cardNode)
			{
				OnAddContentNode?.Invoke(cardNode);
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
					{ "faceDirection", faceDirection },
					{ "hasPhysics", true }
				});
				card.Setup(new Dictionary<string, object>()
				{
					{ "gameMgr", GameMgr },
					{ "battle", Battle },
					{ "node", cardNode }
				});
				OnAddContentNode?.Invoke(cardNode);
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
