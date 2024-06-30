namespace SharpEngineCore.Graphics;

internal interface IPipelineStage
{
    void Bind(DeviceContext context);
    void Unbind(DeviceContext context);
}
