using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Web;
using Bundler.Framework.FileResolvers;
using Bundler.Framework.Files;
using Bundler.Framework.Minifiers;
using Bundler.Framework.Utilities;

namespace Bundler.Framework
{
    internal class JavaScriptBundle: BundleBase, IJavaScriptBundler
    {
        private static Dictionary<string, string> renderedJavaScriptFiles = new Dictionary<string, string>();
        private List<string> javaScriptFiles = new List<string>();
        
        public IJavaScriptBundler AddJs(string javaScriptPath)
        {
            javaScriptFiles.Add(javaScriptPath);
            return this;
        }

        public string RenderJs(string renderTo)
        {
            string scriptTemplate = "<script type=\"text/javascript\" src=\"{0}\"></script>";
            if (HttpContext.Current != null && HttpContext.Current.IsDebuggingEnabled)
            {
                return RenderFiles(scriptTemplate, javaScriptFiles);
            }

            if (!renderedJavaScriptFiles.ContainsKey(renderTo))
            {
                lock (renderedJavaScriptFiles)
                {
                    if (!renderedJavaScriptFiles.ContainsKey(renderTo))
                    {
                        string outputFile = ResolveFile(renderTo);
                        string minifiedJavaScript = ProcessJavaScriptInput(GetFilePaths(javaScriptFiles), outputFile, null, JsMinMinifier.Identifier);
                        string hash = Hasher.Create(minifiedJavaScript);
                        string renderedScriptTag = String.Format(scriptTemplate, renderTo + "?r=" + hash);
                        renderedJavaScriptFiles.Add(renderTo, renderedScriptTag);
                    }
                }
            }
            return renderedJavaScriptFiles[renderTo];
        }

        public static string ProcessJavaScriptInput(List<InputFile> arguments, string outputFile, string gzippedOutputFile, string minifierType)
        {
            List<string> files = GetFiles(arguments);
            string minifiedJavaScript = MinifyJavaScript(files, minifierType);
            WriteFiles(minifiedJavaScript, outputFile);
            WriteGZippedFile(minifiedJavaScript, null);
            return minifiedJavaScript;
        }

        public static string MinifyJavaScript(List<string> files, string minifierType)
        {
            IJavaScriptCompressor minifier = MinifierRegistry.Get(minifierType);
            return MinifyJavaScript(files, minifier).ToString();
        }

        private static StringBuilder MinifyJavaScript(List<string> files, IJavaScriptCompressor minifier)
        {
            var outputJavaScript = new StringBuilder();
            foreach (string file in files)
            {
                outputJavaScript.Append(minifier.CompressFile(file));
            }
            return outputJavaScript;
        }
    }
}