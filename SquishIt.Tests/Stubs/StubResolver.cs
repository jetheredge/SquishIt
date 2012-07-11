using System.Collections.Generic;
using SquishIt.Framework.Resolvers;
using System.Linq;

namespace SquishIt.Tests.Stubs
{
    public class StubResolver : IResolver
    {
        string _pathToResolveTo;
        string[] _directoryContents;

        StubResolver(string pathToResolveTo, string[] directoryContents = null)
        {
            _pathToResolveTo = pathToResolveTo;
            _directoryContents = directoryContents;
        }

        public string Resolve(string file)
        {
            return _pathToResolveTo;
        }

        public IEnumerable<string> ResolveFolder(string path, bool recursive, string debugExtension, IEnumerable<string> allowedFileExtensions, IEnumerable<string> disallowedFileExtensions)
        {
            return _directoryContents
                .Where(
                        f => !f.ToUpperInvariant().EndsWith(debugExtension.ToUpperInvariant())
                            && (allowedFileExtensions == null
                            || allowedFileExtensions.Select(s => s.ToUpperInvariant()).Any(x => Extensions(f).Contains(x))
                            &&
                            (disallowedFileExtensions == null
                            || !disallowedFileExtensions.Select(s => s.ToUpperInvariant()).Any(x => Extensions(f).Contains(x)))
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
                .Select(s => "." + s.ToUpperInvariant());
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