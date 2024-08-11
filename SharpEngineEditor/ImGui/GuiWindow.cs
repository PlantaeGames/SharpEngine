using ImGuiNET;
using SharpEngineEditor.ImGui.Backend;
using System.Diagnostics;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using TerraFX.Interop.DirectX;
using TerraFX.Interop.Gdiplus;
using TerraFX.Interop.Windows;
using static System.Net.Mime.MediaTypeNames;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Buffer = SharpEngineEditor.ImGui.Backend.Buffer;
using Gui = ImGuiNET.ImGui;
using Index = SharpEngineEditor.ImGui.Backend.Index;
using Size = System.Drawing.Size;

namespace SharpEngineEditor.ImGui;

internal class GuiWindow : Window
{
    [StructLayout(LayoutKind.Sequential, Pack = 0, Size = 32)]
    private struct Vertex : IFragmentable
    {
        public FColor2 Position;
        public FColor2 TexCoord;
        public FColor4 Color;

        public int GetFragmentsCount()
        {
            return ToFragments().Length;
        }

        public int GetSize()
        {
            return Unsafe.SizeOf<Vertex>();
        }

        public Fragment[] ToFragments()
        {
            return
                [
                Position.r,
                Position.g,
                
                TexCoord.r,
                TexCoord.g,
               
                Color.r,
                Color.g,
                Color.b,
                Color.a
                ];
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 0, Size = 64)]
    private struct VertexMVPCBuffer : IFragmentable, ISurfaceable
    {
        public FColor4 Row0;
        public FColor4 Row1;
        public FColor4 Row2;
        public FColor4 Row3;

        public int GetFragmentsCount()
        {
            return ToFragments().Length;
        }

        public int GetSize()
        {
            return Unsafe.SizeOf<VertexMVPCBuffer>();
        }

        public Fragment[] ToFragments()
        {
            return
                [
                Row0.r,
                Row0.g,
                Row0.b,
                Row0.a,

                Row1.r,
                Row1.g,
                Row1.b,
                Row1.a,

                Row2.r,
                Row2.g,
                Row2.b,
                Row2.a,

                Row3.r,
                Row3.g,
                Row3.b,
                Row3.a
                ];
        }

        public Surface ToSurface()
        {
            var surface = new FSurface(new(GetFragmentsCount(), 1));
            surface.SetLinearFragments(ToFragments());
            return surface;
        }
    }

    private const string VERTEX_SHADER_NAME = "Shaders\\ImGuiVertexShader.hlsl";
    private const string PIXEL_SHADER_NAME = "Shaders\\ImGuiPixelShader.hlsl";

    private readonly int _width;
    private readonly int _height;

    private const int MAX_VERTICES = 50000;
    private const int MAX_INDICES = 10000;

    private IntPtr _guiContext;

    private Texture2D _fontTexture;
    private ShaderResourceView _fontTextureSrv;
    private Sampler _fontTextureSampler;

    private Device _device;
    private DeviceContext _context;
    private Swapchain _swapchain;

    private VertexBuffer _vertexBuffer;
    private ConstantBuffer _vertexMvpCBuffer;
    private IndexBuffer _indexBuffer;
    private VertexShader _vertexShader;
    private PixelShader _pixelShader;
    private InputLayout _inputLayout;
    private InputAssembler _inputAssemblerStage;
    private VertexShaderStage _vertexStage;
    private Scissors _scissors;
    private RasterizerState _rasterizerState;
    private Viewport _viewport;
    private Rasterizer _rasterizerStage;
    private PixelShaderStage _pixelStage;
    private BlendState _blendState;
    private OutputMerger _outputMergerStage;
    private RenderTargetView _renderTarget;

    private FSurface _vertexSurface;
    private USurface _indexSurface;

    private bool _initialized;

    private void Initialize()
    {
        var factory = Factory.GetInstance();
        var adapter = factory.GetAdpters().FirstOrDefault();
        Debug.Assert(adapter != null);
        _device = new Device(adapter);
        _swapchain = factory.CreateSwapchain(this, _device);
        _context = _device.GetContext();

        _guiContext = Gui.CreateContext();
        Gui.SetCurrentContext(_guiContext);

        var io = Gui.GetIO();
        io.BackendFlags |= ImGuiBackendFlags.RendererHasVtxOffset;
        io.BackendFlags |= ImGuiBackendFlags.RendererHasViewports;
        io.ConfigFlags |= ImGuiConfigFlags.DockingEnable;
        io.ConfigFlags |= ImGuiConfigFlags.NavEnableKeyboard;

/*        var cfg = new ImFontConfig();
        cfg.FontDataOwnedByAtlas = byte.MinValue;
        cfg.RasterizerMultiply = 1.5f;
        cfg.SizePixels = 768.0f / 32.0f;
        cfg.PixelSnapH = byte.MaxValue;
        cfg.OversampleH = 4;
        cfg.OversampleV = 4;*/

        io.DisplayFramebufferScale = Vector2.One;
        io.DisplaySize = new Vector2(_width, _height);
        io.Framerate = 1 / 60f;

        CreateResources();

        _initialized = true;

        void CreateResources()
        {
            var shaderCompiler = new ShaderCompiler();

            _vertexShader = _device.CreateVertexShader(new ShaderModule(VERTEX_SHADER_NAME));
            _pixelShader = _device.CreatePixelShader(new ShaderModule(PIXEL_SHADER_NAME));

            var matrix = Matrix4x4.CreateOrthographic(1, 1, 0, 0);
            var mvpData = new VertexMVPCBuffer
            {
                Row0 = new (matrix.M11, matrix.M12, matrix.M13, matrix.M14),
                Row1 = new (matrix.M21, matrix.M22, matrix.M23, matrix.M24),
                Row2 = new (matrix.M31, matrix.M32, matrix.M33, matrix.M34),
                Row3 = new (matrix.M41, matrix.M42, matrix.M43, matrix.M44)
            };

            _vertexMvpCBuffer = Buffer.CreateConstantBuffer(_device.CreateBuffer(
                mvpData.ToSurface(),
                typeof(VertexMVPCBuffer),
                new ResourceUsageInfo()
                {
                    CPUAccessFlags = D3D11_CPU_ACCESS_FLAG.D3D11_CPU_ACCESS_WRITE,
                    BindFlags = D3D11_BIND_FLAG.D3D11_BIND_CONSTANT_BUFFER,
                    Usage = D3D11_USAGE.D3D11_USAGE_DYNAMIC
                }));

            unsafe
            {
                io.Fonts.GetTexDataAsRGBA32(
                    out byte* pData, out int dataWidth, out int dataHeight, out int stride);

                var surface = new USurface(new(dataWidth, dataHeight));
                var peiceSize = surface.GetPeiceSize();

                var fragments = new List<Fragment>();
                for (var x = 0; x < (dataWidth * dataHeight) * stride; x++)
                {
                    surface.Set(pData[x], x);
                }

            _fontTexture = _device.CreateTexture2D(
                [surface],
                new TextureCreationInfo()
                {
                    Format = DXGI_FORMAT.DXGI_FORMAT_R8G8B8A8_UNORM,
                    UsageInfo = new ResourceUsageInfo()
                    {
                        CPUAccessFlags = D3D11_CPU_ACCESS_FLAG.D3D11_CPU_ACCESS_WRITE,
                        BindFlags = D3D11_BIND_FLAG.D3D11_BIND_SHADER_RESOURCE,
                        Usage = D3D11_USAGE.D3D11_USAGE_DYNAMIC
                    }
                });

                io.Fonts.TexID = (IntPtr)_fontTexture.GetNativePtr().Get();
            }

            _fontTextureSrv = _device.CreateShaderResourceView(
                _fontTexture,
                new ViewCreationInfo()
                {
                    Format = _fontTexture.Info.Format,
                    ViewResourceType = ViewResourceType.Texture2D
                });

            _fontTextureSampler = _device.CreateSampler(new SamplerInfo()
            {
               AddressMode = D3D11_TEXTURE_ADDRESS_MODE.D3D11_TEXTURE_ADDRESS_CLAMP,
                Filter = D3D11_FILTER.D3D11_FILTER_ANISOTROPIC

            });

            _renderTarget = _device.CreateRenderTargetView(
                _swapchain.GetBackTexture(), new ViewCreationInfo()
                {
                    Format = _swapchain.GetBackTexture().Info.Format,
                    ViewResourceType = ViewResourceType.Texture2D
                });

            _inputLayout = _device.CreateInputLayout(new InputLayoutInfo()
            {
                Layout = new Vertex(),
                Topology = Topology.TriangleList,
                VertexShader = _vertexShader
            });

            _vertexSurface = new FSurface(
                    new(MAX_VERTICES * (Unsafe.SizeOf<Vertex>() / Unsafe.SizeOf<FColor1>()), 1));
            _vertexBuffer = Buffer.CreateVertexBuffer(_device.CreateBuffer(
                _vertexSurface,
                typeof(Vertex),
                new ResourceUsageInfo()
                {
                    CPUAccessFlags = D3D11_CPU_ACCESS_FLAG.D3D11_CPU_ACCESS_WRITE,
                    BindFlags = D3D11_BIND_FLAG.D3D11_BIND_VERTEX_BUFFER,
                    Usage = D3D11_USAGE.D3D11_USAGE_DYNAMIC
                }));

            _indexSurface = new USurface(new(MAX_INDICES, 1));
            _indexBuffer = Buffer.CreateIndexBuffer(_device.CreateBuffer(
                _indexSurface,
                typeof(Index),
                new ResourceUsageInfo()
                {
                    CPUAccessFlags = D3D11_CPU_ACCESS_FLAG.D3D11_CPU_ACCESS_WRITE,
                    BindFlags = D3D11_BIND_FLAG.D3D11_BIND_INDEX_BUFFER,
                    Usage = D3D11_USAGE.D3D11_USAGE_DYNAMIC
                }));

            var blendInfo = new BlendStateInfo()
            {
                BlendFactor = new(1,1,1,1)
            };
            blendInfo.RenderTargetBlendDescs[0] = new D3D11_RENDER_TARGET_BLEND_DESC()
            {
                BlendEnable = true,
                RenderTargetWriteMask = (byte)D3D11_COLOR_WRITE_ENABLE.D3D11_COLOR_WRITE_ENABLE_ALL,

                SrcBlend = D3D11_BLEND.D3D11_BLEND_SRC_COLOR,
                DestBlend = D3D11_BLEND.D3D11_BLEND_DEST_COLOR,
                BlendOp = D3D11_BLEND_OP.D3D11_BLEND_OP_ADD,

                SrcBlendAlpha = D3D11_BLEND.D3D11_BLEND_SRC_ALPHA,
                DestBlendAlpha = D3D11_BLEND.D3D11_BLEND_DEST_ALPHA,
                BlendOpAlpha = D3D11_BLEND_OP.D3D11_BLEND_OP_ADD
            };
            _blendState = _device.CreateBlendState(blendInfo);

            _rasterizerState = _device.CreateRasterizerState(new RasterizerStateInfo()
            {
                CullMode = D3D11_CULL_MODE.D3D11_CULL_NONE,
                ScissorsEnabled = false,
                FillMode = D3D11_FILL_MODE.D3D11_FILL_SOLID
            });

            _viewport = new Viewport(new D3D11_VIEWPORT()
            {
                Height = _width,
                Width = _height,
                MaxDepth = 1f
            });

            _scissors = new Scissors(new(0, 0, 0, 0));

            _inputAssemblerStage = new InputAssembler(
                _inputLayout,
                _vertexBuffer,
                _indexBuffer,
                InputAssembler.BindFlags.VertexBuffer |
                InputAssembler.BindFlags.IndexBuffer |
                InputAssembler.BindFlags.Layout);

            _vertexStage = new VertexShaderStage(
                _vertexShader,
                [], 0, 
                [_vertexMvpCBuffer],
                [],
                VertexShaderStage.BindFlags.VertexShader |
                VertexShaderStage.BindFlags.ConstantBuffers);

            _rasterizerStage = new Rasterizer(
                _rasterizerState,
                [_viewport],
                [_scissors],
                Rasterizer.BindFlags.RasterizerState |
                Rasterizer.BindFlags.Viewports |
                Rasterizer.BindFlags.Scissors);

            _pixelStage = new PixelShaderStage(
                _pixelShader,
                [], [], 0, [],
                PixelShaderStage.BindFlags.PixelShader);

            _outputMergerStage = new OutputMerger(
                [_renderTarget],
                default, default, default, [],
                default,
                _blendState,
                OutputMerger.BindFlags.RenderTargetViewAndDepthView |
                OutputMerger.BindFlags.BlendState);
        }
    }

    public GuiWindow(
        string name, Point location, Size size, HWND parent) :
        base(name, location, size, parent)
    {
        _width = size.Width;
        _height = size.Height;

        Initialize();
    }

    public void SetContext()
    {
        Gui.SetCurrentContext(_guiContext);
    }

    public void Render()
    {
        Debug.Assert(Gui.GetCurrentContext() == _guiContext);

        // make gui.
        Gui.NewFrame();
        Gui.ShowDemoWindow();
        Gui.EndFrame();

        Gui.Render();


        var io = Gui.GetIO();
        var data = Gui.GetDrawData();

        UpdateParams();

        ClearContext();

        Pass();

        ClearContext();

        _swapchain.Present();

        unsafe void Pass()
        {
            var cmds = data.CmdLists;
            SetContext();

            // taking pass here
            for (var i = 0; i < cmds.Size; i++)
            {
                var cmdList = cmds[i];

                var vBuf = cmdList.VtxBuffer;
                var iBuf = cmdList.IdxBuffer;

                var vCount = vBuf.Size;
                var fragments = new List<Fragment>();
                for (var v = 0; v < vCount; v++)
                {
                    var vertex = vBuf[v];

                    var posXFrag = new Fragment();
                    var posYFrag = new Fragment();
                    var uvXFrag = new Fragment();
                    var uvYFrag = new Fragment();
                    var colorRFrag = new Fragment();
                    var colorGFrag = new Fragment();
                    var colorBFrag = new Fragment();
                    var colorAFrag = new Fragment();

                    posXFrag.value = vertex.pos.X;
                    posYFrag.value = vertex.pos.Y;
                    uvXFrag.value = vertex.uv.X;
                    uvYFrag.value = vertex.uv.Y;
                    colorRFrag.value = ((vertex.col & 0x000000FF) >> 0) / (float)byte.MaxValue;
                    colorGFrag.value = ((vertex.col & 0x0000FF00) >> 8) / (float)byte.MaxValue;
                    colorBFrag.value = ((vertex.col & 0x00FF0000) >> 16) / (float)byte.MaxValue;
                    colorAFrag.value = ((vertex.col & 0xFF000000) >> 24) / (float)byte.MaxValue;

                    fragments.Add(posXFrag);
                    fragments.Add(posYFrag);
                    fragments.Add(uvXFrag);
                    fragments.Add(uvYFrag);
                    fragments.Add(colorRFrag);
                    fragments.Add(colorGFrag);
                    fragments.Add(colorBFrag);
                    fragments.Add(colorAFrag);
                }
                _vertexSurface.SetLinearFragments(fragments.ToArray());

                var iCount = iBuf.Size;
                for (var x = 0; x < iCount; x++)
                {
                    var index = iBuf[x];
                    _indexSurface.SetUnit(new(x, 0), new Index(index));
                }

                _vertexBuffer.Update(_vertexSurface);
                _indexBuffer.Update(_indexSurface);

                var cBuffer = cmdList.CmdBuffer;

                _inputAssemblerStage.Bind(_context);
                _vertexStage.Bind(_context);
                _outputMergerStage.Bind(_context);
                _pixelStage.Bind(_context);

                _context.ClearRenderTargetView(_renderTarget, new(0, 0, 0, 0));

                for (var c = 0; c < cBuffer.Size; c++)
                {
                    var cmd = cBuffer[c];

                    _scissors = new Scissors(
                        new(
                            (int)cmd.ClipRect.X,
                            (int)(io.DisplaySize.Y - cmd.ClipRect.W),
                            (int)(cmd.ClipRect.Z - cmd.ClipRect.X),
                            (int)(cmd.ClipRect.W - cmd.ClipRect.Y)));

                    _rasterizerStage.Bind(_context);

                    if (cmd.TextureId == (IntPtr)_fontTexture.GetNativePtr().Get())
                    {
                        _context.PSSetSamplers([_fontTextureSampler], 0);
                        _context.PSSetShaderResources([_fontTextureSrv], 0);
                    }

                    // draw call here
                    _context.DrawIndexed((int)cmd.ElemCount, (int)cmd.IdxOffset);

                    _context.PSSetSamplers([_fontTextureSampler], 0, true);
                    _context.PSSetShaderResources([_fontTextureSrv], 0, true);
                }
            }
        }

        void UpdateParams()
        {
            var pos = GetPosition();
            data.DisplayPos = new(pos.X, pos.Y);

            var matrix = Matrix4x4.CreateOrthographicOffCenter(
                data.DisplayPos.X,
                io.DisplaySize.X + data.DisplayPos.X,
                io.DisplaySize.Y + data.DisplayPos.Y,
                data.DisplayPos.Y, -1, 1);
            var mvpData = new VertexMVPCBuffer
            {
                Row0 = new(matrix.M11, matrix.M12, matrix.M13, matrix.M14),
                Row1 = new(matrix.M21, matrix.M22, matrix.M23, matrix.M24),
                Row2 = new(matrix.M31, matrix.M32, matrix.M33, matrix.M34),
                Row3 = new(matrix.M41, matrix.M42, matrix.M43, matrix.M44)
            };

            _vertexMvpCBuffer.Update(mvpData);

            _viewport = new Viewport(new D3D11_VIEWPORT()
            {
                Height = io.DisplaySize.Y,
                Width = io.DisplaySize.X,
                MaxDepth = 1f
            });
        }

        void ClearContext()
        {
            _context.ClearState();
        }
    }

    public override (bool availability, MSG msg) PeekAndDispatchMessage()
    {

        var result = base.PeekAndDispatchMessage();

        if (_initialized)
        {
            Gui.SetCurrentContext(_guiContext);
            var io = Gui.GetIO();
            var msg = result.msg.message;

            unsafe
            {
                var point = new POINT();
                TerraFX.Interop.Windows.Windows.GetCursorPos(&point);
                var pos = this.GetPosition();
                io.AddMousePosEvent(point.x - pos.X, point.y - pos.Y);
            }

            switch (msg)
            {
                case WM.WM_KEYDOWN:
                    
                    io.AddKeyEvent((ImGuiKey)result.msg.wParam.Value, true);
                    break;
                case WM.WM_KEYUP:
                    io.AddKeyEvent((ImGuiKey)result.msg.wParam.Value, false);
                    break;
                case WM.WM_MBUTTONDOWN:
                    io.AddMouseButtonEvent(2, true);
                    break;
                case WM.WM_MBUTTONUP:
                    io.AddMouseButtonEvent(2, false);
                    break;
                case WM.WM_RBUTTONDOWN:
                    io.AddMouseButtonEvent(1, true);
                    break;
                case WM.WM_RBUTTONUP:
                    io.AddMouseButtonEvent(1, false);
                    break;
                case WM.WM_LBUTTONDOWN:
                    io.AddMouseButtonEvent(0, true);
                    break;
                case WM.WM_LBUTTONUP:
                    io.AddMouseButtonEvent(0, false);
                    break;
            }
        }

        return result;
    }
}

