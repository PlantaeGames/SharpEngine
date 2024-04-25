using TerraFX.Interop.Windows;

namespace SharpEngineCore.Exceptions;

internal sealed class Errors
{
    public static bool CheckHResult(HRESULT result) => result == S.S_OK;
}