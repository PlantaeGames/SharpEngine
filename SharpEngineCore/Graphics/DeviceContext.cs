using System.Diagnostics;

using TerraFX.Interop.DirectX;
using TerraFX.Interop.Windows;

namespace SharpEngineCore.Graphics;

internal abstract class DeviceContext
{
    protected readonly ComPtr<ID3D11DeviceContext> _pContext;

    public void IASetVertexBuffer(VertexBuffer[] vertexBuffers, int startSlot)
    {
        NativeIASetVertexBuffer();

        unsafe void NativeIASetVertexBuffer()
        {
            fixed(ID3D11DeviceContext** ppContext = _pContext)
            {
                GraphicsException.SetInfoQueue();
                (*ppContext)->IASetVertexBuffers(startSlot, vertexBuffers.Length,
                    , )

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

