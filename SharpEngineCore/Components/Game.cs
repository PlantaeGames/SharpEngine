using SharpEngineCore.Attributes;
using SharpEngineCore.Exceptions;
using System.Diagnostics;
using System.Reflection;
using System.Reflection.Metadata;
using TerraFX.Interop.Windows;

public sealed class GameAssembly
{
	private Assembly _assembly;

	private MethodInfo _startMethod;
	private MethodInfo _stopMethod;

	public Assembly Assembly => _assembly;

	public void StartExecution()
	{
		Debug.Assert(_assembly != null);

		_startMethod.Invoke(null, null);
	}

	public void StopExecution()
	{
		Debug.Assert(_assembly != null);

		_stopMethod.Invoke(null, null);
	}

	public GameAssembly(string gameAssembly)
	{
		Debug.Assert(gameAssembly != null);
		Debug.Assert(gameAssembly != string.Empty);

		string path = string.Empty;
    
        try
		{
            path = Path.Combine(Environment.CurrentDirectory, gameAssembly);
            var bytes = File.ReadAllBytes(path);
			
            _assembly = Assembly.Load(bytes);

            foreach (var type in _assembly.GetTypes())
            {
                foreach (var method in type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static))
                {
                    var start = method.GetCustomAttribute<GameAssemblyStartAttribute>();
                    var end = method.GetCustomAttribute<GameAssemblyStopAttribute>();

                    if (start != null)
                    {
						Debug.Assert(method.GetParameters().Length == 0);

                        _startMethod = method;
                    }

                    if (end != null)
                    {
                        Debug.Assert(method.GetParameters().Length == 0);

                        _stopMethod = method;
                    }
                }
            }
        }
		catch(Exception e)
		{
			throw new FailedToLoadGameAssemblyException(
				$"Failed to load game assembly.\n\n" +
				$"Path: {path}", e);
		}
	}
}
