using System.Diagnostics;

namespace SharpEngineCore.Graphics;

internal sealed class ObjLoader
{
    public Mesh Load(string fileName)
    {
        Debug.Assert(fileName != null && fileName != string.Empty,
            "Must provide file name to load mesh from.");

        Debug.Assert(File.Exists(fileName),
            "Files does not exists, modal cant be loaded.");

        var mesh = new Mesh();
        var vertices = new List<Vertex>(1000);
        var indices = new List<Index>(1000);

        var lines = File.ReadAllLines(fileName);
        var normalsFound = 0;

        foreach(var line in lines)
        {
            if(line.Length < 2)
                continue;

            if (line[0] != 'v' || line[1] == 't')
                continue;

            var value = ParseCoord(line);

            if (line[1] == 'n')
            {
                // normal here.
                var vertex = vertices[normalsFound];
                vertex.Normal = new(value.x, value.y, value.z, 0);

                vertices[normalsFound] = vertex;

                normalsFound++;
                continue;
            }

            // vertex here.
            var v = new Vertex();
            v.Position.r = value.x;
            v.Position.g = value.y;
            v.Position.b = value.z;

            v.Color = new(1, 1, 1, 1);

            vertices.Add(v);
        }

        foreach (var line in lines)
        {
            if (line.Length < 2)
                continue;

            if (line[0] != 'f')
                continue;

            var values = line.Substring(2)
                         .Split(' ');

            foreach(var value in values)
            {
                var nums = value.Split('/');
                var index = new Index(Convert.ToUInt32(nums[0]) -1);
                indices.Add(index);
            }
        }

        mesh.Vertices = [.. vertices];
        mesh.Indices = [.. indices];
        return mesh;

        (float x, float y, float z) ParseCoord(string value)
        {
            (float x, float y, float z) result = (0, 0, 0);

            var cleanedValue = value.Substring(3);

            var values = cleanedValue.Split([' ']);

            Debug.Assert(values.Length == 3,
                "Error in parsing vertex.");

            result.x = (float)Convert.ToDouble(values[0]);
            result.y = (float)Convert.ToDouble(values[1]);
            result.z = (float)Convert.ToDouble(values[2]);

            return result;
        }
    }

    public ObjLoader()
    { }
}
