using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Web;
using Bundler.Framework.FileResolvers;
using Bundler.Framework.Files;

namespace Bundler.Framework
{
    public class BundleBase
    {
        protected string RenderFiles(string scriptTemplate, IEnumerable<string> files)
        {
            var sb = new StringBuilder();
            foreach (string file in files)
            {
                sb.Append(String.Format(scriptTemplate, file));
            }
            return sb.ToString();
        }

        protected static List<string> GetFiles(List<InputFile> fileArguments)
        {
            var files = new List<string>();
            var fileResolverCollection = new FileResolverCollection();
            foreach (InputFile file in fileArguments)
            {
                files.AddRange(fileResolverCollection.Resolve(file.FilePath, file.FileType));
            }
            return files;
        }

        protected static void WriteFiles(string outputJavaScript, string outputFile)
        {
            if (outputFile != null)
            {
                using (var sr = new StreamWriter(outputFile, false))
                {
                    sr.Write(outputJavaScript);
                }
            }
            else
            {
                Console.WriteLine(outputJavaScript);
            }
        }

        protected static void WriteGZippedFile(string outputJavaScript, string gzippedOutputFile)
        {
            if (gzippedOutputFile != null)
            {
                var gzipper = new FileGZipper();
                gzipper.Zip(gzippedOutputFile, outputJavaScript);
            }
        }

        protected static List<InputFile> GetFilePaths(List<string> list)
        {
            var result = new List<InputFile>();
            foreach (string file in list)
            {
                string mappedPath = HttpContext.Current.Server.MapPath(file);
                result.Add(new InputFile(mappedPath, FileResolver.Type));
            }
            return result;
        }
    }
}