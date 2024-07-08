using SharpEngineCore.Attributes;
using System.Diagnostics;

namespace SharpEngineGame
{
    internal sealed class GameAssembly
    {
        public static void Main()
        {
            try
            {
                Process.Start("SharpEngineCore.exe");
            }
            catch
            {
                throw;
            }
        }

        [GameAssemblyStart]
        internal static void Start()
        {
        }

        [GameAssemblyStop]
        internal static void Stop()
        {

        }
    }
}
