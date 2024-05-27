namespace SharpEngineCore.Graphics;

public sealed class CubemapInfo
{
    public string Left { get; init; } = "Textures\\Skybox\\left.bmp";
    public string Right { get; init; } = "Textures\\Skybox\\right.bmp";
    public string Up { get; init; } = "Textures\\Skybox\\up_invert.bmp";
    public string Down { get; init; } = "Textures\\Skybox\\down.bmp";
    public string Front { get; init; } = "Textures\\Skybox\\front.bmp";
    public string Back { get; init; } = "Textures\\Skybox\\back.bmp";

    public CubemapInfo()
    { }
}
