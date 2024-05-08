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
    public PixelShader[] PixelShaders { get; init; }

    public PixelShaderStage()
    { }

    public void Bind(DeviceContext context)
    {
        throw new NotImplementedException();
    }
}

internal sealed class VertexShaderStage : IPipelineStage
{
    public VertexShader[] VertexShaders { get; init; }

    public VertexShaderStage()
    { }

    public void Bind(DeviceContext context)
    {
        throw new NotImplementedException();
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

internal sealed class Rasterizer : IPipelineStage
{
    public Rasterizer()
    { }


    public void Bind(DeviceContext context)
    {
        throw new NotImplementedException();
    }
}

internal sealed class InputLayout
{
    private InputLayoutInfo _info;
    private ComPtr<ID3D11InputLayout> _ptr;

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

    public InputAssembler(InputLayout layout,
                          VertexBuffer vertexBuffer,
                          IndexBuffer indexBuffer)
    {
        Layout = layout;
        VertexBuffer = vertexBuffer;
        IndexBuffer = indexBuffer;
    }

    public InputAssembler()
    { }

    public void Bind(DeviceContext context)
    {
        throw new NotImplementedException();
    }
}