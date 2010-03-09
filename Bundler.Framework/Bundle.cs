using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Web;
using Bundler.Framework.CssCompressors;
using Bundler.Framework.FileResolvers;
using Bundler.Framework.Files;
using Bundler.Framework.Minifiers;
using Bundler.Framework.Utilities;

namespace Bundler.Framework
{
    public class Bundle: IJavaScriptBundler, ICssBundler
    {
        private static Dictionary<string,string> renderedJavaScriptFiles = new Dictionary<string,string>();
        private static Dictionary<string, string> renderedCssFiles = new Dictionary<string, string>();
        private List<string> javaScriptFiles = new List<string>();
        private List<string> cssFiles = new List<string>();
        
        public IJavaScriptBundler AddJs(string javaScriptPath)
        {
            javaScriptFiles.Add(javaScriptPath);
            return this;
        }

        public string RenderJs(string renderTo)
        {
            string scriptTemplate = "<script type=\"text/javascript\" src=\"{0}\"></script>";
            if (HttpContext.Current.IsDebuggingEnabled)
            {
                return RenderFiles(scriptTemplate, javaScriptFiles);
            }

            if (!renderedJavaScriptFiles.ContainsKey(renderTo))
            {                
                lock (renderedJavaScriptFiles)
                {
                    if (!renderedJavaScriptFiles.ContainsKey(renderTo))
                    {
                        string outputFile = HttpContext.Current.Server.MapPath(renderTo);
                        string minifiedJavaScript = ProcessJavaScriptInput(GetFilePaths(javaScriptFiles), outputFile, null, JsMinMinifier.Identifier);
                        string hash = Hasher.Create(minifiedJavaScript);
                        string renderedScriptTag = String.Format(scriptTemplate, renderTo + "?r=" + hash);
                        renderedJavaScriptFiles.Add(renderTo, renderedScriptTag);
                    }
                }
            }
            return renderedJavaScriptFiles[renderTo];
        }

        public ICssBundler AddCss(string cssScriptPath)
        {
            cssFiles.Add(cssScriptPath);
            return this;
        }

        public string RenderCss(string renderTo)
        {
            string cssTemplate = "<link rel=\"stylesheet\" type=\"text/css\" href=\"{0}\" />";
            if (HttpContext.Current.IsDebuggingEnabled)
            {
                return RenderFiles(cssTemplate, cssFiles);
            }

            if (!renderedCssFiles.ContainsKey(renderTo))
            {
                lock (renderedCssFiles)
                {
                    if (!renderedCssFiles.ContainsKey(renderTo))
                    {
                        string outputFile = HttpContext.Current.Server.MapPath(renderTo);
                        string compressedCss = ProcessCssInput(GetFilePaths(cssFiles), outputFile, null, YuiCompressor.Identifier);
                        string hash = Hasher.Create(compressedCss);
                        string renderedCssTag = String.Format(cssTemplate, renderTo + "?r=" + hash);
                        renderedCssFiles.Add(renderTo, renderedCssTag);
                    }
                }
            }
            return renderedCssFiles[renderTo];
        }        

        public static string ProcessJavaScriptInput(List<InputFile> arguments, string outputFile, string gzippedOutputFile, string minifierType)
        {
            List<string> files = GetFiles(arguments);
            string minifiedJavaScript = MinifyJavaScript(files, minifierType);
            WriteFiles(minifiedJavaScript, outputFile);
            WriteGZippedFile(minifiedJavaScript, null);
            return minifiedJavaScript;
        }

        public static string ProcessCssInput(List<InputFile> arguments, string outputFile, string gzippedOutputFile, string compressorType)
        {
            List<string> files = GetFiles(arguments);
            string compressedCss = CompressCss(files, compressorType);
            WriteFiles(compressedCss, outputFile);
            WriteGZippedFile(compressedCss, null);
            return compressedCss;
        }

        private static List<InputFile> GetFilePaths(List<string> list)
        {
            var result = new List<InputFile>();
            foreach (string file in list)
            {
                string mappedPath = HttpContext.Current.Server.MapPath(file);
                result.Add(new InputFile(mappedPath, FileResolver.Type));
            }
            return result;
        }

        public static string MinifyJavaScript(List<string> files, string minifierType)
        {
            IJavaScriptCompressor minifier = MinifierRegistry.Get(minifierType);
            return MinifyJavaScript(files, minifier).ToString();
        }

        public static string CompressCss(List<string> files, string compressorType)
        {
            ICssCompressor compressor = CssCompressorRegistry.Get(compressorType);
            return CompressCss(files, compressor).ToString();
        }

        private static List<string> GetFiles(List<InputFile> fileArguments)
        {
            var files = new List<string>();
            var fileResolverCollection = new FileResolverCollection();
            foreach (InputFile file in fileArguments)
            {
                files.AddRange(fileResolverCollection.Resolve(file.FilePath, file.FileType));
            }
            return files;
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

        private static StringBuilder CompressCss(List<string> files, ICssCompressor compressor)
        {
            var outputCss = new StringBuilder();
            foreach (string file in files)
            {
                outputCss.Append(compressor.CompressFile(file));
            }
            return outputCss;
        }

        private static void WriteFiles(string outputJavaScript, string outputFile)
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

        private static void WriteGZippedFile(string outputJavaScript, string gzippedOutputFile)
        {
            if (gzippedOutputFile != null)
            {
                var gzipper = new FileGZipper();
                gzipper.Zip(gzippedOutputFile, outputJavaScript);
            }
        }

        private string RenderFiles(string scriptTemplate, IEnumerable<string> files)
        {
            var sb = new StringBuilder();
            foreach (string file in files)
            {
                sb.Append(String.Format(scriptTemplate, file));
            }
            return sb.ToString();
        }
    }
}