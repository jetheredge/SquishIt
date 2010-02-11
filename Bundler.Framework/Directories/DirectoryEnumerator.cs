using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Bundler.Framework.Directories;

namespace Bundler.Framework.Directories
{
    public class DirectoryEnumerator: IDirectoryEnumerator
    {
        private IDirectory directory;

        public DirectoryEnumerator()
        {
        }

        public DirectoryEnumerator(IDirectory directory)
        {
            this.directory = directory;
        }

        public IEnumerable<string> GetFiles(string path)
        {            
            var files = directory.GetFiles(path, "*.js").ToArray();
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

        private IEnumerable<string> OrderFiles(List<string> ordering, string[] files)
        {
            var result = new List<string>();
            foreach (string order in ordering)
            {
                foreach (string file in files)
                {
                    if (Path.GetFileName(file).ToLower() == order)
                    {
                        result.Add(file);
                    }
                }
            }
            return result;
        }

        private List<string> GetOrdering(string path)
        {
            var ordering = new List<string>();
            string orderingFile = path + "ordering.txt";
            if (File.Exists(path + "ordering.txt"))
            {
                using (var sr = new StreamReader(orderingFile))
                {
                    ordering.Add(sr.ReadLine().ToLower());
                }
            }
            return ordering;
        }
    }
}