using ImGuiNET;
using SharpEngineEditor.ImGui.Backend;
using System.Diagnostics;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using TerraFX.Interop.DirectX;
using TerraFX.Interop.Windows;
using Buffer = SharpEngineEditor.ImGui.Backend.Buffer;
using Gui = ImGuiNET.ImGui;
using Index = SharpEngineEditor.ImGui.Backend.Index;
using Size = System.Drawing.Size;

namespace SharpEngineEditor.ImGui;

internal class ImGuiRenderer : IDisposable
{
    public struct PerFrameData
    {
        public float frameRate;

        public PerFrameData()
        {
            frameRate = 1.0f / 60.0f;
        }
    }

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
    private readonly Window _window;

    private const int INIT_VERTEX_CAP = 10000;
    private const int INIT_INDEX_CAP = 20000;
    private const float BUFFERS_GROW_RATE = 1.5f;

    private int _vertexCapacity = INIT_VERTEX_CAP;
    private int _indexCapacity = INIT_INDEX_CAP;

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

    private bool _frameStarted;
    private bool _disposed;

    private void Initialize()
    {
        var factory = Factory.GetInstance();
        var adapter = factory.GetAdpters().FirstOrDefault();
        Debug.Assert(adapter != null);
        _device = new Device(adapter);
        _swapchain = factory.CreateSwapchain(_window, _device);
        _context = _device.GetContext();

        _guiContext = Gui.CreateContext();
        Gui.SetCurrentContext(_guiContext);

        var io = Gui.GetIO();
        io.BackendFlags |= ImGuiBackendFlags.RendererHasVtxOffset;
        io.BackendFlags |= ImGuiBackendFlags.RendererHasViewports;

        io.ConfigFlags |= ImGuiConfigFlags.ViewportsEnable;
        io.ConfigFlags |= ImGuiConfigFlags.DockingEnable;
        io.ConfigFlags |= ImGuiConfigFlags.NavEnableKeyboard;

        CreateResources();

        void CreateResources()
        {
            var shaderCompiler = new ShaderCompiler();

            _vertexShader = _device.CreateVertexShader(new ShaderModule(VERTEX_SHADER_NAME));
            _pixelShader = _device.CreatePixelShader(new ShaderModule(PIXEL_SHADER_NAME));

            var matrix = Matrix4x4.CreateOrthographic(1, 1, 0, 0);
            var mvpData = new VertexMVPCBuffer
            {
                Row0 = new(matrix.M11, matrix.M12, matrix.M13, matrix.M14),
                Row1 = new(matrix.M21, matrix.M22, matrix.M23, matrix.M24),
                Row2 = new(matrix.M31, matrix.M32, matrix.M33, matrix.M34),
                Row3 = new(matrix.M41, matrix.M42, matrix.M43, matrix.M44)
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
                AddressMode = D3D11_TEXTURE_ADDRESS_MODE.D3D11_TEXTURE_ADDRESS_WRAP,
                Filter = D3D11_FILTER.D3D11_FILTER_MIN_MAG_MIP_LINEAR,
                ComparisionFunc = D3D11_COMPARISON_FUNC.D3D11_COMPARISON_ALWAYS

            });

            CreateVertexBuffer(_vertexCapacity);
            CreateIndexBuffer(_indexCapacity);

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

            var blendInfo = new BlendStateInfo()
            {
                BlendFactor = new()
            };
            blendInfo.RenderTargetBlendDescs[0] = new D3D11_RENDER_TARGET_BLEND_DESC()
            {
                BlendEnable = true,

                RenderTargetWriteMask = (byte)D3D11_COLOR_WRITE_ENABLE.D3D11_COLOR_WRITE_ENABLE_ALL,

                SrcBlend = D3D11_BLEND.D3D11_BLEND_SRC_ALPHA,
                DestBlend = D3D11_BLEND.D3D11_BLEND_INV_SRC_ALPHA,
                BlendOp = D3D11_BLEND_OP.D3D11_BLEND_OP_ADD,

                SrcBlendAlpha = D3D11_BLEND.D3D11_BLEND_ONE,
                DestBlendAlpha = D3D11_BLEND.D3D11_BLEND_INV_SRC_ALPHA,
                BlendOpAlpha = D3D11_BLEND_OP.D3D11_BLEND_OP_ADD
            };
            _blendState = _device.CreateBlendState(blendInfo);

            _rasterizerState = _device.CreateRasterizerState(new RasterizerStateInfo()
            {
                CullMode = D3D11_CULL_MODE.D3D11_CULL_NONE,
                ScissorsEnabled = true,
                DepthClippingEnabled = true,
                FillMode = D3D11_FILL_MODE.D3D11_FILL_SOLID
            });

            _viewport = new Viewport(new D3D11_VIEWPORT()
            {
                Height = _width,
                Width = _height,
                MaxDepth = 1f
            });

            _scissors = new(new(0, 0, _width, _height));

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
                [],
                Rasterizer.BindFlags.RasterizerState |
                Rasterizer.BindFlags.Viewports);

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

    private void CreateVertexBuffer(int vertexCap)
    {
        _vertexSurface?.Dispose();

        _vertexSurface = new FSurface(
                new(vertexCap * (Unsafe.SizeOf<Vertex>() / Unsafe.SizeOf<FColor1>()), 1));
        _vertexBuffer = Buffer.CreateVertexBuffer(_device.CreateBuffer(
            _vertexSurface,
            typeof(Vertex),
            new ResourceUsageInfo()
            {
                CPUAccessFlags = D3D11_CPU_ACCESS_FLAG.D3D11_CPU_ACCESS_WRITE,
                BindFlags = D3D11_BIND_FLAG.D3D11_BIND_VERTEX_BUFFER,
                Usage = D3D11_USAGE.D3D11_USAGE_DYNAMIC
            }));

    }

    private void CreateIndexBuffer(int indexCap)
    {
        _indexSurface?.Dispose();

        _indexSurface = new USurface(new(indexCap, 1));
        _indexBuffer = Buffer.CreateIndexBuffer(_device.CreateBuffer(
            _indexSurface,
            typeof(Index),
            new ResourceUsageInfo()
            {
                CPUAccessFlags = D3D11_CPU_ACCESS_FLAG.D3D11_CPU_ACCESS_WRITE,
                BindFlags = D3D11_BIND_FLAG.D3D11_BIND_INDEX_BUFFER,
                Usage = D3D11_USAGE.D3D11_USAGE_DYNAMIC
            }));
    }

    public ImGuiRenderer(
        Window window,
        Size resolution)
    {
        _width = resolution.Width;
        _height = resolution.Height;

        _window = window;

        Initialize();
    }

    public void NewFrame()
    {
        Debug.Assert(_frameStarted == false);
        _frameStarted = true;

        _context.ClearRenderTargetView(_renderTarget, new(0, 0, 0, 0));
        Gui.NewFrame();
    }

    public void EndFrame()
    {
        Debug.Assert(_frameStarted);
        _frameStarted = false;
        Gui.EndFrame();
    }

    public void Render(PerFrameData perFrameData)
    {
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

            // taking pass here
            for (var i = 0; i < cmds.Size; i++)
            {
                var cmdList = cmds[i];

                var vBuf = cmdList.VtxBuffer;
                var iBuf = cmdList.IdxBuffer;

                var vCount = vBuf.Size;


                if(vCount >= _vertexCapacity)
                {
                    _vertexCapacity = (int)(_vertexCapacity * BUFFERS_GROW_RATE);
                    CreateVertexBuffer(_vertexCapacity);
                }

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

                if (iCount >= _indexCapacity)
                {
                    _indexCapacity = (int)(_indexCapacity * BUFFERS_GROW_RATE);
                    CreateIndexBuffer(_indexCapacity);
                }

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
                _rasterizerStage.Bind(_context);

                for (var c = 0; c < cBuffer.Size; c++)
                {
                    var cmd = cBuffer[c];

                    _scissors = new Scissors(
                        new(
                            Math.Abs((int)cmd.ClipRect.X - (int)data.DisplayPos.X),
                            Math.Abs((int)cmd.ClipRect.Y - (int)data.DisplayPos.Y),
                            Math.Abs((int)cmd.ClipRect.Z - (int)data.DisplayPos.X),
                            Math.Abs((int)cmd.ClipRect.W - (int)data.DisplayPos.Y)));
                    if (_scissors.Info.Z <= _scissors.Info.X ||
                        _scissors.Info.W <= _scissors.Info.Y)
                        continue;

                    _context.RSSetScissors([_scissors]);

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
            io.Framerate = perFrameData.frameRate;

            var size = _window.GetSize();
            io.DisplaySize = new Vector2(_width, _height);
            var scaleX = size.Width / (float)_width;
            var scaleY = size.Height / (float)_height;
            var scale = scaleX < scaleY ? scaleX : scaleY;
            var viewportWidth = _width * scaleX;
            var viewportHeight = _height * scaleY;
            var viewportX = (size.Width - viewportWidth) / 2;
            var viewportY = (size.Height - viewportHeight) / 2;
            io.DisplayFramebufferScale = new Vector2(scaleX, scaleY);

            unsafe
            {
                var point = new POINT();
                TerraFX.Interop.Windows.Windows.GetCursorPos(&point);
                TerraFX.Interop.Windows.Windows.ScreenToClient(_window.HWnd, &point);
                io.AddMousePosEvent((point.x - viewportX) / scaleX,
                                    (point.y - viewportY) / scaleY);
            }

            float l = data.DisplayPos.X;
            float r = data.DisplayPos.X + data.DisplaySize.X;
            float t = data.DisplayPos.Y;
            float b = data.DisplayPos.Y + data.DisplaySize.Y;

            var mvpData = new VertexMVPCBuffer
            {
                Row0 = new(2.0f / (r - l), 0.0f, 0.0f, 0.0f),
                Row1 = new(0.0f, 2.0f / (t - b), 0.0f, 0.0f),
                Row2 = new(0.0f, 0.0f, 0.5f, 0.0f),
                Row3 = new((r + l) / (l - r), (t + b) / (b - t), 0.5f, 1.0f)
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

    public void ScanMessage(MSG message)
    {
        var io = Gui.GetIO();
        var msg = message.message;

        switch (msg)
        {
            case WM.WM_KEYDOWN:
                {
                    var imKey = HandleVK(message.wParam.Value, message.lParam.Value);
                    io.AddKeyEvent(imKey, true);
                    if(message.wParam.Value >= 0x41 &&
                       message.wParam.Value <= 0x5A)
                    {
                        io.AddInputCharacter((uint)message.wParam.Value);
                    }
                }
                break;
            case WM.WM_KEYUP:
                {
                    var imKey = HandleVK(message.wParam.Value, message.lParam.Value);
                    io.AddKeyEvent(imKey, false);
                }
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
            case WM.WM_MOUSEWHEEL:
                {
                    var value = ((short)(message.wParam >> 16));
                    io.AddMouseWheelEvent(value, value);
                }
                break;
        }

        ImGuiKey HandleVK(nuint vk, nint lp)
        {
            switch (vk)
            {
                case VK.VK_TAB: return ImGuiKey.Tab;
                case VK.VK_LEFT: return ImGuiKey.LeftArrow;
                case VK.VK_RIGHT: return ImGuiKey.RightArrow;
                case VK.VK_UP: return ImGuiKey.UpArrow;
                case VK.VK_DOWN: return ImGuiKey.DownArrow;
                case VK.VK_PRIOR: return ImGuiKey.PageUp;
                case VK.VK_NEXT: return ImGuiKey.PageDown;
                case VK.VK_HOME: return ImGuiKey.Home;
                case VK.VK_END: return ImGuiKey.End;
                case VK.VK_INSERT: return ImGuiKey.Insert;
                case VK.VK_DELETE: return ImGuiKey.Delete;
                case VK.VK_BACK: return ImGuiKey.Backspace;
                case VK.VK_SPACE: return ImGuiKey.Space;
                case VK.VK_RETURN:
                {
                    if ((lp & 0xFF000000) >> 48 == 0x0100)
                    {
                        return ImGuiKey.KeypadEnter;
                    }

                    return ImGuiKey.Enter;
                }
                case VK.VK_ESCAPE: return ImGuiKey.Escape;
                case VK.VK_OEM_7: return ImGuiKey.Apostrophe;
                case VK.VK_OEM_COMMA: return ImGuiKey.Comma;
                case VK.VK_OEM_MINUS: return ImGuiKey.Minus;
                case VK.VK_OEM_PERIOD: return ImGuiKey.Period;
                case VK.VK_OEM_2: return ImGuiKey.Slash;
                case VK.VK_OEM_1: return ImGuiKey.Semicolon;
                case VK.VK_OEM_PLUS: return ImGuiKey.Equal;
                case VK.VK_OEM_4: return ImGuiKey.LeftBracket;
                case VK.VK_OEM_5: return ImGuiKey.Backslash;
                case VK.VK_OEM_6: return ImGuiKey.RightBracket;
                case VK.VK_OEM_3: return ImGuiKey.GraveAccent;
                case VK.VK_CAPITAL: return ImGuiKey.CapsLock;
                case VK.VK_SCROLL: return ImGuiKey.ScrollLock;
                case VK.VK_NUMLOCK: return ImGuiKey.NumLock;
                case VK.VK_SNAPSHOT: return ImGuiKey.PrintScreen;
                case VK.VK_PAUSE: return ImGuiKey.Pause;
                case VK.VK_NUMPAD0: return ImGuiKey.Keypad0;
                case VK.VK_NUMPAD1: return ImGuiKey.Keypad1;
                case VK.VK_NUMPAD2: return ImGuiKey.Keypad2;
                case VK.VK_NUMPAD3: return ImGuiKey.Keypad3;
                case VK.VK_NUMPAD4: return ImGuiKey.Keypad4;
                case VK.VK_NUMPAD5: return ImGuiKey.Keypad5;
                case VK.VK_NUMPAD6: return ImGuiKey.Keypad6;
                case VK.VK_NUMPAD7: return ImGuiKey.Keypad7;
                case VK.VK_NUMPAD8: return ImGuiKey.Keypad8;
                case VK.VK_NUMPAD9: return ImGuiKey.Keypad9;
                case VK.VK_DECIMAL: return ImGuiKey.KeypadDecimal;
                case VK.VK_DIVIDE: return ImGuiKey.KeypadDivide;
                case VK.VK_MULTIPLY: return ImGuiKey.KeypadMultiply;
                case VK.VK_SUBTRACT: return ImGuiKey.KeypadSubtract;
                case VK.VK_ADD: return ImGuiKey.KeypadAdd;
                case VK.VK_LSHIFT: return ImGuiKey.LeftShift;
                case VK.VK_LCONTROL: return ImGuiKey.LeftCtrl;
                case VK.VK_LMENU: return ImGuiKey.LeftAlt;
                case VK.VK_LWIN: return ImGuiKey.LeftSuper;
                case VK.VK_RSHIFT: return ImGuiKey.RightShift;
                case VK.VK_RCONTROL: return ImGuiKey.RightCtrl;
                case VK.VK_RMENU: return ImGuiKey.RightAlt;
                case VK.VK_RWIN: return ImGuiKey.RightSuper;
                case VK.VK_APPS: return ImGuiKey.Menu;
                case '0': return ImGuiKey._0;
                case '1': return ImGuiKey._1;
                case '2': return ImGuiKey._2;
                case '3': return ImGuiKey._3;
                case '4': return ImGuiKey._4;
                case '5': return ImGuiKey._5;
                case '6': return ImGuiKey._6;
                case '7': return ImGuiKey._7;
                case '8': return ImGuiKey._8;
                case '9': return ImGuiKey._9;
                case 'A': return ImGuiKey.A;
                case 'B': return ImGuiKey.B;
                case 'C': return ImGuiKey.C;
                case 'D': return ImGuiKey.D;
                case 'E': return ImGuiKey.E;
                case 'F': return ImGuiKey.F;
                case 'G': return ImGuiKey.G;
                case 'H': return ImGuiKey.H;
                case 'I': return ImGuiKey.I;
                case 'J': return ImGuiKey.J;
                case 'K': return ImGuiKey.K;
                case 'L': return ImGuiKey.L;
                case 'M': return ImGuiKey.M;
                case 'N': return ImGuiKey.N;
                case 'O': return ImGuiKey.O;
                case 'P': return ImGuiKey.P;
                case 'Q': return ImGuiKey.Q;
                case 'R': return ImGuiKey.R;
                case 'S': return ImGuiKey.S;
                case 'T': return ImGuiKey.T;
                case 'U': return ImGuiKey.U;
                case 'V': return ImGuiKey.V;
                case 'W': return ImGuiKey.W;
                case 'X': return ImGuiKey.X;
                case 'Y': return ImGuiKey.Y;
                case 'Z': return ImGuiKey.Z;
                case VK.VK_F1: return ImGuiKey.F1;
                case VK.VK_F2: return ImGuiKey.F2;
                case VK.VK_F3: return ImGuiKey.F3;
                case VK.VK_F4: return ImGuiKey.F4;
                case VK.VK_F5: return ImGuiKey.F5;
                case VK.VK_F6: return ImGuiKey.F6;
                case VK.VK_F7: return ImGuiKey.F7;
                case VK.VK_F8: return ImGuiKey.F8;
                case VK.VK_F9: return ImGuiKey.F9;
                case VK.VK_F10: return ImGuiKey.F10;
                case VK.VK_F11: return ImGuiKey.F11;
                case VK.VK_F12: return ImGuiKey.F12;
                case VK.VK_F13: return ImGuiKey.F13;
                case VK.VK_F14: return ImGuiKey.F14;
                case VK.VK_F15: return ImGuiKey.F15;
                case VK.VK_F16: return ImGuiKey.F16;
                case VK.VK_F17: return ImGuiKey.F17;
                case VK.VK_F18: return ImGuiKey.F18;
                case VK.VK_F19: return ImGuiKey.F19;
                case VK.VK_F20: return ImGuiKey.F20;
                case VK.VK_F21: return ImGuiKey.F21;
                case VK.VK_F22: return ImGuiKey.F22;
                case VK.VK_F23: return ImGuiKey.F23;
                case VK.VK_F24: return ImGuiKey.F24;
                case VK.VK_BROWSER_BACK: return ImGuiKey.AppBack;
                case VK.VK_BROWSER_FORWARD: return ImGuiKey.AppForward;
                default:
                    return ImGuiKey.None;
            }
        }
    }

    protected virtual void Dispose(bool disposing)
    {
        if(_disposed == false)
        {
            _disposed = true;

            if (disposing)
            {
                ReleaseNative();
                ReleaseManaged();

                return;
            }

            ReleaseNative();

            void ReleaseNative()
            {
                Gui.DestroyContext();
            }

            void ReleaseManaged()
            {
                _indexSurface.Dispose();
                _vertexSurface.Dispose();
            }
        }
    }

    public void Dispose()
    {
        Dispose(true);

        GC.SuppressFinalize(this);
    }

    ~ImGuiRenderer()
    {
        Dispose(false);
    }
}