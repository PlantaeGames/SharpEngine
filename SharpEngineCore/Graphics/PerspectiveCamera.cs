namespace SharpEngineCore.Graphics;

public sealed class PerspectiveCamera
{
    public readonly TransformConstantData Transform;
    
    public readonly Viewport Viewport;

    public PerspectiveCamera(Size size)
    { 
        Viewport = new Viewport()
        {
            Info = new TerraFX.Interop.DirectX.D3D11_VIEWPORT()
            {
                TopLeftX = 0f,
                TopLeftY = 0f,
                Width = size.Width,
                Height = size.Height,
                MinDepth = 0f,
                MaxDepth = 1f
            }
        };
    }
}
