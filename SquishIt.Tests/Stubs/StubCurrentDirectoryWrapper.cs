using SquishIt.Framework.Files;

namespace SquishIt.Tests.Stubs
{
    public class StubCurrentDirectoryWrapper: ICurrentDirectoryWrapper
    {
        public T UsingCurrentDirectory<T>(string directory, System.Func<T> innerFunction)
        {
            return innerFunction();
        }
    }
}