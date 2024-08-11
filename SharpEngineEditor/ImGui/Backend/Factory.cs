﻿using SharpEngineEditor.ImGui.Backend;
using TerraFX.Interop.DirectX;
using TerraFX.Interop.Windows;

namespace SharpEngineEditor.ImGui.Backend;

internal sealed class Factory
{
    private ComPtr<IDXGIFactory> _pFactory;

    private static Factory _instance;
    private static object _instanceLock = new();

    private const uint MAX_ENUMERATE_DEVICE_COUNT = 12u;

    public Swapchain CreateSwapchain(Window window, Device device)
    {
        var format = DXGI_FORMAT.DXGI_FORMAT_R8G8B8A8_UNORM;

        return new Swapchain(NativeCreateSwapchain(),
            new SwapchainInfo()
            {
                Format = format
            },
            window, device);

        unsafe ComPtr<IDXGISwapChain> NativeCreateSwapchain()
        {
            var desc = new DXGI_SWAP_CHAIN_DESC();

            desc.BufferDesc = new DXGI_MODE_DESC();
            desc.BufferDesc.Width = 0u;
            desc.BufferDesc.Height = 0u;
            desc.BufferDesc.RefreshRate = new DXGI_RATIONAL
            {
                Denominator = 0u,
                Numerator = 0u
            };
            desc.BufferDesc.Format = format;
            desc.BufferDesc.ScanlineOrdering = DXGI_MODE_SCANLINE_ORDER.DXGI_MODE_SCANLINE_ORDER_UNSPECIFIED;
            desc.BufferDesc.Scaling = DXGI_MODE_SCALING.DXGI_MODE_SCALING_UNSPECIFIED;

            desc.BufferCount = 1u;
            desc.BufferUsage = DXGI.DXGI_USAGE_RENDER_TARGET_OUTPUT;

            desc.OutputWindow = window.HWnd;
            desc.Windowed = true;

            desc.Flags = 0u;

            desc.SwapEffect = DXGI_SWAP_EFFECT.DXGI_SWAP_EFFECT_DISCARD;

            desc.SampleDesc = new DXGI_SAMPLE_DESC
            {
                Quality = 0u,
                Count = 1u
            };

            var pSwapchain = new ComPtr<IDXGISwapChain>();
            fixed(IDXGISwapChain** ppSwapchain = pSwapchain)
            {
                fixed(IDXGIFactory** ppFactory = _pFactory)
                {
                    var pUnknown = new ComPtr<IUnknown>();
                    device.GetNativePtr().As(&pUnknown);

                    fixed(ID3D11Device** ppDevice = device.GetNativePtr())
                    {
                        GraphicsException.SetInfoQueue();
                        var result = (*ppFactory)->CreateSwapChain(pUnknown.Get(),
                            &desc, ppSwapchain);

                        if(result.FAILED)
                        {
                            // error here.
                            GraphicsException.ThrowLastGraphicsException(
                                $"Failed to create swapchain\nError Code: {result}");
                        }
                    }
                }
            }

            return pSwapchain;
        }
    }

    public Adapter[] GetAdpters()
    {
        var pAdapters = NativeEnumerate();

        var adapters = new Adapter[pAdapters.Length];
        for (var i = 0; i < adapters.Length; i++)
        {
            adapters[i] = new Adapter(pAdapters[i]);
        }

        return adapters;

        unsafe ComPtr<IDXGIAdapter>[] NativeEnumerate()
        {
            var foundList = new List<uint>((int)MAX_ENUMERATE_DEVICE_COUNT);

            fixed (IDXGIFactory** ppFactory = _pFactory)
            {
                for (var i = 0u; i < MAX_ENUMERATE_DEVICE_COUNT; i++)
                {
                    IDXGIAdapter* pTemp = (IDXGIAdapter*)IntPtr.Zero;
                    if((*ppFactory)->EnumAdapters(i, &pTemp).SUCCEEDED)
                        foundList.Add(i);
                }

                var pAdapters = new ComPtr<IDXGIAdapter>[foundList.Count];
                for (var i = 0; i < pAdapters.Length; i++)
                {
                    pAdapters[i] = new ComPtr<IDXGIAdapter>();
                }
                for (var i = 0; i <  foundList.Count; i++)
                {
                    fixed (IDXGIAdapter** ppAdapter = pAdapters[i])
                    {
                        GraphicsException.SetInfoQueue();
                        var result = (*ppFactory)->EnumAdapters(foundList[i], ppAdapter);

                        if (result.FAILED)
                        {
                            // error here.
                            GraphicsException.ThrowLastGraphicsException
                                ($"Failed to get adapter.\nError Code: {result}");
                        }
                    }
                }

                return pAdapters;
            }
        }
    }

    public static Factory GetInstance()
    {
        lock (_instanceLock)
        {
            _instance ??= new Factory();

            return _instance;
        }
    }

    private Factory()
    {
        Initialize();
    }

    private void Initialize()
    {
        NativeInitialize();

        unsafe void NativeInitialize()
        {
            fixed (IDXGIFactory** ppFactory = _pFactory)
            {
                var uuid = typeof(IDXGIFactory).GUID;

                GraphicsException.SetInfoQueue();
                var result = DirectX.CreateDXGIFactory(&uuid, (void**)ppFactory);

                if(result.FAILED)
                {
                    // error here.
                    GraphicsException.ThrowLastGraphicsException(
                        $"Failed to create DXGI Factory.\nError Code: {result}");
                }
            }
        }
    }
}