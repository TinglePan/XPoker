using System;

namespace XCardGame.Ui;

public interface ISelect
{
    public Action OnSelected { get; }
    public bool IsSelected { get; set; }
    public bool CanSelect();
    public void ToggleSelect(bool to);
}