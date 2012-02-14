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

        public IEnumerable<string> TryResolveFolder(string path, IEnumerable<string> allowedFileExtensions, IEnumerable<string> disallowedFileExtensions) 
        {
            return _directoryContents
                .Where(
                        f => (allowedFileExtensions == null
                            || allowedFileExtensions.Select(s => s.ToUpper()).Any(x => Extensions(f).Contains(x))
                            &&
                            (disallowedFileExtensions == null
                            || !disallowedFileExtensions.Select(s => s.ToUpper()).Any(x => Extensions(f).Contains(x)))
                            ))
                .ToArray();
        }

        public virtual bool IsDirectory(string path) 
        {
            return _directoryContents != null;
        }

        static IEnumerable<string> Extensions(string path) 
        {
            return path.Split('.')
                .Skip(1)
                .Select(s => "." + s.ToUpper());
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