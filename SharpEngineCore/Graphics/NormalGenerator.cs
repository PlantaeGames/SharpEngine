using System.Diagnostics;

using SharpEngineCore.Utilities;

namespace SharpEngineCore.Graphics;

internal sealed class NormalGenerator
{
    public void GenerateForTriangles(ref Vertex[] vertices)
    {
        Debug.Assert(vertices.Length % 3 == 0,
            "Some triangles are missing their vertices.");

        for(var i =  0; i < vertices.Length; i+=3)
        {
            var a = vertices[i + 0].Position;
            var b = vertices[i + 1].Position;
            var c = vertices[i + 2].Position;

            var n = Normal.CalculateTriangleNormal(
                    new (a.r, a.g, a.b),
                    new(b.r, b.g, b.b),
                    new(c.r, c.g, c.b));

            if(i >= 3)
            {

            }

            vertices[i + 0].Normal = new FColor4(n.X, n.Y, n.Z, 1);
            vertices[i + 1].Normal = new FColor4(n.X, n.Y, n.Z, 1);
            vertices[i + 2].Normal = new FColor4(n.X, n.Y, n.Z, 1);
        }
    }

    public NormalGenerator()
    { }
}
