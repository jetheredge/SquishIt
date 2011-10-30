using System.Collections.Generic;
using SquishIt.Framework.Resolvers;

namespace SquishIt.Tests.Stubs
{
    public class StubFileSystemResolver : IResolver
    {
        private string _pathToResolveTo;
        public StubFileSystemResolver(string pathToResolveTo)
        {
            _pathToResolveTo = pathToResolveTo;
        }

        public IEnumerable<string> TryResolve(string file)
        {
            yield return _pathToResolveTo;
        }
    }
}