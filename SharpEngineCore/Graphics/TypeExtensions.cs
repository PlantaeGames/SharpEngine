using System.Diagnostics;

namespace SharpEngineCore.Graphics;

internal static class TypeExtensions
{
    public static bool Match<TOther>(this Type type, TOther other)
    {
        return type.Name == other.GetType().Name;
    }

    /// <summary>
    /// Gets the size in bytes of type.
    /// </summary>
    /// <remarks>Only for IFragmentables and IUnitables types</remarks>
    public static int GetDataTypeSize(this Type type)
    {
        Debug.Assert(type.GetInterface(nameof(IFragmentable)) != null ||
                     type.GetInterface(nameof(IUnitable)) != null,
             "Type must be inherited from IUnitable or IFragmentable.");

        var obj = Activator.CreateInstance(type);

        if (type.GetInterface(nameof(IUnitable)) != null)
        {

            return  (int)type.InvokeMember(nameof(IUnitable.GetUnitCount),
                System.Reflection.BindingFlags.Public |
                System.Reflection.BindingFlags.InvokeMethod |
                System.Reflection.BindingFlags.Instance, Type.DefaultBinder,
                obj, null) * Unit.GetSize();
        }

        return (int)type.InvokeMember(nameof(IFragmentable.GetFragmentsCount),
            System.Reflection.BindingFlags.Public |
            System.Reflection.BindingFlags.InvokeMethod |
            System.Reflection.BindingFlags.Instance, Type.DefaultBinder,
            obj, null) * Fragment.GetSize();
    }
}
