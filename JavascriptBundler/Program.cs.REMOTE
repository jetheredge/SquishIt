using System;
using System.Collections.Generic;
using System.Reflection;
using Bundler.Framework.FileResolvers;
using Bundler.Framework.Files;

namespace JavascriptBundler
{
    class Program
    {
        static void Main(string[] args)
        {                        
            bool showHelp = false;
            bool header = true;
            var fileArguments = new List<InputFile>();
            string outputFile = null;
            string gzippedOutputFile = null;
            string minifierType = null;

            var optionSet = new OptionSet()
                                {
                                    { "noheader", "Doesn't show application info and version", v => header = v == null},
                                    { "h|?|help", "Shows help", v => showHelp = v != null},
                                    { "file=", "File to include", v => fileArguments.Add(new InputFile(v, FileResolver.Type)) },
                                    { "dir=", "Directory to include", v => fileArguments.Add(new InputFile(v, DirectoryResolver.Type)) },
                                    { "http=", "Http file to include", v => fileArguments.Add(new InputFile(v, HttpResolver.Type)) },
                                    { "out=", "Output file", v => outputFile = v },
                                    { "outgz=", "Output file", v => gzippedOutputFile = v },
                                    { "min=", "Optional minifier to use (jsmin, closure, yui)", v => minifierType = v.ToLower() }
                                };

            optionSet.Parse(args);

            if (header)
            {
                Assembly appAssembly = Assembly.GetEntryAssembly();
                Console.WriteLine("JavaScript Bundler Version: " + appAssembly.GetName().Version);
                Console.WriteLine("Copyright 2010 Justin Etheredge (http://www.CodeThinked.com)");
            }

            if (showHelp)
            {
                ShowHelp(optionSet);
                return;
            }

            Bundler.Framework.Bundle.ProcessInput(fileArguments, outputFile, gzippedOutputFile, minifierType);
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