using Godot;

namespace XCardGame.Scripts.Ui;

public interface IContentContractGetIcon<out TIcon>: IContent
{
    public TIcon GetIcon();
}