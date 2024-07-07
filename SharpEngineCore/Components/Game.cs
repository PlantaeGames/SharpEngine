using System.Diagnostics;
using System.Reflection;

public sealed class Game
{
	private Assembly _assembly;

	public void Load()
	{
		Debug.Assert(_assembly != null);
	}

	public Game(string gameAssenbmlyPath)
	{

	}

	public Game(Assembly gameAssenbly)
	{

	}
}
