using Godot;
using XCardGame.Scripts.Common;

namespace XCardGame.Scripts.Game;

public partial class Intro: Node
{
    public GameMgr GameMgr;
    public BaseButton StartButton;
    public BaseButton QuitButton;

    public PackedScene BattleScene;
    
    public override void _Ready()
    {
        GameMgr = GetNode<GameMgr>("/root/GameMgr");
        StartButton = GetNode<BaseButton>("MainMenu/StartButton");
        QuitButton = GetNode<BaseButton>("MainMenu/QuitButton");
        BattleScene = ResourceCache.Instance.Load<PackedScene>("res://Scenes/BattleScene.tscn");
        StartButton.Pressed += Start;
        QuitButton.Pressed += GameMgr.Quit;
        
        // NOTE: On load main scene workaround.
        if (GameMgr.SceneStack.Count == 1)
        {
            GameMgr.SceneStack.Add(this);
        }
    }

    protected void Start()
    {
        GameMgr.ChangeScene(BattleScene);
        GameMgr.StartBattle();
    }
    
}