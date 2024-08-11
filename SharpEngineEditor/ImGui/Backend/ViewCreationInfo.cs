﻿using TerraFX.Interop.DirectX;

namespace SharpEngineEditor.ImGui.Backend;

internal readonly struct ViewCreationInfo()
{
    public readonly int TextureMipLevels { get; init; } = 1;
    public readonly int TextureMipIndex { get; init; }
    public readonly int TextureArraySliceCount { get; init; }
    public readonly int TextureArraySliceIndex { get; init; }
    public readonly int BufferByteStride { get; init; }
    public readonly int BufferBytesSize { get; init; }
    public readonly ViewResourceType ViewResourceType { get; init; }
    public readonly DXGI_FORMAT Format { get; init; }
    public readonly D3D11_BUFFER_UAV_FLAG UAVBufferFlags { get; init; }
}
