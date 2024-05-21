namespace SharpEngineCore.Graphics;

public sealed class Mesh
{
    public Vertex[] Vertices;
    public Index[] Indices;

    public static Mesh Plane()
    {
        var plane = new Mesh();
        var gen = new NormalGenerator();

        plane.Vertices =
            [
                new Vertex()
                {
                    Position = new(0.5f, 0.5f, 0, 1),
                    Color = new(1f, 0, 0, 0),
                    Normal = new (),
                    TexCoord = new()
                },
                new Vertex()
                {
                    Position = new(-0.5f, 0.5f, 0, 1),
                    Color = new(0f, 1f, 0, 0),
                    Normal = new (),
                    TexCoord = new()
                },
                new Vertex()
                {
                    Position = new(0.5f, -0.5f, 0, 1),
                    Color = new(0f, 0, 1f, 0),
                    Normal = new (),
                    TexCoord = new()
                },
                new Vertex()
                {
                    Position = new(-0.5f, 0.5f, 0, 1),
                    Color = new(0f, 0, 1f, 0),
                    Normal = new (),
                    TexCoord = new()
                },
                new Vertex()
                {
                    Position = new(-0.5f, -0.5f, 0, 1),
                    Color = new(0f, 1, 0, 0),
                    Normal = new (),
                    TexCoord = new()
                },
                new Vertex()
                {
                    Position = new(0.5f, -0.5f, 0, 1),
                    Color = new(1, 0, 0, 0),
                    Normal = new (),
                    TexCoord = new()
                }
            ];

        plane.Indices =
            [
                new Index()
                {
                    Value = new (0)
                },
                new Index()
                {
                    Value = new (1)
                },
                new Index()
                {
                    Value = new (2)
                },
                new Index()
                {
                    Value = new (1)
                },
                new Index()
                {
                    Value = new (4)
                },
                new Index()
                {
                    Value = new (2)
                }
            ];

        gen.GenerateForTriangles(ref plane.Vertices);

        return plane;
    }


    public static Mesh Triangle()
    {
        var triangle = new Mesh();
        var gen = new NormalGenerator();

        triangle.Vertices =
            [
                new Vertex()
                {
                    Position = new(0f, 0.5f, 0, 1),
                    Color = new(1f, 0, 0, 0),
                    Normal = new (),
                    TexCoord = new()
                },
                new Vertex()
                {
                    Position = new(0.5f, -0.5f, 0, 1),
                    Color = new(0f, 1f, 0, 0),
                    Normal = new (),
                    TexCoord = new()
                },
                new Vertex()
                {
                    Position = new(-0.5f, -0.5f, 0, 1),
                    Color = new(0f, 0, 1f, 0),
                    Normal = new (),
                    TexCoord = new()
                }
            ];

        triangle.Indices =
            [
                new Index()
                {
                    Value = new (0)
                },
                new Index()
                {
                    Value = new (1)
                },
                new Index()
                {
                    Value = new (2)
                }
            ];

        gen.GenerateForTriangles(ref triangle.Vertices);

        return triangle;
    }

    public static Mesh Cube()
    {
        var mesh = new Mesh();
        var gen = new NormalGenerator();

        mesh.Vertices = 
            [
            // 1
                new ()
                {
                    Position = new(-0.5f, 0.5f, -0.5f, 0),
                    Color = new (1, 0, 0, 1)
                },
                new ()
                {
                    Position = new(0.5f, -0.5f, -0.5f, 0),
                    Color = new (1, 0, 0, 1)
                },
                new ()
                {
                    Position = new(-0.5f, -0.5f, -0.5f, 0),
                    Color = new (1, 0, 0, 1)
                },
                new ()
                {
                    Position = new(0.5f, 0.5f, -0.5f, 0),
                    Color = new (1, 0, 0, 1)
                }
           // 2
                ,
                new ()
                {
                    Position = new(0.5f, -0.5f, -0.5f, 0),
                    Color = new (0, 1, 0, 1)
                },
                new ()
                {
                    Position = new(0.5f, 0.5f, 0.5f, 0),
                    Color = new (0, 1, 0, 1)
                },
                new ()
                {
                    Position = new(0.5f, -0.5f, 0.5f, 0),
                    Color = new (0, 1, 0, 1)
                },
                new ()
                {
                    Position = new(0.5f, 0.5f, -0.5f, 0),
                    Color = new (0, 1, 0, 1)
                }
            // 3
                ,
                new ()
                {
                    Position = new(-0.5f, 0.5f, 0.5f, 0),
                    Color = new (0, 1, 0, 1)
                },
                new ()
                {
                    Position = new(-0.5f, -0.5f, -0.5f, 0),
                    Color = new (0, 1, 0, 1)
                },
                new ()
                {
                    Position = new(-0.5f, -0.5f, 0.5f, 0),
                    Color = new (0, 1, 0, 1)
                },
                new ()
                {
                    Position = new(-0.5f, 0.5f, -0.5f, 0),
                    Color = new (0, 1, 0, 1)
                }
            // 4
                ,
                new ()
                {
                    Position = new(0.5f, 0.5f, 0.5f, 0),
                    Color = new (0, 0, 1, 1)
                },
                new ()
                {
                    Position = new(-0.5f, -0.5f, 0.5f, 0),
                    Color = new (0, 0, 1, 1)
                },
                new ()
                {
                    Position = new(0.5f, -0.5f, 0.5f, 0),
                    Color = new (0, 0, 1, 1)
                },
                new ()
                {
                    Position = new(-0.5f, 0.5f, 0.5f, 0),
                    Color = new (0, 0, 1, 1)
                }
            // 5
                ,
                new ()
                {
                    Position = new(-0.5f, 0.5f, -0.5f, 0),
                    Color = new (1, 0, 1, 1)
                },
                new ()
                {
                    Position = new(0.5f, 0.5f, 0.5f, 0),
                    Color = new (1, 0, 1, 1)
                },
                new ()
                {
                    Position = new(0.5f, 0.5f, -0.5f, 0),
                    Color = new (1, 0, 1, 1)
                },
                new ()
                {
                    Position = new(-0.5f, 0.5f, 0.5f, 0),
                    Color = new (1, 0, 1, 1)
                }
            // 6
                ,
                new ()
                {
                    Position = new(0.5f, -0.5f, 0.5f, 0),
                    Color = new (0, 1, 1, 1)
                },
                new ()
                {
                    Position = new(-0.5f, -0.5f, -0.5f, 0),
                    Color = new (0, 1, 1, 1)
                },
                new ()
                {
                    Position = new(0.5f, -0.5f, -0.5f, 0),
                    Color = new (0, 1, 1, 1)
                }
                ,
                new ()
                {
                    Position = new(-0.5f, -0.5f, 0.5f, 0),
                    Color = new (0, 1, 1, 1)
                }
            ];

        mesh.Indices = 
            [
                new (0),
                new (1),
                new (2),
                new (0),
                new (3),
                new (1),

                new (4),
                new (5),
                new (6),
                new (4),
                new (7),
                new (5),

                new (8),
                new (9),
                new (10),
                new (8),
                new (11),
                new (9),

                new (12),
                new (13),
                new (14),
                new (12),
                new (15),
                new (13),

                new (16),
                new (17),
                new (18),
                new (16),
                new (19),
                new (17),

                new (20),
                new (21),
                new (22),
                new (20),
                new (23),
                new (21)
            ];

        var vertices = new Vertex[mesh.Indices.Length];
        for(var i = 0; i < vertices.Length; i++)
        {
            var v = new Vertex();

            v = mesh.Vertices[mesh.Indices[i].Value];

            vertices[i] = v;
        }

        mesh.Vertices = vertices;
        gen.GenerateForTriangles(ref mesh.Vertices);

        return mesh;
    }

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
        //if (Indices.Length < 1)
        //    return [];

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
