using System;
using System.Collections.Generic;
using System.IO;

namespace JavascriptCombiner
{
    class Program
    {
        static void Main(string[] args)
        {
            var directories = new List<string>();
            bool showHelp = false;
            var argumentFiles = new List<string>();
            string outputFile = null;

            var optionSet = new OptionSet()
            {
                { "h|?|help", "Shows help", v => showHelp = v != null },
                { "d=|dir=", "Directories to include", v => directories.Add(v) },
                { "f=|file=", "Files to include", v => argumentFiles.Add(v) },
                { "o|out", "Output file", v => outputFile = v }
            };

            optionSet.Parse(args);

            if (showHelp)
            {
                ShowHelp(optionSet);
            }

            var files = new List<string>();
            GatherFilesFromDirectories(directories, files);
            ResolveFileFullPaths(argumentFiles, files);

            foreach (string file in files)
            {
                Console.WriteLine(file);
            }
        }

        private static void ResolveFileFullPaths(List<string> argumentFiles, List<string> files)
        {
            foreach (string file in argumentFiles)
            {
                var filePath = Path.GetFullPath(file);
                files.Add(filePath);
            }                        
        }

        private static void GatherFilesFromDirectories(List<string> directories, List<string> files)
        {
            foreach (string directory in directories)
            {
                var filesInDirectory = Directory.GetFiles(directory, "*.js");                
                foreach (string file in filesInDirectory)
                {
                    files.Add(file);
                }
            }
        }

        static void ShowHelp(OptionSet p)
        {
            Console.WriteLine("Usage:  [OPTIONS]+ message");
            Console.WriteLine("Greet a list of individuals with an optional message.");
            Console.WriteLine("If no message is specified, a generic greeting is used.");
            Console.WriteLine();
            Console.WriteLine("Options:");
            p.WriteOptionDescriptions(Console.Out);
        }
    }
}
