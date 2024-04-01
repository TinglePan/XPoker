using System.Collections.Generic;

namespace XCardGame.Scripts;

public interface ISetup
{
    public void Setup(Dictionary<string, object> args);
}