using SharpEngineCore.Graphics;
using System.Diagnostics;

namespace SharpEngineCore.ECS.Components;

public class Camera : Component
{
    private CameraObject _state;

    public float fov = 60f;
    public float nearPlane = 0.03f;
    public float farPlane = 1000f;

    private bool _initialized;

    public override void OnExternalAwake()
    {
        CreateState();
    }

    private void CreateState()
    {
        Debug.Assert(_initialized == false);

        var transform = gameObject.Transform;

        var viewport = new Viewport(new()
        {
            Height = 1080f,
            Width = 1920f,
            MaxDepth = 1f
        });

        _state = Graphics.Graphics.CreateCameraObject(
            new CameraInfo()
            {
                cameraTransform = new CameraConstantData()
                {
                    Position = transform.Position,
                    Rotation = transform.Rotation,
                    Scale = transform.Scale,
                    Projection = CameraInfo.Perspective,
                    Attributes = new(viewport.AspectRatio,
                                     fov,
                                     nearPlane,
                                     farPlane)
                },
                viewport = viewport
            });

        _initialized = true;
    }

    public override void OnExternalDestroy()
    {
        ExpireState();
    }

    public override void Awake()
    {
        if (_initialized == false)
            CreateState();
    }

    public override void OnDestroy()
    {
        if(_initialized)
            ExpireState();
    }

    private void ExpireState()
    {
        Debug.Assert(_initialized);

        _state.SetState(State.Expired);

        _initialized = false;
    }

    public override void OnPreRender()
    {
        UpdateState();
    }

    private void UpdateState()
    {
        Debug.Assert(_initialized);

        var transform = gameObject.Transform;

        if(Input.Input.GetKey(Input.Key.Down))
        {
            transform.Position.X -= 1f;
        }

        _state.UpdateCamera(new CameraConstantData()
        {
            Projection = CameraInfo.Perspective,
            Attributes = new FColor4(
                _state.AspectRatio, fov, nearPlane, farPlane),
            Position = transform.Position,
            Rotation = transform.Rotation,
            Scale = transform.Scale
        });
    }

    public override void OnDisable()
    {
        Debug.Assert(_initialized);

        _state.SetState(State.Paused);
    }

    public override void OnEnable()
    {
        Debug.Assert(_initialized);

        _state.SetState(State.Active);
    }
}