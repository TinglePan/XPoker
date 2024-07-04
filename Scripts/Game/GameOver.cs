using Godot;
using XCardGame.Scripts.Common;

namespace XCardGame.Scripts.Game;

public partial class GameOver: Node
{
    public GameMgr GameMgr;
    public BaseButton RestartButton;
    public BaseButton ReturnToTitleButton;
    public BaseButton QuitButton;

    public PackedScene BattleScene;
    public PackedScene TitleScene;
    
    public override void _Ready()
    {
        GD.Print("Ready Gameover");
        GameMgr = GetNode<GameMgr>("/root/GameMgr");
        RestartButton = GetNode<BaseButton>("MainMenu/RestartButton");
        ReturnToTitleButton = GetNode<BaseButton>("MainMenu/ReturnToTitleButton");
        QuitButton = GetNode<BaseButton>("MainMenu/QuitButton");
        BattleScene = ResourceCache.Instance.Load<PackedScene>("res://Scenes/BattleScene.tscn");
        TitleScene = ResourceCache.Instance.Load<PackedScene>("res://Scenes/Intro.tscn");
        RestartButton.Pressed += Restart;
        ReturnToTitleButton.Pressed += ReturnToTitle;
        QuitButton.Pressed += GameMgr.Quit;
    }
    
    protected void Restart()
    {
        GameMgr.ChangeScene(BattleScene);
        GameMgr.StartBattle();
    }

    protected void ReturnToTitle()
    {
        GameMgr.ChangeScene(TitleScene);
    }
}