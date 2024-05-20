using System.Diagnostics;

namespace SharpEngineCore.Graphics;

internal abstract class RenderPipeline : PipelineEvents
{
    protected readonly Texture2D _outputTexture;
    protected RenderPass[] _renderPasses = [];

    protected List<GraphicsObject> _graphicsObjects = new();
    protected List<LightObject> _lightObjects = new();
    protected List<CameraObject> _cameraObjects = new();

    private List<GraphicsObject> _pausedGraphicsObjects = new();
    private List<LightObject> _pausedLightObjects = new();
    private List<CameraObject> _pausedCameraObjects = new();

    public abstract GraphicsObject CreateGraphicsObject(Device device, Material material,
                                                        Mesh mesh);
    public abstract CameraObject CreateCameraObject(Device device, 
                                           CameraConstantData data, Viewport viewport);
    public abstract LightObject CreateLightObject(Device device, LightData data);
    public abstract List<GraphicsObject> GetGraphicsObjects();

#nullable enable
    protected T? Get<T>()
        where T : RenderPass
    {
        foreach (var renderPass in _renderPasses)
        {
            if (renderPass is T)
            {
                return (T)renderPass;
            }
        }

        Debug.Assert(false,
            $"Could not found {nameof(T)}.");

        return null;
    }
#nullable disable


    public sealed override void Initialize(Device device, DeviceContext context)
    {
        OnInitialize(device, context);

        foreach (var renderPass in _renderPasses)
        {
            renderPass.Initialize(device, context);
        }
    }

    public sealed override void Ready(Device device, DeviceContext context)
    {
        context.ClearState();
        ClearPendingObjects();

        OnReady(device, context);
    }

    public sealed override void Go(Device device, DeviceContext context)
    {
        OnGo(device, context);

        foreach (var renderPass in _renderPasses)
        {
            renderPass.Ready(device, context);
            renderPass.Go(device, context);
        }
    }

    private void ClearPendingObjects()
    {
        ClearLightObjects();
        ClearCameraObjects();
        ClearGraphicsObjects();

        void ClearLightObjects()
        {
            // removing expired
            var toRemove = new List<int>();
            for(var i = 0; i < _lightObjects.Count; i++)
            {
                if(_lightObjects[i].State == State.Expired)
                    toRemove.Add(i);
            }
            for(var i = 0; i <  toRemove.Count; i++)
            {
                _lightObjects.RemoveAt(toRemove[i]);
            }

            // getting paused
            var toPause = new List<int>();
            for (var i = 0; i < _lightObjects.Count; i++)
            {
                if (_lightObjects[i].State == State.Active)
                    continue;

                toPause.Add(i);
            }
            for (var i = 0; i < toPause.Count; i++)
            {
                _pausedLightObjects.Add(_lightObjects[toPause[i]]);
                _lightObjects.RemoveAt(toPause[i]);
            }

            // resuming active paused
            var toResume = new List<int>();
            for (var i = 0; i < _pausedLightObjects.Count; i++)
            {
                if (_pausedLightObjects[i].State == State.Active)
                {
                    toResume.Add(i);
                }
            }
            for(var i = 0; i < toRemove.Count; i++)
            {
                _lightObjects.Add(_pausedLightObjects[toResume[i]]);
                _pausedLightObjects.RemoveAt(toResume[i]);
            }

            // removing paused expired
            toRemove.Clear();
            for(var i = 0; i < _pausedLightObjects.Count; i++)
            {
                if (_pausedLightObjects[i].State == State.Expired)
                {
                    toRemove.Add(i);
                }
            }
            for(var i = 0; i < toRemove.Count; i++)
            {
                _pausedLightObjects.RemoveAt(toRemove[i]);
            }
        }

        void ClearCameraObjects()
        {
            // removing expired
            var toRemove = new List<int>();
            for (var i = 0; i < _cameraObjects.Count; i++)
            {
                if (_cameraObjects[i].State == State.Expired)
                    toRemove.Add(i);
            }
            for (var i = 0; i < toRemove.Count; i++)
            {
                _cameraObjects.RemoveAt(toRemove[i]);
            }

            // getting paused
            var toPause = new List<int>();
            for (var i = 0; i < _cameraObjects.Count; i++)
            {
                if (_cameraObjects[i].State == State.Active)
                    continue;

                toPause.Add(i);
            }
            for (var i = 0; i < toPause.Count; i++)
            {
                _pausedCameraObjects.Add(_cameraObjects[toPause[i]]);
                _cameraObjects.RemoveAt(toPause[i]);
            }

            // resuming active paused
            var toResume = new List<int>();
            for (var i = 0; i < _pausedCameraObjects.Count; i++)
            {
                if (_pausedCameraObjects[i].State == State.Active)
                {
                    toResume.Add(i);
                }
            }
            for (var i = 0; i < toRemove.Count; i++)
            {
                _cameraObjects.Add(_pausedCameraObjects[toResume[i]]);
                _pausedCameraObjects.RemoveAt(toResume[i]);
            }

            // removing paused expired
            toRemove.Clear();
            for (var i = 0; i < _pausedCameraObjects.Count; i++)
            {
                if (_pausedCameraObjects[i].State == State.Expired)
                {
                    toRemove.Add(i);
                }
            }
            for (var i = 0; i < toRemove.Count; i++)
            {
                _pausedCameraObjects.RemoveAt(toRemove[i]);
            }
        }

        void ClearGraphicsObjects()
        {
            // removing expired
            var toRemove = new List<int>();
            for (var i = 0; i < _graphicsObjects.Count; i++)
            {
                if (_graphicsObjects[i].State == State.Expired)
                    toRemove.Add(i);
            }
            for (var i = 0; i < toRemove.Count; i++)
            {
                _graphicsObjects.RemoveAt(toRemove[i]);
            }

            // getting paused
            var toPause = new List<int>();
            for (var i = 0; i < _graphicsObjects.Count; i++)
            {
                if (_graphicsObjects[i].State == State.Active)
                    continue;

                toPause.Add(i);
            }
            for (var i = 0; i < toPause.Count; i++)
            {
                _pausedGraphicsObjects.Add(_graphicsObjects[toPause[i]]);
                _graphicsObjects.RemoveAt(toPause[i]);
            }

            // resuming active paused
            var toResume = new List<int>();
            for (var i = 0; i < _pausedGraphicsObjects.Count; i++)
            {
                if (_pausedGraphicsObjects[i].State == State.Active)
                {
                    toResume.Add(i);
                }
            }
            for (var i = 0; i < toRemove.Count; i++)
            {
                _graphicsObjects.Add(_pausedGraphicsObjects[toResume[i]]);
                _pausedGraphicsObjects.RemoveAt(toResume[i]);
            }

            // removing paused expired
            toRemove.Clear();
            for (var i = 0; i < _pausedGraphicsObjects.Count; i++)
            {
                if (_pausedGraphicsObjects[i].State == State.Expired)
                {
                    toRemove.Add(i);
                }
            }
            for (var i = 0; i < toRemove.Count; i++)
            {
                _pausedGraphicsObjects.RemoveAt(toRemove[i]);
            }
        }
    }

    protected RenderPipeline(Texture2D outputTexture)
    {
        _outputTexture = outputTexture;
    }
}
