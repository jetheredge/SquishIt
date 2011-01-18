using System.Collections.Generic;
using SquishIt.Framework.FileResolvers;

namespace SquishIt.Tests.Stubs
{
    public class StubFileResolver : IFileResolver
    {
        private string _pathToResolveTo;
        public StubFileResolver(string pathToResolveTo)
        {
            _pathToResolveTo = pathToResolveTo;
        }

        public IEnumerable<string> TryResolve(string file)
        {
            yield return _pathToResolveTo;
        }
    }
}