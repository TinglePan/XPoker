using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Godot;
using XCardGame.Scripts.Cards;
using XCardGame.Scripts.Common;
using XCardGame.Scripts.Common.Constants;
using XCardGame.Scripts.Game;

namespace XCardGame.Scripts.Ui;

public partial class CardContainer: BaseContentContainer
{
	public new class SetupArgs: BaseContentContainer.SetupArgs
	{
		public bool AllowInteract;
		public Type ExpectedInteractCardDefType;
		public Enums.CardFace DefaultCardFaceDirection;
		public Func<int, Enums.CardFace> GetCardFaceDirectionFunc;
		public bool OnlyDisplay;
		public bool ShouldCollectDealtItemAndRuleCards;
	}
	
	public PackedScene CardPrefab;
	public Battle Battle;

	public List<BaseCard> Cards => Contents.Cast<BaseCard>().ToList();
	public List<CardNode> CardNodes => ContentNodes.Cast<CardNode>().ToList();
	
	public Enums.CardFace DefaultCardFaceDirection; 
	public Func<int, Enums.CardFace> GetCardFaceDirectionFunc;

	public bool AllowInteract;
	public Type ExpectedInteractCardDefType;
	public bool ShouldCollectDealtItemAndRuleCards;
	public bool OnlyDisplay;
	
	public override void _Ready()
	{
		base._Ready();
		CardPrefab = ResourceCache.Instance.Load<PackedScene>("res://Scenes/Card.tscn");
	}

	public override void Setup(object o)
	{
		base.Setup(o);
		var args = (SetupArgs)o;
		Battle = GameMgr.CurrentBattle;
		AllowInteract = args.AllowInteract;
		OnlyDisplay = args.OnlyDisplay;
		DefaultCardFaceDirection = args.DefaultCardFaceDirection;
		GetCardFaceDirectionFunc = args.GetCardFaceDirectionFunc;
		if (AllowInteract)
		{
			ExpectedInteractCardDefType = args.ExpectedInteractCardDefType;
		}
		ShouldCollectDealtItemAndRuleCards = args.ShouldCollectDealtItemAndRuleCards;
	}

	public async void MoveCardNodesToContainer(CardContainer targetContainer, float delay = 0f)
	{
		var tasks = new List<Task>();
		var contentNodes = new List<BaseContentNode>(ContentNodes);
		foreach (var contentNode in contentNodes)
		{
			tasks.Add(MoveCardNodeToContainer((CardNode)contentNode, targetContainer));
			if (delay > 0)
			{
				await Utils.Wait(this, delay);
			}
		}
		await Task.WhenAll(tasks);
	}
	
	public async Task MoveCardNodeToContainer(CardNode cardNode, CardContainer targetContainer, int index = -1)
	{
		GD.Print($"move card node to container {cardNode} to {targetContainer} at {index}");
		var sourceContainer = cardNode.CurrentContainer.Value;
		sourceContainer.ContentNodes.Remove(cardNode);
		if (index < 0)
		{
			targetContainer.ContentNodes.Add(cardNode);
		}
		else
		{
			targetContainer.ContentNodes.Insert(index, cardNode);
		}
		await cardNode.TweenControl.WaitTransformComplete();
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
		SuppressNotifications = true;
		var index = startingIndex;
		foreach (var content in contents)
		{
			if (content is BaseCard card)
			{
				var cardNode = CardPrefab.Instantiate<CardNode>();
				AddChild(cardNode);
				var faceDirection = GetCardFaceDirection(index);
				cardNode.Setup(new CardNode.SetupArgs
				{
					Content = card,
					Container = this,
					FaceDirection = faceDirection,
					HasPhysics = true
				});
				card.Setup(new BaseCard.SetupArgs
				{
					GameMgr = GameMgr,
					Battle = Battle,
					Node = cardNode
				});
				ContentNodes.Insert(index, cardNode);
				OnAddContentNode?.Invoke(cardNode);
			}
			index++;
		}
		AdjustLayout();
		SuppressNotifications = false;
	}

	protected Enums.CardFace GetCardFaceDirection(int index)
	{
		return GetCardFaceDirectionFunc?.Invoke(index) ?? DefaultCardFaceDirection;
	}
}
