namespace SharpEngineEditor.ImGui.Backend;

public readonly struct BufferInfo(Size size, int byteStride, int bytesSize,
    Type layout, ResourceUsageInfo usageInfo)
{ 
    public readonly Size SurfaceSize { get; init; } = size;
    public readonly int ByteStride { get; init; } = byteStride;
    public readonly int BytesSize { get; init; } = bytesSize;
    public readonly Type Layout { get; init; } = layout;
    public readonly ResourceUsageInfo UsageInfo { get; init; } = usageInfo;
}
