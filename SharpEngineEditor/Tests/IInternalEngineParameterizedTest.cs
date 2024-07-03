namespace SharpEngineEditor.Tests
{
    internal interface IInternalEngineParameterizedTest<T> : IInternalEngineTest
    {
        bool Run(T param);
    }
}
