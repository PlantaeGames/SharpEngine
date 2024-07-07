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

	public Game(string gameAssenbmlyPath)
	{
		try
		{
			_assembly = Assembly.Load(gameAssenbmlyPath);
		}
		catch(Exception e)
		{
			throw new FailedToLoadGameAssemblyException(
				$"Failed to load game assembly.", e);
		}
	}
}
