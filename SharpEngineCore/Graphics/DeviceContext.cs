using System.Diagnostics;

using TerraFX.Interop.DirectX;
using TerraFX.Interop.Windows;

namespace SharpEngineCore.Graphics;

internal abstract class DeviceContext
{
    protected readonly ComPtr<ID3D11DeviceContext> _pContext;

    public void RSSetViewports(Viewport[] ports)
    {
        NativeRSSetViewports();

        unsafe void NativeRSSetViewports()
        {
            var rPorts = new D3D11_VIEWPORT[ports.Length];
            for(var i = 0; i < ports.Length; i++)
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

            fixed(ID3D11DeviceContext** ppDeviceContext = _pContext)
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
            for(var i = 0u; i < count; i++)
            {
                ppBuffers[i] = buffers[i].GetNativePtr();
            }
            
            var strides = stackalloc uint[(int)count];
            var offsets = stackalloc uint[(int)count];
            for(var i = 0u; i < count; i++)
            {
                strides[i] = 0u;
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

    public void ClearRenderTargetView(RenderTargetView renderTargetView, Fragment color)
    {
        NativeClearRenderTargetView(color);

        unsafe void NativeClearRenderTargetView(Fragment color)
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

