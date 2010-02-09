using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using JavascriptBundler.FileResolvers;
using JavascriptBundler.Files;
using JavascriptBundler.Minifiers;
using JavascriptCombiner;

namespace JavascriptBundler
{
    class Program
    {
        static void Main(string[] args)
        {            
            bool showHelp = false;            
            var fileArguments = new List<InputFile>();
            string outputFile = null;
            string gzippedOutputFile = null;
            string minifierType = null;

            var optionSet = new OptionSet()
            {
                { "h|?|help", "Shows help", v => showHelp = v != null},
                { "file=", "File to include", v => fileArguments.Add(new InputFile(v, FileResolver.Type)) },
                { "dir=", "Directory to include", v => fileArguments.Add(new InputFile(v, DirectoryResolver.Type)) },
                { "http=", "Http file to include", v => fileArguments.Add(new InputFile(v, HttpResolver.Type)) },
                { "out=", "Output file", v => outputFile = v },
                { "outgz=", "Output file", v => gzippedOutputFile = v },
                { "min=", "Optional minifier to use (jsmin, closure, yui)", v => minifierType = v.ToLower() }
            };

            optionSet.Parse(args);

            if (showHelp)
            {
                ShowHelp(optionSet);
                return;
            }

            ProcessInput(fileArguments, outputFile, gzippedOutputFile, minifierType);
        }

        private static void ProcessInput(List<InputFile> fileArguments, string outputFile, string gzippedOutputFile, string minifierType)
        {
            var files = new List<string>();
            var fileResolverCollection = new FileResolverCollection();
            foreach (InputFile file in fileArguments)
            {
                files.AddRange(fileResolverCollection.Resolve(file.FilePath, file.FileType));
            }

            IFileCompressor minifier = null;
            if (minifierType == "jsmin")
            {
                minifier = new JsMinMinifier();
            }
            else if (minifierType == "closure")
            {
                minifier = new ClosureMinifier();
            }
            else if (minifierType == "yui")
            {                
            }
            else
            {
                minifier = new NullMinifier();
            }

            var outputJavaScript = new StringBuilder();                        
            foreach (string file in files)
            {                
                outputJavaScript.Append(minifier.Compress(file));
            }

            if (outputFile != null)
            {                
                using (var sr = new StreamWriter(outputFile, false))
                {
                    sr.Write(outputJavaScript.ToString());
                }
            }
            else
            {
                Console.WriteLine(outputJavaScript);
            }

            if (gzippedOutputFile != null)
            {
                var gzipper = new FileGZipper();
                gzipper.Zip(gzippedOutputFile, outputJavaScript.ToString());                
            }
        }

        static void ShowHelp(OptionSet p)
        {
            Console.WriteLine("Usage: javascript_bundler [Options]");
            Console.WriteLine("Combines multiple javascript files into a single file.");
            Console.WriteLine("Optionally minifies and compresses.");
            Console.WriteLine();
            Console.WriteLine("Options:");
            p.WriteOptionDescriptions(Console.Out);
        }
    }
}
