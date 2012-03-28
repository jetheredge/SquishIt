using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SquishIt.Framework.Resolvers
{
    public class FileSystemResolver : IResolver
    {
        public string TryResolve(string path) 
        {
            return Path.GetFullPath(path);
        }

        public bool IsDirectory(string path) 
        {
            return Directory.Exists(path);
        }

        public IEnumerable<string> TryResolveFolder(string path, bool recursive, IEnumerable<string> allowedFileExtensions, IEnumerable<string> disallowedFileExtensions)
        {
            if (IsDirectory(path)) 
            {
              var files = Directory.GetFiles(path, "*.*", recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly)
                    .Where(
                        f => (allowedFileExtensions == null
                            || allowedFileExtensions.Select(s => s.ToUpper()).Any(x => Extensions(f).Contains(x))
                            &&
                            (disallowedFileExtensions == null
                            || !disallowedFileExtensions.Select(s => s.ToUpper()).Any(x => Extensions(f).Contains(x)))
                            ))
                    .ToArray();
                Array.Sort(files);
                return files;
            }
            return new[] { Path.GetFullPath(path) };
        }

        static IEnumerable<string> Extensions(string path)
        {
            return path.Split('.')
                .Skip(1)
                .Select(s => "." + s.ToUpper());
        }
    }
}