﻿using System.Runtime.InteropServices;

namespace SharpEngineCore.Graphics;

[StructLayout(LayoutKind.Sequential, Pack = 0, Size = 64)]
public struct LightConstantData : IFragmentable, ISurfaceable
{
    public FColor4 Position;
    public FColor4 Rotation;
    public FColor4 Color;
    public FColor4 AmbientColor;

    public int GetFragmentsCount()
    {
        return ToFragments().Length;
    }

    public int GetSize()
    {
        unsafe
        {
            return sizeof(TransformConstantData);
        }
    }

    public Fragment[] ToFragments()
    {
        return
            [
                Position.r,
                Position.g,
                Position.b,
                Position.a,

                Rotation.r,
                Rotation.g,
                Rotation.b,
                Rotation.a,

                Color.r,
                Color.g,
                Color.b,
                Color.a,

                AmbientColor.r,
                AmbientColor.g,
                AmbientColor.b,
                AmbientColor.a
            ];
    }

    public Surface ToSurface()
    {
        var surface = new FSurface(new(GetFragmentsCount(), 1));
        surface.SetLinearFragments(ToFragments());
        return surface;
    }
}
