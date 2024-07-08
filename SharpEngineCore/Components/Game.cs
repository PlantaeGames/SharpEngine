using System.Diagnostics;
using System.Reflection;
using TerraFX.Interop.Windows;

public sealed class Game
{
	private Assembly _assembly;

	public void StartExecution()
	{
		Debug.Assert(_assembly != null);


	}

	public void StopExecution()
	{
		Debug.Assert(_assembly != null);

		
	}

	public Game(string gameAssembly)
	{
		Debug.Assert(gameAssembly != null);
		Debug.Assert(gameAssembly != string.Empty);

		string path = string.Empty;
    
        try
		{
            path = Path.Combine(Environment.CurrentDirectory, gameAssembly);
            var bytes = File.ReadAllBytes(path);
			
            _assembly = Assembly.Load(bytes);
		}
		catch(FileLoadException e)
		{
			throw new FailedToLoadGameAssemblyException(
				$"Failed to load game assembly.\n\n" +
				$"Path: {path}", e);
		}
	}
}
