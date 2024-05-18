namespace SharpEngineCore.Graphics;

internal abstract class Pass : PipelineEvents
{
    protected List<GraphicsObject> _toAdd = new();
    private List<GraphicsObject> _toRemove = new();
    private List<GraphicsObject> _toPause = new();
    private List<GraphicsObject> _paused = new();

    public List<GraphicsObject> GraphicsObjects { get; private set; }

    public sealed override void Initialize(Device device, DeviceContext context)
    {
        OnInitialize(device, context);
    }

    public sealed override void Ready(Device device, DeviceContext context)
    {
        OnReady(device, context);
        ClearPendingGraphicsObjects();
    }

    public sealed override void Go(Device device, DeviceContext context)
    {
        OnGo(device, context);
    }

    private void ClearPendingGraphicsObjects()
    {
        // removing expired
        foreach (var graphicsObject in GraphicsObjects)
        {
            if (graphicsObject.State == State.Active)
                continue;

            if (graphicsObject.State == State.Paused)
            {
                _toPause.Add(graphicsObject);
                continue;
            }

            _toRemove.Add(graphicsObject);
        }
        foreach (var graphicsObject in _toRemove)
        {
            GraphicsObjects.Remove(graphicsObject);
        }
        _toRemove.Clear();

        // adding pending
        foreach(var graphicsObject in _toAdd)
        {
            GraphicsObjects.Add(graphicsObject);
        }
        _toAdd.Clear();

        // pausing newbies
        foreach(var graphicsObject in _toPause)
        {
            _paused.Add(graphicsObject);
            GraphicsObjects.Remove(graphicsObject);
        }
        _toPause.Clear();

        // managing paused'
        var tempgraphicsObjects = new List<GraphicsObject>();
        foreach(var graphicsObject in _paused)
        {
            if (graphicsObject.State == State.Paused)
                continue;

            if(graphicsObject.State == State.Active)
            {
                _toAdd.Add(graphicsObject);
                tempgraphicsObjects.Add(graphicsObject);

                continue;
            }

            _toRemove.Add(graphicsObject);
            tempgraphicsObjects.Add(graphicsObject);
        }
        foreach(var graphicsObject in tempgraphicsObjects)
        {
            _paused.Remove(graphicsObject);
        }
    }

    protected Pass()
    {}
}