using SharpEngineCore.Misc;

namespace SharpEngineCore.ECS.Components;

public class Camera : Component, IDustable
{
    private bool _isDusty;




    public void SetDusty()
    {
        _isDusty = true;
    }
}
