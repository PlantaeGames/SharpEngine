using System.Diagnostics;
using System.Text;

namespace SharpEngineCore.Utilities;

internal static class StringExtensions
{
    public static string ToSingleString(this string[] strings)
    {
        Debug.Assert(strings.Length > 0, "Array length can't be zero or less than zero.");

        var sb = new StringBuilder(strings.Length * strings[0].Length);
        foreach(var str in strings)
        {
            sb.Append(str);
        }

        string result = sb.ToString();
        return result;
    }

    public static string ToSingleString(this char[] chars)
    {
        Debug.Assert(chars.Length > 0, "Array length can't be zero or less than zero.");

        var sb = new StringBuilder(chars.Length);
        foreach(var c in chars)
        {
            sb.Append(c);
        }

        var result = sb.ToString();
        return result;
    }
}
