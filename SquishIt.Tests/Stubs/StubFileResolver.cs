using System.Collections.Generic;
using SquishIt.Framework.Resolvers;
using System.Linq;

namespace SquishIt.Tests.Stubs
{
    public class StubFileSystemResolver : IResolver
    {
        private string _pathToResolveTo;
        private string[] _directoryContents;

        public StubFileSystemResolver(string pathToResolveTo, string [] directoryContents = null)
        {
            _pathToResolveTo = pathToResolveTo;
            _directoryContents = directoryContents;
        }

        public string TryResolve(string file)
        {
            return _pathToResolveTo;
        }

        public IEnumerable<string> TryResolveFolder(string path, string[] allowedExtensions) {
            return _directoryContents
                .Where(dc => allowedExtensions.Any(ext => dc.EndsWith(ext, System.StringComparison.InvariantCultureIgnoreCase)))
                .ToArray();
        }

        public virtual bool IsDirectory(string path) {
            return _directoryContents != null;
        }
    }
}