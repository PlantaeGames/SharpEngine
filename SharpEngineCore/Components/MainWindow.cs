using TerraFX.Interop.Windows;

using SharpEngineCore.Graphics;
using SharpEngineCore.Utilities;
using Index = SharpEngineCore.Graphics.Index;

namespace SharpEngineCore.Components;

internal sealed class MainWindow : Window
{
    private Logger _log;
    private Renderer _renderer;
    private Swapchain _swapchain;

    public void Update()
    {
        _renderer.Render();
        _swapchain.Present();
    }

    private void Initialize()
    {
        _log.LogHeader("Creating Renderer.");

        var factory = Factory.GetInstance();
        var adapter = factory.GetAdpters()[0];

        _renderer = new Renderer(adapter);

        _swapchain = _renderer.CreateSwapchain(this);
        var backTexture = _swapchain.GetBackTexture();

        var pipeline = new DefaultRenderPipeline(backTexture);

        var triangle = new Mesh();
        triangle.Vertices =
            [
                new Vertex()
                {
                    Position = new(0f, 0.5f, 0, 1),
                    Color = new(1f, 0, 0, 0),
                    Normal = new (),
                    TexCoord = new()
                },
                new Vertex()
                {
                    Position = new(0.5f, -0.5f, 0, 1),
                    Color = new(0f, 1f, 0, 0),
                    Normal = new (),
                    TexCoord = new()
                },
                new Vertex()
                {
                    Position = new(-0.5f, -0.5f, 0, 1),
                    Color = new(0f, 0, 1f, 0),
                    Normal = new (),
                    TexCoord = new()
                }
            ];
        triangle.Indices =
            [
                new Index()
                {
                    Value = new (0)
                },
                new Index()
                {
                    Value = new (1)
                },
                new Index()
                {
                    Value = new (2)
                }
            ];

        _renderer.SetPipeline(pipeline);

        var pass = pipeline
                .Get<ForwardRenderPass>()
                .Get<ForwardPass>();
        pass.AddSubVariation(new(triangle));

        _log.LogMessage("Renderer Created.");
        _log.BreakLine();
        _log.LogMessage("Presenting...");
    }

    public MainWindow(string name, Point location, Size size) : base(name, location, size)
    {
        _log = new();
        Initialize();
    }

    protected override LRESULT WndProc(HWND hWND, uint msg, WPARAM wParam, LPARAM lParam)
    {
        return base.WndProc(hWND, msg, wParam, lParam);
    }
}