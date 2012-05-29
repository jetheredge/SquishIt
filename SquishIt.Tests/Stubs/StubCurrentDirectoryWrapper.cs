using SquishIt.Framework.Files;

namespace SquishIt.Tests.Stubs
{
    public class StubCurrentDirectoryWrapper: ICurrentDirectoryWrapper
    {
        public void SetCurrentDirectory(string directory)
        {
        }

        public void Revert()
        {
        }
    }
}