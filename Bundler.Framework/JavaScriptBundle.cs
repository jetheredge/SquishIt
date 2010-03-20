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
    internal class JavaScriptBundle: BundleBase, IJavaScriptBundle, IJavaScriptBundleBuilder
    {
        private readonly IDebugStatusReader debugStatusReader;
        private static Dictionary<string, string> renderedJavaScriptFiles = new Dictionary<string, string>();
        private List<string> javaScriptFiles = new List<string>();

        public JavaScriptBundle()
        {
            debugStatusReader = new DebugStatusReader();
        }

        public JavaScriptBundle(IDebugStatusReader debugStatusReader)
        {
            this.debugStatusReader = debugStatusReader;
        }

        IJavaScriptBundleBuilder IJavaScriptBundle.Add(string javaScriptPath)
        {
            javaScriptFiles.Add(javaScriptPath);
            return this;
        }

        IJavaScriptBundleBuilder IJavaScriptBundleBuilder.Add(string javaScriptPath)
        {
            javaScriptFiles.Add(javaScriptPath);
            return this;
        }

        public void AsNamed(string name, string renderTo)
        {
            Render(renderTo, name);
        }

        string IJavaScriptBundle.RenderNamed(string name)
        {
            return renderedJavaScriptFiles[name];
        }

        string IJavaScriptBundleBuilder.Render(string renderTo)
        {
            return Render(renderTo, renderTo);
        }

        private string Render(string renderTo, string key)
        {
            string scriptTemplate = "<script type=\"text/javascript\" src=\"{0}\"></script>";
            if (debugStatusReader.IsDebuggingEnabled())
            {                
                return RenderFiles(scriptTemplate, javaScriptFiles);
            }

            if (!renderedJavaScriptFiles.ContainsKey(key))
            {
                lock (renderedJavaScriptFiles)
                {
                    if (!renderedJavaScriptFiles.ContainsKey(key))
                    {
                        string outputFile = ResolveAppRelativePathToFileSystem(renderTo);
                        string minifiedJavaScript = ProcessJavaScriptInput(GetFilePaths(javaScriptFiles), outputFile, null, JsMinMinifier.Identifier);
                        string hash = Hasher.Create(minifiedJavaScript);
                        string renderedScriptTag = String.Format(scriptTemplate, ExpandAppRelativePath(renderTo) + "?r=" + hash);
                        renderedJavaScriptFiles.Add(key, renderedScriptTag);
                    }
                }
            }
            return renderedJavaScriptFiles[key];
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