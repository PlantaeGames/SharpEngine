using SharpEngineCore.Misc;
using TerraFX.Interop.Windows;

namespace SharpEngineCore.ECS.Components;

public class Camera : Component, IDustable
{
    private bool _isDusty;

    public override void OnExternalAwake()
    {
        base.OnExternalAwake();
    }

    public override void OnExternalDestroy()
    {
        base.OnExternalDestroy();
    }

    public override void Awake()
    {
        base.Awake();
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
    }


    public void SetDusty()
    {
        _isDusty = true;
    }
}
