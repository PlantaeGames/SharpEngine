using SharpEngineEditor.Exceptions;
using System.Diagnostics;
using System.Reflection;

namespace SharpEngineEditor.Utilities;

public sealed class TypeResolver
{
    private readonly Assembly[] _assemblies;
    public readonly Type ReferenceType;

    private List<Type> _types = new();

#nullable enable
    public Type? Resolve(string typeName)
    {
        return _types.Find(x => x.Name == typeName);
    }

    public Type? Resolve(Type type)
    {
        return Resolve(type.Name);
    }
#nullable disable

    public void Refresh()
    {
        var types = new List<Type>();

        foreach (var assembly in _assemblies)
        {
            try
            {
                foreach (var type in assembly.GetTypes())
                {
                    if (type.BaseType == null)
                        continue;

                    if (type.BaseType.Name != ReferenceType.Name)
                        continue;

                    types.Add(type);
                }
            }
            catch (Exception e)
            {
                throw new FailedToResolveTypes(
                    $"Failed to resovle types of {assembly}", e);
            }
        }

        _types = types;
    }

    public TypeResolver(Type referenceType)
    {
        _assemblies = [Assembly.GetExecutingAssembly()];
        ReferenceType = referenceType;

        Refresh();
    }

    public TypeResolver(Assembly[] assemblies, Type referenceType)
    {
        Debug.Assert(assemblies != null);
        Debug.Assert(assemblies.Length > 0);

        _assemblies = assemblies;
        ReferenceType = referenceType;

        Refresh();
    }
}
