using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SquishIt.Framework.Directories
{
    public class DirectoryEnumerator: IDirectoryEnumerator
    {
        private readonly IDirectory directory;

        public DirectoryEnumerator()
        {
            directory = new Directory();
        }

        public DirectoryEnumerator(IDirectory directory)
        {
            this.directory = directory;
        }

        public IEnumerable<string> GetFiles(string path)
        {
            var files = directory.GetFiles(path, "*.js")
                .Where(file => !file.EndsWith("-vsdoc.js", StringComparison.OrdinalIgnoreCase))
                .ToArray();

            var ordering = GetOrdering(path);
            if (ordering.Count <= 0)
            {
                return files;
            }

            if (ordering.Count != files.Length)
            {
                Console.Error.WriteLine("Number of entries in 'ordering.txt' does not match number of javascript files in folder.");
            }

            return OrderFiles(ordering, files);
        }

        private static IEnumerable<string> OrderFiles(IEnumerable<string> ordering,
            IEnumerable<string> files)
        {
            var orderedFiles =
                from order in ordering
                from file in files
                let fileName = Path.GetFileName(file)
                where !String.IsNullOrEmpty(fileName)
                    && fileName.Equals(order, StringComparison.OrdinalIgnoreCase)
                select file;

            return orderedFiles;
        }

        private static List<string> GetOrdering(string path)
        {
            var orderingFile = Path.Combine(path, "ordering.txt");
            if (!File.Exists(orderingFile))
            {
                return new List<string>();
            }

            return ReadOrderingFile(orderingFile);
        }

        private static List<string> ReadOrderingFile(string orderingFile)
        {
            var ordering = new List<string>();
            using (var sr = new StreamReader(orderingFile))
            {
                for (string entry = sr.ReadLine(); entry != null; entry = sr.ReadLine())
                {
                    entry = entry.Trim();
                    if (!String.IsNullOrEmpty(entry))
                    {
                        ordering.Add(entry);
                    }
                }
            }
            return ordering;
        }
    }
}