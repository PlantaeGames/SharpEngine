namespace SharpEngineCore.Graphics;

public sealed class Material
{
    public readonly bool UseIndexedRendering = false;

    public Topology Topology;

    public ShaderModule VertexShader;
    public ConstantBuffer[] VertexConstantBuffers = [];
    public Texture[] VertexTextures = [];
    public Sampler[] VertexSamplers = [];
    public Buffer[] VertexBuffers = [];
    
    public ShaderModule PixelShader;
    public ConstantBuffer[] PixelConstantBuffers = [];
    public Texture[] PixelTextures = [];
    public Sampler[] PixelSamplers = [];
    public Buffer[] PixelBuffers = [];

    public Material()
    { }
}
