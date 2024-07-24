namespace XCardGame.Ui;

public interface IContentContractGetIcon<out TIcon>: IContent
{
    public TIcon GetIcon();
}