namespace SharpEngineCore.Graphics;

public sealed class Material
{
    public readonly bool UseIndexedRendering = false;

    public readonly Topology Topology;

    public readonly ShaderModule VertexShader;
    public readonly ConstantBuffer[] VertexConstantBuffers = [];
    public readonly Texture[] VertexTextures = [];
    public readonly Sampler[] VertexSamplers = [];
    public readonly Buffer[] VertexBuffers = [];
    
    public readonly ShaderModule PixelShader;
    public readonly ConstantBuffer[] PixelConstantBuffers = [];
    public readonly Texture[] PixelTextures = [];
    public readonly Sampler[] PixelSamplers = [];
    public readonly Buffer[] PixelBuffers = [];

    public Material()
    { }
}
