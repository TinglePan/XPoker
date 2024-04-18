using Godot;
using XCardGame.Scripts.Cards;
using XCardGame.Scripts.Cards.SpecialCards;
using XCardGame.Scripts.Common.DataBinding;
using XCardGame.Scripts.InputHandling;

namespace XCardGame.Scripts.Ui;

public partial class SpecialCardNode: CardNode
{
    [Export]
    public TextureRect Icon;
    
    private BaseInputHandler _inputHandler;

    public override void _ExitTree()
    {
        base._ExitTree();
        if (_inputHandler != null)
        {
            _inputHandler.OnLMouseButtonPressed -= LeftMouseButtonPressHandler;
        }
    }

    public override void Setup(System.Collections.Generic.Dictionary<string, object> args)
    {
        base.Setup(args);
        var gameMgr = GetNode<GameMgr>("/root/GameMgr");
        var inputMgr = gameMgr.InputMgr;
        _inputHandler = inputMgr.CurrentInputHandler; 
        _inputHandler.OnLMouseButtonPressed += LeftMouseButtonPressHandler;
    }
    
    protected void LeftMouseButtonPressHandler(Vector2 position)
    {
        if (GetGlobalRect().HasPoint(position))
        {
            ((BaseSpecialCard)Card.Value).Activate();
        }
    }

    public override void OnCardChanged(object sender, ValueChangedEventDetailedArgs<BaseCard> args)
    {
        base.OnCardChanged(sender, args);
        if (args.NewValue is BaseSpecialCard specialCard)
        {
            Icon.Texture = GD.Load<Texture2D>(specialCard.IconPath);
        }
    }
}