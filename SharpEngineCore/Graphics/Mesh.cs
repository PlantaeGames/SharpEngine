namespace SharpEngineCore.Graphics;

public sealed class Mesh
{
    public Vertex[] Vertices;
    public Index[] Indices;

    public Fragment[] ToVertexFragments()
    {
        int vertexCount = Vertices.Length;
        int perVertexFragments = Vertices[0].GetFragmentsCount();

        var vertexFragments = new Fragment[vertexCount * perVertexFragments];
        for (var x = 0; x < vertexCount; x++)
        {
            var offset = x * perVertexFragments;
            var fragments = Vertices[x].ToFragments();

            for (var f = 0; f < fragments.Length; f++)
            {
                vertexFragments[offset + f] = fragments[f];
            }
        }

        return vertexFragments;
    }

    public Unit[] ToIndexUnits()
    {
        int indexCount = Indices.Length;
        int perIndexUnits = Indices[0].GetUnitCount();

        var indexUnits = new Unit[indexCount * perIndexUnits];
        for (var x = 0; x < indexCount; x++)
        {
            var offset = x * perIndexUnits;
            var units = Indices[x].ToUnits();

            for (var f = 0; f < units.Length; f++)
            {
                indexUnits[offset + f] = units[f];
            }
        }

        return indexUnits;
    }
}
