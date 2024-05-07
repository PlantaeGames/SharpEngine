using TerraFX.Interop.Windows;


namespace SharpEngineCore.Graphics;

internal class Renderer
{
    protected readonly Device _device;
    protected readonly RenderPipeline _pipeline;

    public void Render()
    {
        // TODO: Start Pipeline Here.
        _pipeline.Ready(_device);
        _pipeline.Go(_device);
    }

    public Renderer(RenderPipeline pipeline, Adapter adapter)
    {
        _pipeline = pipeline;
        _device = new Device(adapter);

        InitializePipeline();
    }

    private void InitializePipeline()
    {
        _pipeline.Initialize(_device);
    }
}
