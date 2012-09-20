using SquishIt.Framework.Files;

namespace SquishIt.Tests.Stubs
{
    public class StubDirectoryWrapper: IDirectoryWrapper
    {
        public T ExecuteInDirectory<T>(string directory, System.Func<T> innerFunction)
        {
            return innerFunction();
        }
    }
}