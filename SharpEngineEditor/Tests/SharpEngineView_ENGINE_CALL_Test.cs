using SharpEngineCore.Graphics;
using SharpEngineEditor.Misc;
using System.Diagnostics;
using System.Windows.Media;

namespace SharpEngineEditor.Tests
{

    internal sealed class SharpEngineView_ENGINE_CALL_Test :
        IInternalEngineParameterizedTest<SharpEngineView>
    {

        public bool Run()
        {
            Debug.Assert(false,
                $"Use {nameof(Run)} Paramertized to SharpEngineView");

            return false;
        }

        bool IInternalEngineParameterizedTest<SharpEngineView>.Run(SharpEngineView engineView)
        {
            Debug.Assert(engineView != null);

            var result = false;

            try
            {
                engineView.ENGINE_CALL(() =>
                {
                    _ = Mesh.Cube();
                });

                _ = engineView.ENGINE_CALL<Mesh>(Mesh.Cube);

            }
            catch (Exception e)
            {
                Debug.Assert(result,
                    $"{e}");
                return result;
            }

            result = true;
            return result;
        }
    }
}
