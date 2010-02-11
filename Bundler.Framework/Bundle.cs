using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Web;
using Bundler.Framework.FileResolvers;
using Bundler.Framework.Files;
using Bundler.Framework.Minifiers;

namespace Bundler.Framework
{
    public class Bundle
    {
        private static Dictionary<string,string> renderedFiles = new Dictionary<string,string>();
        private List<string> javascriptFiles = new List<string>();
        
        public Bundle AddJs(string javaScriptPath)
        {
            javascriptFiles.Add(javaScriptPath);
            return this;
        }

        public string RenderJs(string renderTo)
        {
            string scriptTemplate = "<script type=\"text/javascript\" src=\"{0}\"></script>";
            if (HttpContext.Current.IsDebuggingEnabled)
            {
                var sb = new StringBuilder();
                foreach (string file in javascriptFiles)
                {
                    sb.Append(String.Format(scriptTemplate, file));    
                }
                return sb.ToString();
            }

            if (!renderedFiles.ContainsKey(renderTo))
            {                
                lock (renderedFiles)
                {
                    if (!renderedFiles.ContainsKey(renderTo))
                    {
                        string path = HttpContext.Current.Server.MapPath(renderTo);
                        ProcessInput(GetFilePaths(javascriptFiles), path, null, "jsmin");
                        string renderedScriptTag = String.Format(scriptTemplate, renderTo + "?r=" + DateTime.Now.ToString("yyyyMMddHHmmss"));
                        renderedFiles.Add(renderTo, renderedScriptTag);
                    }
                }                
            }
            return renderedFiles[renderTo];
        }

        private List<InputFile> GetFilePaths(List<string> list)
        {
            var result = new List<InputFile>();
            foreach (string file in list)
            {
                string mappedPath = HttpContext.Current.Server.MapPath(file);
                result.Add(new InputFile(mappedPath, FileResolver.Type));
            }
            return result;
        }

        public static void ProcessInput(List<InputFile> fileArguments, string outputFile, string gzippedOutputFile, string minifierType)
        {
            var files = new List<string>();
            var fileResolverCollection = new FileResolverCollection();
            foreach (InputFile file in fileArguments)
            {
                files.AddRange(fileResolverCollection.Resolve(file.FilePath, file.FileType));
            }

            IFileCompressor minifier = GetMinifier(minifierType);

            StringBuilder outputJavaScript = MinifyJavaScript(files, minifier);
            WriteFiles(outputJavaScript, files, minifier, outputFile);
            WriteGZippedFile(outputJavaScript, gzippedOutputFile);
        }

        private static StringBuilder MinifyJavaScript(List<string> files, IFileCompressor minifier)
        {
            var outputJavaScript = new StringBuilder();
            foreach (string file in files)
            {
                outputJavaScript.Append(minifier.Compress(file));
            }
            return outputJavaScript;
        }

        private static void WriteFiles(StringBuilder outputJavaScript, List<string> files, IFileCompressor minifier, string outputFile)
        {            
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
        }

        private static void WriteGZippedFile(StringBuilder outputJavaScript, string gzippedOutputFile)
        {
            if (gzippedOutputFile != null)
            {
                var gzipper = new FileGZipper();
                gzipper.Zip(gzippedOutputFile, outputJavaScript.ToString());
            }
        }

        private static IFileCompressor GetMinifier(string minifierType)
        {
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
            return minifier;
        }
    }
}