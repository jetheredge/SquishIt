using System.Collections.Generic;
using SquishIt.Framework.Resolvers;
using System.Linq;

namespace SquishIt.Tests.Stubs
{
    public class StubResolver : IResolver
    {
        private string _pathToResolveTo;
        private string[] _directoryContents;

        private StubResolver(string pathToResolveTo, string [] directoryContents = null)
        {
            _pathToResolveTo = pathToResolveTo;
            _directoryContents = directoryContents;
        }

        public string TryResolve(string file)
        {
            return _pathToResolveTo;
        }

        public IEnumerable<string> TryResolveFolder(string path, IEnumerable<string> allowedExtensions) {
            return _directoryContents
                .Where(dc => allowedExtensions.Any(ext => dc.EndsWith(ext, System.StringComparison.InvariantCultureIgnoreCase)))
                .ToArray();
        }

        public virtual bool IsDirectory(string path) {
            return _directoryContents != null;
        }
    
        public static IResolver ForDirectory(string[] files)
        {
            return new StubResolver(null, files);   
        }
        
        public static IResolver ForFile(string file)
        {
            return new StubResolver(file, null);   
        }
    }
}