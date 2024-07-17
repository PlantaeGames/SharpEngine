using SharpEngineEditorControls.Exceptions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Windows.Documents;

namespace SharpEngineEditorControls.Extensions;

internal static class AssemblyExtensions
{
    public static (Type type, T attribute)[] GetTypesOfCustomAttribute<T>(this Assembly assembly)
        where T : Attribute
    {
        Debug.Assert(assembly != null);

        var types = new List<(Type type, T attribute)>();
        try
        {
            foreach (var type in assembly.DefinedTypes)
            {
                var attribute = type.GetCustomAttribute<T>();
                if (attribute == null)
                    continue;

                types.Add((type, attribute));
            }
        }
        catch(Exception e)
        {
            Debug.Assert(false, $"{e}");
        }

        return [.. types];
    }
}
