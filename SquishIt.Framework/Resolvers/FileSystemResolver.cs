using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SquishIt.Framework.Resolvers
{
    public class FileSystemResolver : IResolver
    {
        //public IEnumerable<string> TryResolve(string path)
        //{
        //    if (Directory.Exists(path))
        //    {
        //        var files = Directory.GetFiles(path);
        //        Array.Sort(files);
        //        return files;
        //    }

        //    return new[] { Path.GetFullPath(path) };
        //}

        public IEnumerable<string> TryResolve(string path, string[] allowedFileExtensions)
        {
            if (Directory.Exists(path))
            {
                var files = Directory.GetFiles(path)
                    .Where(
                        f => allowedFileExtensions == null || allowedFileExtensions.Any(x => f.EndsWith(x, StringComparison.InvariantCultureIgnoreCase)))
                    .ToArray();
                Array.Sort(files);
                return files;
            }
            return new[] { Path.GetFullPath(path) };
        }
    }
}