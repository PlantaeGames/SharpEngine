using System.Diagnostics;
using System.Text.Encodings;

using TerraFX.Interop.DirectX;
using static TerraFX.Interop.DirectX.DirectX;
using TerraFX.Interop.Windows;
using System.Text;
using System.Runtime.InteropServices;

namespace SharpEngineCore.Graphics;

/// <summary>
/// Represents the virtual Graphics card provided by the driver.
/// Used for allocation of resources on gpu.
/// </summary>
internal sealed class Device
{
    private ComPtr<ID3D11Device> _pDevice;
    private DeviceContext _context;

    private D3D_FEATURE_LEVEL _featureLevel;

    public InputLayout CreateInputLayout(InputLayoutInfo info)
    {
        var count = info.Layout.GetFragmentsCount();

        Debug.Assert(count > 0,
            "Fragment count can't be zero here.");

        return new InputLayout(NativeCreateInputLayout(), info);

        unsafe ComPtr<ID3D11InputLayout> NativeCreateInputLayout()
        {
            var pDesc = stackalloc D3D11_INPUT_ELEMENT_DESC[count];

            var type = info.Layout.GetType();
            var members = type.GetMembers();
            var feilds = members
                .Where(x => x.MemberType == System.Reflection.MemberTypes.Field)
                .ToArray();

            var pNames = new nint[count];
            for (var i = 0; i < count; i++)
            {
                var nameBytes = Encoding.ASCII.GetBytes(feilds[i].Name);
                nint pName = 0x0;
                try
                {
                   pName = Marshal.AllocHGlobal(nameBytes.Length + 1);
                }
                catch (Exception)
                {
                    foreach (var p in pNames)
                        Marshal.FreeHGlobal(p);

                    throw;
                }

                for (var x = 0; x < nameBytes.Length; x++)
                {
                    *((byte*)pName + (sizeof(byte) * x)) = nameBytes[x];
                }
                *((byte*)pName + (sizeof(byte) * nameBytes.Length)) = 0;

                pNames[i] = pName;
            }
            for (var i = 0; i < count; i++)
            {
                pDesc[i].SemanticName = (sbyte*)pNames[i];
                pDesc[i].SemanticIndex = 0u;
                pDesc[i].Format = DXGI_FORMAT.DXGI_FORMAT_R32G32B32A32_FLOAT;
                pDesc[i].AlignedByteOffset = D3D11.D3D11_APPEND_ALIGNED_ELEMENT;
                pDesc[i].InstanceDataStepRate = 0u;
                pDesc[i].InputSlot = 0u;
                pDesc[i].InputSlotClass = D3D11_INPUT_CLASSIFICATION.D3D11_INPUT_PER_VERTEX_DATA;
            }

            var pBlob = info.VertexShader.GetBlob().GetNativePtr().Get();

            var pInputLayout = new ComPtr<ID3D11InputLayout>();
            fixed(ID3D11Device** ppDevice = _pDevice)
            {
                GraphicsException.SetInfoQueue();
                var result = (*ppDevice)->CreateInputLayout(pDesc, (uint)count,
                    pBlob->GetBufferPointer(), pBlob->GetBufferSize(),
                    pInputLayout.GetAddressOf());

                if(result.FAILED)
                {
                    foreach (var p in pNames)
                        Marshal.FreeHGlobal(p);

                    // error here
                    GraphicsException.ThrowLastGraphicsException(
                        $"Failed to create input layout.\nError Code: {result}");
                }
            }

            foreach (var p in pNames)
                Marshal.FreeHGlobal(p);

            return pInputLayout;
        }
    }

    /// <summary>
    /// Creates a new pixel shader from shader module.
    /// </summary>
    /// <param name="module">The shader module to create shader from.</param>
    /// <returns>Created Pixel Shader.</returns>
    /// <exception cref="GraphicsException"></exception>
    public PixelShader CreatePixelShader(ShaderModule module)
    {
        var compiler = new ShaderCompiler();
        var blob = compiler.Compile(module, new ShaderCompiler.Params()
        {
            Target = ShaderCompiler.Params.Shader.PS,
            FeatureLevel = _featureLevel
        });

        return new PixelShader(NativeCreatePixelShader(), blob);

        unsafe ComPtr<ID3D11PixelShader> NativeCreatePixelShader()
        {
            var pBlob = blob.GetNativePtr().Get();
            var length = pBlob->GetBufferSize();

            var pShader = new ComPtr<ID3D11PixelShader>();
            fixed (ID3D11Device** ppDevice = _pDevice)
            {
                GraphicsException.SetInfoQueue();
                var result = (*ppDevice)->CreatePixelShader(pBlob->GetBufferPointer(), length,
                    (ID3D11ClassLinkage*)IntPtr.Zero,
                    pShader.GetAddressOf());

                if (result.FAILED)
                {
                    // error here.
                    GraphicsException.ThrowLastGraphicsException(
                        $"Failed to create Pixel Shader\nError Code: {result}");
                }
            }

            return pShader;
        }
    }

    /// <summary>
    /// Creates a new vertex shader from shader module.
    /// </summary>
    /// <param name="module">The shader module to create shader from.</param>
    /// <returns>Created Vertex Shader.</returns>
    /// <exception cref="GraphicsException"></exception>
    public VertexShader CreateVertexShader(ShaderModule module)
    {
        var compiler = new ShaderCompiler();
        var blob = compiler.Compile(module, new ShaderCompiler.Params()
        {
            Target = ShaderCompiler.Params.Shader.VS,
            FeatureLevel = _featureLevel
        });

        return new VertexShader(NativeCreateVertexShader(), blob);

        unsafe ComPtr<ID3D11VertexShader> NativeCreateVertexShader()
        {
            var pBlob = blob.GetNativePtr().Get();
            var length = pBlob->GetBufferSize();

            var pShader = new ComPtr<ID3D11VertexShader>();
            fixed(ID3D11Device** ppDevice = _pDevice)
            {
                GraphicsException.SetInfoQueue();
                var result = (*ppDevice)->CreateVertexShader(pBlob->GetBufferPointer(), length,
                    (ID3D11ClassLinkage*)IntPtr.Zero,
                    pShader.GetAddressOf());

                if(result.FAILED)
                {
                    // error here.
                    GraphicsException.ThrowLastGraphicsException(
                        $"Failed to create Vertex Shader\nError Code: {result}");
                }
            }

            return pShader;
        }
    }

    /// <summary>
    /// Create buffer for graphics purposes.
    /// </summary>
    /// <param name="surface">The initial data to write.</param>
    /// <param name="usageInfo">Usuage of the buffer.</param>
    /// <exception cref="GraphicsException"></exception>
    /// <returns></returns>
    public Buffer CreateBuffer(Surface surface, ResourceUsageInfo usageInfo)
    {
        var (ptr, size) = NativeCreateBuffer();
        return new Buffer(ptr, new BufferInfo()
        {
            Size = size,
            UsageInfo = usageInfo
        });

        unsafe (ComPtr<ID3D11Buffer> ptr, int size) NativeCreateBuffer()
        {
            (ComPtr<ID3D11Buffer> ptr, int size) pBuffer = new();
            pBuffer.size = surface.GetSliceSize() * surface.GetPeiceSize();

            var desc = new D3D11_BUFFER_DESC();
            desc.StructureByteStride = (uint) sizeof(Fragment);
            desc.ByteWidth = (uint) pBuffer.size;

            desc.BindFlags = (uint) usageInfo.BindFlags;
            desc.CPUAccessFlags = (uint) usageInfo.CPUAccessFlags;
            desc.Usage = usageInfo.Usage;

            desc.MiscFlags = 0u;

            var initialData = new D3D11_SUBRESOURCE_DATA();
            initialData.pSysMem = surface.GetNativePointer().ToPointer();

            fixed (ID3D11Device** ppDevice = _pDevice)
            {
                GraphicsException.SetInfoQueue();
                var result = (*ppDevice)->CreateBuffer(&desc, &initialData,
                                                        pBuffer.ptr.GetAddressOf());

                if (result.FAILED)
                {
                    // error here.
                    GraphicsException.ThrowLastGraphicsException(
                        $"Failed to Create Buffer\nError Code: {result}");
                }
            }

            return pBuffer;
        }
    }

    /// <summary>
    /// Creates render target view on viewable resources (i.e texture2d ).
    /// 
    /// Throws Graphics Exception on failure.
    /// </summary>
    /// <param name="resource">A viewable resource</param>
    /// <returns>Created render target view.</returns>
    public RenderTargetView CreateRenderTargetView(Resource resource)
    {
        return new RenderTargetView(NativeCreateView());

        unsafe ComPtr<ID3D11RenderTargetView> NativeCreateView()
        {
            //var desc = new D3D11_RENDER_TARGET_VIEW_DESC();
            // TODO: Currently its just working without the description, just like in GrainEngine.

            fixed(ID3D11Device** ppDevice = _pDevice)
            {
                var pRenderTargetView = new ComPtr<ID3D11RenderTargetView>();
                fixed (ID3D11RenderTargetView** ppRenderTargetView = pRenderTargetView)
                {
                    fixed (ID3D11Resource** ppResource = resource.GetNativePtrAsResource())
                    {
                        GraphicsException.SetInfoQueue();
                        var result = (*ppDevice)->CreateRenderTargetView(
                            *ppResource, (D3D11_RENDER_TARGET_VIEW_DESC*)IntPtr.Zero, ppRenderTargetView);

                        if (result.FAILED)
                        {
                            // error here.
                            GraphicsException.ThrowLastGraphicsException(
                                $"Failed to create render target view\nError Code: {result}");
                        }
                    }
                }

                return pRenderTargetView;
            }
        }
    }

    public Texture2D CreateTexture2D(Surface surface, ResourceUsageInfo usageInfo)
    {
        return new Texture2D(NativeCreateTexture2D(), new TextureInfo()
        {
            Size = surface.Size,
            UsageInfo = usageInfo
        });

        unsafe ComPtr<ID3D11Texture2D> NativeCreateTexture2D()
        {
            var desc = new D3D11_TEXTURE2D_DESC();
            desc.Width = (uint)surface.Size.Width;
            desc.Height = (uint)surface.Size.Height;
            desc.MipLevels = 1u;
            desc.ArraySize = 1u;
            desc.SampleDesc = new DXGI_SAMPLE_DESC
            {
                Quality = 0u,
                Count = 1u
            };
            desc.MiscFlags = 0u;
            desc.Format = usageInfo.Format;
            desc.Usage = usageInfo.Usage;
            desc.BindFlags = (uint)usageInfo.BindFlags;
            desc.CPUAccessFlags = (uint)usageInfo.CPUAccessFlags;

            var initialData = new D3D11_SUBRESOURCE_DATA();
            initialData.pSysMem = surface.GetNativePointer().ToPointer();
            initialData.SysMemPitch = (uint)surface.GetSliceSize();

            fixed (ID3D11Device** ppDevice = _pDevice)
            {
                var pTexture2D = new ComPtr<ID3D11Texture2D>();
                fixed (ID3D11Texture2D** ppTexture2D = pTexture2D)
                {
                    GraphicsException.SetInfoQueue();
                    var result = (*ppDevice)->CreateTexture2D(&desc, 
                        &initialData, ppTexture2D);

                    if(result.FAILED)
                    {
                        // error here
                        GraphicsException.ThrowLastGraphicsException(
                            $"Failed to create texture 2d\nError Code: {result}");
                    }
                }

                return pTexture2D;
            }
        }
    }

    public ComPtr<ID3D11Device> GetNativePtr() => new (_pDevice);

    public ImmediateDeviceContext GetContext() => (ImmediateDeviceContext) _context;
    public D3D_FEATURE_LEVEL GetFeatureLevel() => _featureLevel;

    public Device(Adapter adapter, bool isEnumalted = false)
    {
        Create(adapter, isEnumalted);
    }

    private void Create(Adapter adaper, bool isEnumalted)
    {
        NativeCreate();

        unsafe void NativeCreate()
        {
            fixed(ID3D11Device** ppDevice = _pDevice)
            {
                var pImContext = new ComPtr<ID3D11DeviceContext>();
                fixed(ID3D11DeviceContext** ppDeviceContext = pImContext)
                {
                    fixed (IDXGIAdapter** ppAdapter = adaper.GetNativePtr())
                    {
                        D3D_FEATURE_LEVEL featureLevel;
                        D3D_DRIVER_TYPE driverType = isEnumalted ?
                                                     D3D_DRIVER_TYPE.D3D_DRIVER_TYPE_SOFTWARE :
                                                     D3D_DRIVER_TYPE.D3D_DRIVER_TYPE_UNKNOWN;
                        uint flags = 0u;
#if DEBUG
                        flags |= (uint)D3D11_CREATE_DEVICE_FLAG.D3D11_CREATE_DEVICE_DEBUG;
#endif

                        GraphicsException.SetInfoQueue();
                        var result = D3D11CreateDevice(*ppAdapter, driverType,
                            (HMODULE)IntPtr.Zero, flags, (D3D_FEATURE_LEVEL*)IntPtr.Zero, 0u, 
                            D3D11.D3D11_SDK_VERSION, ppDevice, &featureLevel, ppDeviceContext);

                        if (result.FAILED)
                        {
                            // error here.
                            GraphicsException.ThrowLastGraphicsException(
                                $"Failed to create device\nError Code: {result}");
                        }

                        _featureLevel = featureLevel;
                    }
                }

                _context = new ImmediateDeviceContext(pImContext);
            }
        }
    }
}