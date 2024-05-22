using System.Diagnostics;
using System.Text;
using System.Runtime.InteropServices;

using TerraFX.Interop.DirectX;
using static TerraFX.Interop.DirectX.DirectX;
using TerraFX.Interop.Windows;

using SharpEngineCore.Utilities;

namespace SharpEngineCore.Graphics;

/// <summary>
/// Represents the virtual Graphics card provided by the driver.
/// Used for allocation of resources on gpu.
/// </summary>
internal sealed class Device
{
    private Adapter _adapter;
    private ComPtr<ID3D11Device> _pDevice;
    private DeviceContext _context;

    private D3D_FEATURE_LEVEL _featureLevel;

    public UnorderedAccessView CreateUnorderedAccessView(Resource resource,
                                                         ViewCreationInfo info)
    {
        return new UnorderedAccessView(NativeCreateUnorderedAccessView(),
            resource, new UnorderedAccessViewInfo()
            {
                ViewInfo = info,
                ResourceViewInfo = new ResourceViewInfo()
                {
                    ResourceInfo = resource.ResourceInfo
                }
            }, this);

        unsafe ComPtr<ID3D11UnorderedAccessView> NativeCreateUnorderedAccessView()
        {
            var desc = new D3D11_UNORDERED_ACCESS_VIEW_DESC();
            desc.Format = info.Format;

            switch (info.ViewResourceType)
            {
                case ViewResourceType.Buffer:
                    desc.ViewDimension = D3D11_UAV_DIMENSION.D3D11_UAV_DIMENSION_BUFFER;
                    desc.Buffer.NumElements = (uint)(info.BufferBytesSize / info.BufferByteStride);
                    desc.Buffer.FirstElement = 0;
                    break;
                case ViewResourceType.Texture2D:
                    desc.ViewDimension = D3D11_UAV_DIMENSION.D3D11_UAV_DIMENSION_TEXTURE2D;
                    desc.Texture2D.MipSlice = 0u;
                    break;

                default:
                    Debug.Assert(false,
                        "Unknown type of unordered access view");
                    break;
            }

            var pView = new ComPtr<ID3D11UnorderedAccessView>();
            fixed (ID3D11Device** ppDevice = _pDevice)
            {
                GraphicsException.SetInfoQueue();
                var result = (*ppDevice)->CreateUnorderedAccessView(resource.GetNativePtrAsResource(),
                                &desc, pView.GetAddressOf());

                if(result.FAILED)
                {
                    // error here.
                    GraphicsException.ThrowLastGraphicsException(
                        $"Failed to create unordered resource view\nError Code: {result}");
                }
            }

            return pView;
        }
    }

    public DepthStencilView CreateDepthStencilView(Resource resource, ViewCreationInfo info)
    {
        return new DepthStencilView(NativeCreateDepthStencilView(), resource,
            new DepthStencilViewInfo()
            {
                ViewInfo = info,
                ResourceViewInfo = new ResourceViewInfo()
                {
                    ResourceInfo = resource.ResourceInfo
                }
            },
            this);

        unsafe ComPtr<ID3D11DepthStencilView> NativeCreateDepthStencilView()
        {
            // TODO: Using the same params as the resource created, at mip map 0.
            var desc = new D3D11_DEPTH_STENCIL_VIEW_DESC();
            desc.Format = info.Format;
            desc.ViewDimension = D3D11_DSV_DIMENSION.D3D11_DSV_DIMENSION_TEXTURE2D;
            desc.Texture2D.MipSlice = 0;

            var pView = new ComPtr<ID3D11DepthStencilView>();
            fixed(ID3D11Device** ppDevice = _pDevice)
            {
                GraphicsException.SetInfoQueue();
                var result = (*ppDevice)->CreateDepthStencilView(resource.GetNativePtrAsResource(),
                    &desc,
                    pView.GetAddressOf());

                if(result.FAILED)
                {
                    // error here
                    GraphicsException.ThrowLastGraphicsException(
                        $"Failed to create depth stencil view\nError Code: {result}");
                }
            }

            return pView;
        }
    }

    public DepthStencilState CreateDepthStencilState(DepthStencilStateInfo info)
    {
        return new DepthStencilState(NativeCreateDepthStencilState(), info,
            this);

        unsafe ComPtr<ID3D11DepthStencilState> NativeCreateDepthStencilState()
        {
            var desc = new D3D11_DEPTH_STENCIL_DESC();
            desc.DepthEnable = info.DepthEnabled;
            desc.DepthWriteMask = info.DepthWriteMask;
            desc.DepthFunc = info.DepthComparisionFunc;

            desc.StencilEnable = info.StencilEnable;
            desc.StencilReadMask = info.StencilReadMask;
            desc.StencilWriteMask = info.StencilWriteMask;
            desc.FrontFace = info.FrontFace;
            desc.BackFace = info.BackFace;

            var pState = new ComPtr<ID3D11DepthStencilState>();
            fixed(ID3D11Device** ppDevice = _pDevice)
            {
                GraphicsException.SetInfoQueue();

                var result = (*ppDevice)->CreateDepthStencilState(&desc,
                    pState.GetAddressOf());

                if(result.FAILED)
                {
                    // error here
                    GraphicsException.ThrowLastGraphicsException(
                        $"Failed to create Depth Stencil State\nError Code: {result}");
                }
            }

            return pState;
        }
    }

    public ShaderResourceView CreateShaderResourceView(Resource resource,
        ViewCreationInfo info)
    {
        return new ShaderResourceView(
            NativeCreateShaderResourceView(),
            resource,
            new ShaderResourceViewInfo
            {
                ViewInfo = info,
                ResourceViewInfo = new ResourceViewInfo()
                {
                    ResourceInfo = resource.ResourceInfo
                }
            },
            this);

        unsafe ComPtr<ID3D11ShaderResourceView> NativeCreateShaderResourceView()
        {
            var desc = new D3D11_SHADER_RESOURCE_VIEW_DESC();
            desc.Format = info.Format;

            var pDesc = &desc;

            switch (info.ViewResourceType)
            {
                case ViewResourceType.Buffer:
                    desc.ViewDimension = D3D_SRV_DIMENSION.D3D11_SRV_DIMENSION_BUFFER;
                    desc.Buffer.FirstElement = (uint)0;
                    desc.Buffer.NumElements = (uint)(info.BufferBytesSize / info.BufferByteStride);
                    break;
                case ViewResourceType.Texture2D:
                    desc.ViewDimension = D3D_SRV_DIMENSION.D3D11_SRV_DIMENSION_TEXTURE2D;
                    desc.Texture2D.MostDetailedMip = 0;
                    desc.Texture2D.MipLevels = (uint)info.TextureMipLevels;
                    unchecked
                    {
                        desc.Texture2D.MipLevels = (uint)-1;
                    }
                    break;
                case ViewResourceType.CubeMap:
                    desc.ViewDimension = D3D_SRV_DIMENSION.D3D11_SRV_DIMENSION_TEXTURECUBE;
                    desc.TextureCube.MostDetailedMip = 0;
                    desc.TextureCube.MipLevels = (uint)info.TextureMipLevels;
                    break;
                default:
                    pDesc = (D3D11_SHADER_RESOURCE_VIEW_DESC*)IntPtr.Zero;
                    break;
            }

            var pView = new ComPtr<ID3D11ShaderResourceView>();
            fixed(ID3D11Device** ppDevice = _pDevice)
            {
                GraphicsException.SetInfoQueue();
                var result = (*ppDevice)->CreateShaderResourceView(
                    resource.GetNativePtrAsResource(),
                    pDesc, pView.GetAddressOf());

                if(result.FAILED)
                {
                    // error here
                    GraphicsException.ThrowLastGraphicsException(
                        $"Failed to create shader resource view\nError Code: {result}");
                }
            }

            return pView;
        }
    }

    public Sampler CreateSampler(SamplerInfo info)
    {
        return new Sampler(NativeCreateSampler(), info, this);

        unsafe ComPtr<ID3D11SamplerState> NativeCreateSampler()
        {
            var desc = new D3D11_SAMPLER_DESC();
            desc.Filter = info.Filter;
            desc.AddressU = info.AddressMode;
            desc.AddressV = info.AddressMode;
            desc.AddressW = info.AddressMode;
            desc.BorderColor = new D3D11_SAMPLER_DESC._BorderColor_e__FixedBuffer()
                                { e0 = info.BorderColor.r };
            desc.ComparisonFunc = info.ComparisionFunc;

            var pSampler = new ComPtr<ID3D11SamplerState>();
            fixed(ID3D11Device** ppDevice = _pDevice)
            {
                GraphicsException.SetInfoQueue();
                var result = (*ppDevice)->CreateSamplerState(&desc, pSampler.GetAddressOf());

                if(result.FAILED)
                {
                    // error here
                    GraphicsException.ThrowLastGraphicsException(
                        $"Failed to Create Sampler\nError Code: {result}");
                }
            }

            return pSampler;
        }
    }

    public InputLayout CreateInputLayout(InputLayoutInfo info)
    {
        var type = info.Layout.GetType();
        var feilds = type.GetFields();;

        var count = feilds.Length;

        Debug.Assert(count > 0,
            "Fragment count can't be zero here.");

        return new InputLayout(NativeCreateInputLayout(), info, this);

        unsafe ComPtr<ID3D11InputLayout> NativeCreateInputLayout()
        {
            var format = DXGI_FORMAT.DXGI_FORMAT_UNKNOWN;
            switch (info.Layout.GetFragmentsCount())
            {
                case 4:
                    format = DXGI_FORMAT.DXGI_FORMAT_R32_FLOAT;
                    break;
                case 8:
                    format = DXGI_FORMAT.DXGI_FORMAT_R32G32_FLOAT;
                    break;
                case 12:
                    format = DXGI_FORMAT.DXGI_FORMAT_R32G32B32_FLOAT;
                    break;
                case 16:
                    format = DXGI_FORMAT.DXGI_FORMAT_R32G32B32A32_FLOAT;
                    break;
                default:
                    Debug.Assert(false,
                        "Input Layout can only have 4,8,12,16 fragments.");
                    break;
            }

            var pDesc = stackalloc D3D11_INPUT_ELEMENT_DESC[count];

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
                pDesc[i].Format = format;
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

        return new PixelShader(NativeCreatePixelShader(), blob, this);

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

        return new VertexShader(NativeCreateVertexShader(), blob, this);

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
    /// Create structured buffer for graphics purposes.
    /// </summary>
    /// <param name="surface">The initial data to write.</param>
    /// <param name="usageInfo">Usuage of the buffer.</param>
    /// <exception cref="GraphicsException"></exception>
    /// <returns>Created structured buffer.</returns>
    public Buffer CreateBuffer(Surface surface, Type layout,
        ResourceUsageInfo usageInfo)
    {
        Debug.Assert(layout.GetMembers().Length > 0,
            "Layout can't be empty.");
        Debug.Assert(layout.GetInterface(nameof(IFragmentable)) != null ||
                     layout.GetInterface(nameof(IUnitable)) != null,
            "Layout must be inherited from IUnitable or IFragmentable.");
        Debug.Assert(surface.Size.Height == 1,
            "Buffers can't be 2 dimensional, consider setting the height to 1.");
        Debug.Assert(surface.Channels == Channels.Single,
            "Surface used for creating buffer must be single channel.");

        var stride = layout.GetDataTypeSize();
        var bytesSize = surface.Size.ToArea() * surface.GetPeiceSize();

        var ptr = NativeCreateBuffer();
        return new Buffer(ptr, new BufferInfo()
        {
            Layout = layout,
            SurfaceSize = surface.Size,
            ByteStride = stride,
            BytesSize = bytesSize,
            UsageInfo = usageInfo
        },
        this);

        unsafe ComPtr<ID3D11Buffer> NativeCreateBuffer()
        {
            var desc = new D3D11_BUFFER_DESC();
            desc.ByteWidth = (uint) bytesSize;
            desc.StructureByteStride = (uint) stride;

            desc.BindFlags = (uint) usageInfo.BindFlags;
            desc.CPUAccessFlags = (uint) usageInfo.CPUAccessFlags;
            desc.Usage = usageInfo.Usage;

            desc.MiscFlags = (uint)usageInfo.MiscFlags;

            var initialData = new D3D11_SUBRESOURCE_DATA();
            initialData.pSysMem = surface.GetNativePointer().ToPointer();

            ComPtr<ID3D11Buffer> pBuffer = new();
            fixed (ID3D11Device** ppDevice = _pDevice)
            {
                GraphicsException.SetInfoQueue();
                var result = (*ppDevice)->CreateBuffer(&desc, &initialData,
                                                        pBuffer.GetAddressOf());

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
    public RenderTargetView CreateRenderTargetView(Resource resource,
        ViewCreationInfo info)
    {
        return new RenderTargetView(NativeCreateView(), resource,
        new RenderTargetViewInfo()
        {
            ViewInfo = info,
            ResourceViewInfo = new ResourceViewInfo()
            {
                ResourceInfo = resource.ResourceInfo
            }
        },
        this);

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

    /// <summary>
    /// Creates a 2d texture.
    /// </summary>
    /// <param name="surface">The surface to create texture from.</param>
    /// <param name="usageInfo">Resource Usuage Info.</param>
    /// <param name="format">The format of the texture.</param>
    /// <returns>Created 2d texture.</returns>
    /// <exception cref="GraphicsException"></exception>
    /// <remarks>If the format set unknown the format will be taken from surface channels.</remarks>
    public Texture2D CreateTexture2D(Surface[] surfaces, TextureCreationInfo info)
    {
        Debug.Assert(surfaces.Length > 0,
            "Given surfaces to make textures are 0.");
        // validating surface types and sizes
        bool isF = surfaces[0].GetType().Match<FSurface>();
        bool isU = surfaces[0].GetType().Match<USurface>();
        var size = surfaces[0].Size;
        var channels = surfaces[0].Channels;
#if DEBUG
        for(var i = 1; i < surfaces.Length; i++)
        {
            bool f = surfaces[0].GetType().Match<FSurface>();
            bool u = surfaces[0].GetType().Match<USurface>();
            var dim = surfaces[0].Size;
            var chans = surfaces[0].Channels;

            Debug.Assert(isF == f,
                "Surfaces types does not match with each other.");
            Debug.Assert(isU == u,
                "Surfaces types does not match with each other.");
            Debug.Assert(size == dim,
                "Surfaces sizes does not match with each other.");
            Debug.Assert(channels == chans,
                "Surfaces channels does not match with each other.");
        }
#endif

        Debug.Assert(isF || isU,
            "Unknown / External Surface type is being used.");
    
        var formatToUse = info.Format == DXGI_FORMAT.DXGI_FORMAT_UNKNOWN ?
                                                isF? surfaces[0].Channels.ToFFormat() : 
                                                     surfaces[0].Channels.ToUFormat() :
                                         info.Format;

        return new Texture2D(NativeCreateTexture2D(), new TextureInfo()
        {
            SubtexturesCount = surfaces.Length,
            MipLevels = info.MipLevels,
            Size = size,
            Channels = channels,
            Format = formatToUse,
            UsageInfo = info.Usage
        },
        this);

        unsafe ComPtr<ID3D11Texture2D> NativeCreateTexture2D()
        {
            var desc = new D3D11_TEXTURE2D_DESC();
            desc.Width = (uint)size.Width;
            desc.Height = (uint)size.Height;
            desc.MipLevels = (uint)info.MipLevels;
            desc.ArraySize = (uint)surfaces.Length;
            desc.SampleDesc = new DXGI_SAMPLE_DESC
            {
                Quality = 0u,
                Count = 1u
            };
            desc.MiscFlags = (uint)info.Usage.MiscFlags;
            desc.Format = formatToUse;
            desc.Usage = info.Usage.Usage;
            desc.BindFlags = (uint)info.Usage.BindFlags;
            desc.CPUAccessFlags = (uint)info.Usage.CPUAccessFlags;

            var pInitialData = stackalloc D3D11_SUBRESOURCE_DATA[surfaces.Length];
            for (var i = 0; i < surfaces.Length; i++)
            {
                pInitialData[i] = new D3D11_SUBRESOURCE_DATA();
                pInitialData[i].pSysMem = surfaces[i].GetNativePointer().ToPointer();
                pInitialData[i].SysMemPitch = (uint)surfaces[i].GetSliceSize();
            }

            fixed (ID3D11Device** ppDevice = _pDevice)
            {
                var pTexture2D = new ComPtr<ID3D11Texture2D>();
                fixed (ID3D11Texture2D** ppTexture2D = pTexture2D)
                {
                    GraphicsException.SetInfoQueue();
                    var result = (*ppDevice)->CreateTexture2D(&desc, 
                        pInitialData, ppTexture2D);

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
        _adapter = adaper;

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