﻿namespace SharpEngineCore.Graphics;

internal class Renderer
{
    protected readonly Device _device;
    protected readonly DeviceContext _context;
    protected RenderPipeline _pipeline;

    public void SetPipeline(RenderPipeline pipeline)
    {
        _pipeline = pipeline;
        InitializePipeline();
    }

    public Swapchain CreateSwapchain(Window window)
    {
        return Factory.GetInstance().CreateSwapchain(window, _device);
    }

    public void Render()
    {
        _context.ClearState();
        _pipeline.Ready(_device, _context);
        _pipeline.Go(_device, _context);
    }

    public Renderer(Adapter adapter)
    {
        _device = new Device(adapter);
        _context = _device.GetContext();
    }

    private void InitializePipeline()
    {
        if (_pipeline.Initalized)
            return;

        _pipeline.Initialize(_device, _context);
    }
}