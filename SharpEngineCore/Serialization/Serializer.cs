using SharpEngineCore.ECS;
using System.Diagnostics;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SharpEngineCore.Serialization;

public sealed class Serializer
{
    public Serializer()
    {}

    public string SerializeJson<T>(T @object)
    {
        var json = JsonSerializer.Serialize(@object, new JsonSerializerOptions()
        {
            IncludeFields = true
        });

        return json;
    }

    public T DeSerialize<T>(string json)
    {
        var @object = JsonSerializer.Deserialize<T>(json);
        return @object;
    }
}
