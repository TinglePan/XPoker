using System.Collections.ObjectModel;
using Godot;
using XCardGame.Scripts.Buffs;
using XCardGame.Scripts.Cards;
using XCardGame.Scripts.Cards.CardMarkers;

namespace XCardGame.Scripts.GameLogic;


public partial class Shop: Node2D
{
    public ObservableCollection<BaseCard> SkillCards;
    public ObservableCollection<BaseCard> AbilityCards;
    public ObservableCollection<PokerCard> PokerCards;
    public ObservableCollection<BaseCardMarker> Markers;

}