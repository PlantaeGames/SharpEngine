using System.Runtime.InteropServices;

using TerraFX.Interop.DirectX;
using TerraFX.Interop.Windows;

namespace SharpEngineCore.Graphics;

internal sealed class InputLayoutInfo
{
    public D3D_PRIMITIVE_TOPOLOGY Topology { get; init; }
    public IFragmentable Layout { get; init; }
    public VertexShader VertexShader { get; init; }

    public InputLayoutInfo()
    { }

    public InputLayoutInfo(
        D3D_PRIMITIVE_TOPOLOGY topology,
        IFragmentable layout,
        VertexShader vertexShader)
    {
        this.Topology = topology;
        this.Layout = layout;
        this.VertexShader = vertexShader;
    }
}

public interface IFragmentable
{
    public Fragment[] ToFragments();
    public int GetFragmentsCount();
}

[StructLayout(LayoutKind.Sequential, Pack = 0, Size = 64)]
public struct Vertex : IFragmentable
{
    public Fragment Position;
    public Fragment Normal;
    public Fragment Color;
    public Fragment TexCoord;

    public int GetFragmentsCount()
    {
        return ToFragments().Length;
    }

    public Fragment[] ToFragments()
    {
        return [Position, Normal, Color, TexCoord];
    }
}

internal sealed class PixelShaderStage : IPipelineStage
{
    public PixelShader PixelShader { get; init; }

    public PixelShaderStage(PixelShader pixelShader)
    {
        PixelShader = pixelShader;
    }

    public PixelShaderStage()
    { }

    public void Bind(DeviceContext context)
    {
        context.PSSetShader(PixelShader);
    }
}

internal sealed class VertexShaderStage : IPipelineStage
{
    public VertexShader VertexShader { get; init; }

    public VertexShaderStage(VertexShader vertexShader)
    {
        VertexShader = vertexShader;
    }

    public VertexShaderStage()
    { }

    public void Bind(DeviceContext context)
    {
        context.VSSetShader(VertexShader);
    }
}

internal sealed class OutputMerger : IPipelineStage
{
    public OutputMerger()
    { }

    public void Bind(DeviceContext context)
    {
        throw new NotImplementedException();
    }
}

internal readonly struct Viewport
{
    public readonly D3D11_VIEWPORT Info { get; init; }

    public Viewport(D3D11_VIEWPORT info)
    {
        Info = info;
    }

    public Viewport()
    { }
}

internal sealed class Rasterizer : IPipelineStage
{
    private Viewport[] Viewports { get; init; }

    public Rasterizer(Viewport[] viewports)
    {
        Viewports = viewports;
    }

    public Rasterizer()
    { }


    public void Bind(DeviceContext context)
    {
        context.RSSetViewports(Viewports);
    }
}

internal sealed class InputLayout
{
    private readonly InputLayoutInfo _info;
    private readonly ComPtr<ID3D11InputLayout> _ptr;

    public ComPtr<ID3D11InputLayout> GetNativePtr() => new(_ptr);

    public InputLayout(ComPtr<ID3D11InputLayout> pInputLayout,
                       InputLayoutInfo info)
    {
        _ptr = new(pInputLayout);
        _info = info;
    }
}

internal sealed class InputAssembler : IPipelineStage
{
    private InputLayout Layout { get; init; }
    private VertexBuffer VertexBuffer { get; init; }
    private IndexBuffer IndexBuffer { get; init; }

    public void Bind(DeviceContext context)
    {
        context.IASetInputLayout(Layout);
        context.IASetVertexBuffer([VertexBuffer], 0);
        context.IASetIndexBuffer(IndexBuffer);
    }

    public InputAssembler(InputLayout layout,
                          VertexBuffer vertexBuffer,
                          IndexBuffer indexBuffer)
    {
        Layout = layout;
        VertexBuffer = vertexBuffer;
        IndexBuffer = indexBuffer;
    }

    public InputAssembler()
    {
    }
}