using SharpEngineCore.Exceptions;

public sealed class FailedToLoadGameAssemblyException : 
	SharpException
{
	public FailedToLoadGameAssemblyException(string message, Exception inner) :
		base(message, inner)
	{ }
}
