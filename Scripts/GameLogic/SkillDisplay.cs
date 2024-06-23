using System.Collections.Generic;
using System.Collections.ObjectModel;
using Godot;
using XCardGame.Scripts.Cards;
using XCardGame.Scripts.Cards.SkillCards;
using XCardGame.Scripts.Common.Constants;
using XCardGame.Scripts.Nodes;

namespace XCardGame.Scripts.GameLogic;

public partial class SkillDisplay : Node2D, ISetup
{
	public CardContainer PlayerRoundSkillContainer;
	public CardContainer EnemyRoundSkillContainer;

	public CardContainer PlayerSkillContainer;
	public CardContainer EnemySkillContainer;
	
	public bool HasSetup { get; set; }
	
	public ObservableCollection<BaseCard> PlayerRoundSkills;
	public ObservableCollection<BaseCard> EnemyRoundSkills;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		PlayerRoundSkillContainer = GetNode<CardContainer>("PlayerRoundSkills");
		EnemyRoundSkillContainer = GetNode<CardContainer>("EnemyRoundSkills");
		PlayerRoundSkills = new ObservableCollection<BaseCard>();
		EnemyRoundSkills = new ObservableCollection<BaseCard>();
		HasSetup = false;
	}
	
	public void Setup(Dictionary<string, object> args)
	{
		PlayerSkillContainer = (CardContainer)args["playerSkillCardContainer"];
		EnemySkillContainer = (CardContainer)args["enemySkillCardContainer"];
		PlayerRoundSkillContainer.Setup(new Dictionary<string, object>()
		{
			{ "allowInteract", false },
			{ "cards", PlayerRoundSkills },
			{ "contentNodeSize", Configuration.CardSize },
			{ "separation", Configuration.CardContainerSeparation },
			{ "pivotDirection", Enums.Direction2D8Ways.Neutral },
			{ "nodesPerRow", Configuration.RoundSkillsCount },
			{ "hasBorder", true },
			{ "expectedContentNodeCount", 1 },
			{ "hasName", true },
			{ "containerName", "Skill sequence" },
			{ "defaultCardFaceDirection", Enums.CardFace.Up }
		});
		EnemyRoundSkillContainer.Setup(new Dictionary<string, object>()
		{
			{ "allowInteract", false },
			{ "cards", EnemyRoundSkills },
			{ "contentNodeSize", Configuration.CardSize },
			{ "separation", Configuration.CardContainerSeparation },
			{ "pivotDirection", Enums.Direction2D8Ways.Neutral },
			{ "nodesPerRow", Configuration.RoundSkillsCount },
			{ "hasBorder", true },
			{ "expectedContentNodeCount", 1 },
			{ "hasName", true },
			{ "containerName", "Skill sequence" },
			{ "defaultCardFaceDirection", Enums.CardFace.Up }
		});
		HasSetup = true;
	}

	public void EnsureSetup()
	{
		if (!HasSetup)
		{
			GD.PrintErr( $"{this} not setup yet");
		}
	}
	
	public async void PrepareRoundSkill(CardNode skillCardNode, CardContainer targetContainer, float delay = 0f)
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

	public async void ClearUpRoundSkills(float delay = 0f)
	{
		async void ClearUpRoundSkillsOf(CardContainer roundSkillContainer, CardContainer skillContainer)
		{
			for (int i = 0; i < roundSkillContainer.ContentNodes.Count; i++)
			{
				if (delay > 0)
				{
					var timer = GetTree().CreateTimer(delay);
					await ToSignal(timer, Timer.SignalName.Timeout);
				}
				var node = roundSkillContainer.ContentNodes[0];
				roundSkillContainer.ContentNodes.Remove(node);
				skillContainer.ContentNodes.Add(node);
			}
		}
		ClearUpRoundSkillsOf(PlayerRoundSkillContainer, PlayerSkillContainer);
		ClearUpRoundSkillsOf(EnemyRoundSkillContainer, EnemySkillContainer);
	}
}