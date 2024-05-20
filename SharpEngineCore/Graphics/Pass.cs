namespace SharpEngineCore.Graphics;

internal abstract class Pass : PipelineEvents
{

    private List<PipelineVariation> _paused = new();
    protected List<PipelineVariation> _subVariations = new();

    public sealed override void Initialize(Device device, DeviceContext context)
    {
        OnInitialize(device, context);
    }

    public sealed override void Ready(Device device, DeviceContext context)
    {
        ClearPendingVariations();
        OnReady(device, context);
    }

    public sealed override void Go(Device device, DeviceContext context)
    {
        OnGo(device, context);
    }

    private void ClearPendingVariations()
    {
        // removing expired
        var toRemove = new List<int>();
        for (var i = 0; i < _subVariations.Count; i++)
        {
            if (_subVariations[i].State == State.Expired)
                toRemove.Add(i);
        }
        for (var i = 0; i < toRemove.Count; i++)
        {
            _subVariations.RemoveAt(toRemove[i]);
        }

        // getting paused
        var toPause = new List<int>();
        for (var i = 0; i < _subVariations.Count; i++)
        {
            if (_subVariations[i].State == State.Active)
                continue;

            toPause.Add(i);
        }
        for (var i = 0; i < toPause.Count; i++)
        {
            _paused.Add(_subVariations[toPause[i]]);
            _subVariations.RemoveAt(toPause[i]);
        }

        // resuming active paused
        var toResume = new List<int>();
        for (var i = 0; i < _paused.Count; i++)
        {
            if (_paused[i].State == State.Active)
            {
                toResume.Add(i);
            }
        }
        for (var i = 0; i < toRemove.Count; i++)
        {
            _subVariations.Add(_paused[toResume[i]]);
            _paused.RemoveAt(toResume[i]);
        }

        // removing paused expired
        toRemove.Clear();
        for (var i = 0; i < _paused.Count; i++)
        {
            if (_paused[i].State == State.Expired)
            {
                toRemove.Add(i);
            }
        }
        for (var i = 0; i < toRemove.Count; i++)
        {
            _paused.RemoveAt(toRemove[i]);
        }
    }

    protected Pass()
    {}
}