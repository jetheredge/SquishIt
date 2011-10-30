using System;
using System.Collections.Generic;
using System.IO;

namespace SquishIt.Framework.Resolvers
{
    public class FileSystemResolver: IResolver
    {
        public IEnumerable<string> TryResolve(string path)
        {
            if (Directory.Exists(path))
            {
                var files = Directory.GetFiles(path);
                Array.Sort(files);
                return files;
            }

            return new[] { Path.GetFullPath(path) };
        }
    }
}