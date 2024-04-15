using System.Diagnostics;

namespace SharpEngineCore.Exceptions;

/// <summary>
/// Extensions for SharpException
/// </summary>
internal static class SharpExceptionExtensions
{
    /// <summary>
    /// Gets all the inner exceptions.
    /// </summary>
    /// <param name="exception">Root exception.</param>
    /// <returns></returns>
    public static IEnumerable<Exception> GetExceptions(this SharpException exception)
    {
        Debug.Assert(exception != null);

        var e = exception as Exception;
        do
        {
            yield return e;
            e = e.InnerException;
        }
        while(e != null);
    }
}
