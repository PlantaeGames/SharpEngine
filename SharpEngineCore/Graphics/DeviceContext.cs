using System.Diagnostics;

using TerraFX.Interop.DirectX;
using TerraFX.Interop.Windows;

namespace SharpEngineCore.Graphics;

internal abstract partial class DeviceContext
{
    protected readonly ComPtr<ID3D11DeviceContext> _pContext;

    public void ClearDepthStencilView(DepthStencilView view, DepthStencilClearInfo info)
    {
        NativeClearDepthStencilView();

        unsafe void NativeClearDepthStencilView()
        {
            fixed(ID3D11DeviceContext** ppContext = _pContext)
            {
                GraphicsException.SetInfoQueue();
                (*ppContext)->ClearDepthStencilView(view.GetNativePtr(),
                    (uint)info.ClearFlags, info.Depth, info.Stencil);

                Debug.Assert(GraphicsException.CheckIfAny() == false,
                    "Error in clearing depth stencil view.");
            }
        }
    }

    public void OMSetDepthStencilState(DepthStencilState state)
    {
        NativeOMSetDepthStencilState();

        unsafe void NativeOMSetDepthStencilState()
        {
            fixed(ID3D11DeviceContext** ppContext = _pContext)
            {
                GraphicsException.SetInfoQueue();
                (*ppContext)->OMSetDepthStencilState(state.GetNativePtr(), 1u);

                Debug.Assert(GraphicsException.CheckIfAny() == false,
                    "Error in setting depth stencil state of output merger");
            }
        }
    }

    public void VSSetShaderResources(ShaderResourceView[] views, int startIndex)
    {
        NativeVSSetShaderResources();

        unsafe void NativeVSSetShaderResources()
        {
            var count = views.Length;

            var pViews = stackalloc ID3D11ShaderResourceView*[count];
            for (var i = 0; i < count; i++)
            {
                pViews[i] = views[i].GetNativePtr().Get();
            }

            fixed (ID3D11DeviceContext** ppContext = _pContext)
            {
                GraphicsException.SetInfoQueue();
                (*ppContext)->VSSetShaderResources((uint)startIndex, (uint)count, pViews);

                Debug.Assert(GraphicsException.CheckIfAny() == false,
                    "Error in vertex shader setting resource views.");
            }
        }
    }

    public void VSSetConstantBuffers(ConstantBuffer[] buffers, int startIndex)
    {
        NativeVSConstantBuffers();

        unsafe void NativeVSConstantBuffers()
        {
            var count = buffers.Length;

            var pBuffers = stackalloc ID3D11Buffer*[count];
            for (var i = 0; i < count; i++)
            {
                pBuffers[i] = buffers[i].GetNativePtr().Get();
            }

            fixed (ID3D11DeviceContext** ppContext = _pContext)
            {
                GraphicsException.SetInfoQueue();
                (*ppContext)->VSSetConstantBuffers((uint)startIndex, (uint)count, pBuffers);

                Debug.Assert(GraphicsException.CheckIfAny() == false,
                    "Error in setting vertex shaders constant buffers.");
            }
        }
    }

    public void PSSetConstantBuffers(ConstantBuffer[] buffers, int startIndex)
    {
        NativePSSetConstantBuffers();

        unsafe void NativePSSetConstantBuffers()
        {
            var count = buffers.Length;

            var pBuffers = stackalloc ID3D11Buffer*[count];
            for (var i = 0; i < count; i++)
            {
                pBuffers[i] = buffers[i].GetNativePtr().Get();
            }

            fixed (ID3D11DeviceContext** ppContext = _pContext)
            {
                GraphicsException.SetInfoQueue();
                (*ppContext)->PSSetConstantBuffers((uint)startIndex, (uint)count, pBuffers);

                Debug.Assert(GraphicsException.CheckIfAny() == false,
                    "Error in setting pixel shaders constant buffers.");
            }
        }
    }

    public void PSSetSamplers(Sampler[] samplers, int startIndex)
    {
        NativePSSetSamplers();

        unsafe void NativePSSetSamplers()
        {
            var count = samplers.Length;

            var pSamplers = stackalloc ID3D11SamplerState*[count];
            for (var i = 0; i < count; i++)
            {
                pSamplers[i] = samplers[i].GetNativePtr().Get();
            }

            fixed(ID3D11DeviceContext** ppContext = _pContext)
            {
                GraphicsException.SetInfoQueue();
                (*ppContext)->PSSetSamplers((uint)startIndex, (uint)count, pSamplers);

                Debug.Assert(GraphicsException.CheckIfAny() == false,
                    "Error in setting pixel shader samplers.");
            }
        }
    }

    public void PSSetShaderResources(ShaderResourceView[] views, int startIndex)
    {
        NativePSSetShaderResources();

        unsafe void NativePSSetShaderResources()
        {
            var count = views.Length;

            var pViews = stackalloc ID3D11ShaderResourceView*[count];
            for(var i = 0; i < count; i++)
            {
                pViews[i] = views[i].GetNativePtr().Get();
            }

            fixed(ID3D11DeviceContext** ppContext = _pContext)
            {
                GraphicsException.SetInfoQueue();
                (*ppContext)->PSSetShaderResources((uint)startIndex, (uint)count, pViews);

                Debug.Assert(GraphicsException.CheckIfAny() == false,
                    "Error in Pixel shader setting resource views.");
            }
        }
    }

    public void Unmap(Resource resource, MapInfo info)
    {
        NativeUnmap();

        unsafe void NativeUnmap()
        {
            fixed(ID3D11DeviceContext** ppContext = _pContext)
            {
                GraphicsException.SetInfoQueue();
                (*ppContext)->Unmap(resource.GetNativePtrAsResource(),
                    (uint)info.SubResourceIndex);

                if(GraphicsException.CheckIfAny())
                {
                    GraphicsException.ThrowLastGraphicsException(
                        "Failed to unmap memory");
                }
            }
        }
    }

    public MappedSubResource Map(Resource resource, MapInfo info)
    {
        return new(NativeMap(), info);

        unsafe D3D11_MAPPED_SUBRESOURCE NativeMap()
        {
            var map = new D3D11_MAPPED_SUBRESOURCE();

            fixed(ID3D11DeviceContext** ppContext = _pContext)
            {
                GraphicsException.SetInfoQueue();
                var result = (*ppContext)->Map(resource.GetNativePtrAsResource(),
                    (uint)info.SubResourceIndex, info.MapType, 0u, &map);

                if(result.FAILED)
                {
                    // error here
                    GraphicsException.ThrowLastGraphicsException(
                        $"Failed to map memory\nError Code: {result}");
                }
            }

            return map;
        }
    }

    public void DrawIndexed(int indexCount, int startIndex)
    {
        NativeDrawIndexed();

        unsafe void NativeDrawIndexed()
        {
            fixed (ID3D11DeviceContext** ppContext = _pContext)
            {
                GraphicsException.SetInfoQueue();
                (*ppContext)->DrawIndexed((uint)indexCount, (uint)startIndex, 0);

                if (GraphicsException.CheckIfAny())
                {
                    // error here
                    GraphicsException.ThrowLastGraphicsException(
                        "Error during submiting draw call.");
                }
            }
        }
    }

    public void ClearState()
    {
        NativeClearState();

        unsafe void NativeClearState()
        {
            fixed (ID3D11DeviceContext** pContext = _pContext)
            {
                GraphicsException.SetInfoQueue();
                (*pContext)->ClearState();

                Debug.Assert(GraphicsException.CheckIfAny() == false,
                    "Error in clearing pipeline state.");
            }
        }
    }

    public void Draw(int vertexCount, int startIndex)
    {
        NativeDraw();

        unsafe void NativeDraw()
        {
            fixed (ID3D11DeviceContext** ppContext = _pContext)
            {
                GraphicsException.SetInfoQueue();
                (*ppContext)->Draw((uint)vertexCount, (uint)startIndex);

                if (GraphicsException.CheckIfAny())
                {
                    // error here
                    GraphicsException.ThrowLastGraphicsException(
                        "Error during submiting draw call.");
                }
            }
        }
    }

    public void OMSetRenderTargets(RenderTargetView[] views,
        DepthStencilView depthStencilView)
    {
        NativeOMSetRenderTargets();

        unsafe void NativeOMSetRenderTargets()
        {
            var ppViews = stackalloc ID3D11RenderTargetView*[views.Length];
            for (var i = 0; i < views.Length; i++)
            {
                ppViews[i] = views[i].GetNativePtr();
            }

            fixed (ID3D11DeviceContext** ppContext = _pContext)
            {
                GraphicsException.SetInfoQueue();
                (*ppContext)->OMSetRenderTargets((uint)views.Length,
                    ppViews, depthStencilView.GetNativePtr());

                Debug.Assert(GraphicsException.CheckIfAny() == false,
                    "Failed to set render target view to output merger");
            }
        }
    }

    public void RSSetViewports(Viewport[] ports)
    {
        NativeRSSetViewports();

        unsafe void NativeRSSetViewports()
        {
            var rPorts = new D3D11_VIEWPORT[ports.Length];
            for (var i = 0; i < ports.Length; i++)
            {
                rPorts[i] = ports[i].Info;
            }

            fixed (D3D11_VIEWPORT* pPorts = rPorts)
            {

                fixed (ID3D11DeviceContext** ppContext = _pContext)
                {
                    GraphicsException.SetInfoQueue();

                    (*ppContext)->RSSetViewports((uint)ports.Length, pPorts);

                    Debug.Assert(GraphicsException.CheckIfAny() == false,
                        "Failed to set view ports to rasterizer.");
                }
            }
        }
    }

    /// <summary>
    /// Binds the pixel shader to the pipeline.
    /// </summary>
    /// <param name="shader"></param>
    public void PSSetShader(PixelShader shader)
    {
        NativePSSetShader();

        unsafe void NativePSSetShader()
        {
            var pShader = shader.GetNativePtr().Get();

            fixed (ID3D11DeviceContext** ppDeviceContext = _pContext)
            {
                GraphicsException.SetInfoQueue();
                (*ppDeviceContext)->PSSetShader(pShader,
                    (ID3D11ClassInstance**)IntPtr.Zero, 0u);

                Debug.Assert(GraphicsException.CheckIfAny() == false,
                    "Failed to set Vertex Shader");
            }
        }
    }

    /// <summary>
    /// Binds vertex shader to pipeline
    /// </summary>
    /// <param name="shader">The vertex shader to bind</param>
    public void VSSetShader(VertexShader shader)
    {
        NativeVSSetShader();

        unsafe void NativeVSSetShader()
        {
            var pShader = shader.GetNativePtr().Get();

            fixed (ID3D11DeviceContext** ppDeviceContext = _pContext)
            {
                GraphicsException.SetInfoQueue();
                (*ppDeviceContext)->VSSetShader(pShader,
                    (ID3D11ClassInstance**)IntPtr.Zero, 0u);

                Debug.Assert(GraphicsException.CheckIfAny() == false,
                    "Failed to set Vertex Shader");
            }
        }
    }

    /// <summary>
    /// Binds the given vertex Buffers to the input assembler pipline stage
    /// </summary>
    /// <param name="vertexBuffers">The vertex buffers to bind.</param>
    /// <param name="startSlot">The slot to start binding from.</param>
    /// <remarks>Valid binding slots are 0 - 15.</remarks>
    public void IASetIndexBuffer(IndexBuffer buffer)
    {
        NativeIASetVertexBuffer();

        unsafe void NativeIASetVertexBuffer()
        {
            var pBuffer = buffer.GetNativePtr().Get();
            var format = DXGI_FORMAT.DXGI_FORMAT_R32_UINT;
            var offset = 0u;

            fixed (ID3D11DeviceContext** ppContext = _pContext)
            {
                GraphicsException.SetInfoQueue();
                (*ppContext)->IASetIndexBuffer(pBuffer, format, offset);

                Debug.Assert(GraphicsException.CheckIfAny() == false,
                    "Failed to set Input Assembler Vertex Buffers.");
            }
        }
    }

    /// <summary>
    /// Binds the given vertex Buffers to the input assembler pipline stage
    /// </summary>
    /// <param name="buffers">The vertex buffers to bind.</param>
    /// <param name="startSlot">The slot to start binding from.</param>
    /// <remarks>Valid binding slots are 0 - 15.</remarks>
    public void IASetVertexBuffer(VertexBuffer[] buffers, int startSlot)
    {
        Debug.Assert(buffers.Length > 0,
            "Vertex buffers can't be zero to bind to Input Assembler.");

        NativeIASetVertexBuffer();

        unsafe void NativeIASetVertexBuffer()
        {
            var count = (uint)buffers.Length;

            var ppBuffers = stackalloc ID3D11Buffer*[buffers.Length];
            for (var i = 0u; i < count; i++)
            {
                ppBuffers[i] = buffers[i].GetNativePtr();
            }

            var strides = stackalloc uint[(int)count];
            var offsets = stackalloc uint[(int)count];
            for (var i = 0u; i < count; i++)
            {
                strides[i] = (uint)buffers[i].Info.Layout.GetDataTypeSize();
                offsets[i] = 0u;
            }

            fixed (ID3D11DeviceContext** ppContext = _pContext)
            {
                GraphicsException.SetInfoQueue();
                (*ppContext)->IASetVertexBuffers((uint)startSlot, count,
                  ppBuffers, strides, offsets);

                Debug.Assert(GraphicsException.CheckIfAny() == false,
                    "Failed to set Input Assembler Vertex Buffers.");
            }
        }
    }

    public void IASetTopology(Topology topology)
    {
        NativeIASetTopology();

        unsafe void NativeIASetTopology()
        {
            fixed (ID3D11DeviceContext** ppContext = _pContext)
            {
                GraphicsException.SetInfoQueue();

                (*ppContext)->IASetPrimitiveTopology((D3D_PRIMITIVE_TOPOLOGY)topology);

                Debug.Assert(GraphicsException.CheckIfAny() == false,
                    "Error in setting input assembler primitive topology");
            }
        }
    }

    public void IASetInputLayout(InputLayout inputLayout)
    {
        NativeIASetInputLayout();

        unsafe void NativeIASetInputLayout()
        {
            fixed (ID3D11DeviceContext** ppContext = _pContext)
            {
                GraphicsException.SetInfoQueue();
                (*ppContext)->IASetInputLayout(inputLayout.GetNativePtr());

                Debug.Assert(GraphicsException.CheckIfAny() == false,
                    "Failed to set Input Assembler Layout.");
            }
        }
    }

    public void ClearRenderTargetView(RenderTargetView renderTargetView, FColor4 color)
    {
        NativeClearRenderTargetView(color);

        unsafe void NativeClearRenderTargetView(FColor4 color)
        {
            fixed(ID3D11DeviceContext** ppContext = _pContext)
            {
                fixed (ID3D11RenderTargetView** ppView = renderTargetView.GetNativePtr())
                {
                    GraphicsException.SetInfoQueue();
                    (*ppContext)->ClearRenderTargetView(*ppView, (float*)&color);

                    if(GraphicsException.CheckIfAny())
                    {
                        // error here
                        GraphicsException.ThrowLastGraphicsException(
                            "Failed to clear render target view");
                    }
                }
            }
        }
    }

    protected DeviceContext(ComPtr<ID3D11DeviceContext> pDeviceContext)
    {
        _pContext = new(pDeviceContext);
    }
}

internal sealed class ImmediateDeviceContext : DeviceContext
{
    public ImmediateDeviceContext(ComPtr<ID3D11DeviceContext> pDeviceContext)
        : base(pDeviceContext) { }
}