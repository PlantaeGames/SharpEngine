﻿using TerraFX.Interop.DirectX;

namespace SharpEngineCore.Graphics;

public sealed class Material
{
    public bool UseIndexedRendering = false;

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

    public D3D11_CULL_MODE CullMode;
    public D3D11_FILL_MODE FillMode;

    public Material()
    { }
}
