namespace SharpEngineCore.Graphics;

internal sealed class PerspectiveCamera
{
    public readonly ConstantBuffer Transform;
    
    public readonly Viewport Viewport;
    public readonly float AspectRatio;

    public PerspectiveCamera(Size size, ConstantBuffer transformBuffer)
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

        AspectRatio = (float)size.Height / (float)size.Width;
        Transform = transformBuffer;
    }
}
