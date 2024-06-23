﻿namespace SharpEngineCore.Graphics;

internal sealed class SkyboxVariation : PipelineVariation
{
    public SkyboxVariation(
        InputLayout inputLayout,
        VertexBuffer vertexBuffer,
        IndexBuffer indexBuffer,
        VertexShader vertexShader,
        ConstantBuffer vertexTransformBuffer,
        PixelShader pixelShader,
        RenderTargetView renderTargetView,
        DepthStencilView depthView,
        RasterizerState rasterizerState)
    {
        InputAssembler = new InputAssembler()
        {
            Layout = inputLayout,
            VertexBuffer = vertexBuffer,
            IndexBuffer = indexBuffer,
            
            Flags = InputAssembler.BindFlags.VertexBuffer |
                    InputAssembler.BindFlags.IndexBuffer |
                    InputAssembler.BindFlags.Layout
        };

        VertexShaderStage = new VertexShaderStage()
        {
            VertexShader = vertexShader,
            ConstantBuffers = [vertexTransformBuffer],

            Flags = VertexShaderStage.BindFlags.VertexShader |
                    VertexShaderStage.BindFlags.ConstantBuffers
        };

        PixelShaderStage = new PixelShaderStage()
        {
            PixelShader = pixelShader,

            Flags = PixelShaderStage.BindFlags.PixelShader
        };

        Rasterizer = new Rasterizer()
        {
            RasterizerState = rasterizerState,

            Flags = Rasterizer.BindFlags.RasterizerState
        };

        OutputMerger = new OutputMerger()
        {
            DepthStencilView = depthView,
            RenderTargetViews = [renderTargetView],

            Flags = OutputMerger.BindFlags.RenderTargetViewAndDepthView,
        };

        VertexCount = vertexBuffer.VertexCount;
        IndexCount = indexBuffer.IndexCount;
        UseIndexRendering = false;

        _stages = [
            InputAssembler,
            VertexShaderStage, 
            PixelShaderStage,
            OutputMerger,
            Rasterizer];
    }
}