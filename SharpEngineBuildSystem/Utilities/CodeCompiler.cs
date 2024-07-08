using Microsoft.CSharp;
using System.CodeDom.Compiler;
using Microsoft.Build.Construction;

namespace SharpEngineBuildSystem.Utilities;

internal sealed class CodeCompiler : IDisposable
{
    private CSharpCodeProvider _codeProvder;
    private SolutionFile _solution;

    private bool _disposed;

    public CodeCompiler()
    { 
        _codeProvder = new CSharpCodeProvider();
    }

    public void Compile()
    {
    }

    public void Dispose()
    {
        if (_disposed)
            return;

        _codeProvder.Dispose();

        _disposed = true;
    }
}
