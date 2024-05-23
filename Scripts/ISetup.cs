using System.Collections.Generic;

namespace XCardGame.Scripts;

public interface ISetup
{
    
    public bool HasSetup { get; set; }

    public void Setup(Dictionary<string, object> args);
    public void EnsureSetup();
}