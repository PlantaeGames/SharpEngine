using SharpEngineCore.Attributes;
using System.Diagnostics;

namespace SharpEngineGame
{
    internal static class GameAssembly
    {
        [GameAssemblyStartAttribute]
        internal static void Start()
        {
        }

        [GameAssemblyStopAttribute]
        internal static void Stop()
        {

        }
    }
}
