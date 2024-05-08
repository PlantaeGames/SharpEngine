namespace SharpEngineCore.Graphics;

internal class Renderer
{
    protected readonly Device _device;
    protected readonly DeviceContext _context;
    protected readonly RenderPipeline _pipeline;

    public void Render()
    {
        _context.ClearState();
        _pipeline.Ready(_device, _context);
        _pipeline.Go(_device, _context);
    }

    public Renderer(RenderPipeline pipeline, Adapter adapter)
    {
        _pipeline = pipeline;
        _device = new Device(adapter);
        _context = _device.GetContext();

        InitializePipeline();
    }

    private void InitializePipeline()
    {
        _pipeline.Initialize(_device, _context);
    }
}
