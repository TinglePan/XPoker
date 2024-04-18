using Godot;
using Godot.Collections;

namespace XCardGame.Scripts;

[GlobalClass]
public partial class TestRes : Resource
{
    public enum TestResType {
        TestResType1,
        TestResType2,
        TestResType3
    }
    
    [Export] public Array<TestResType> ResTypes;
}