using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using Bundler.Framework.CssCompressors;
using Bundler.Framework.FileResolvers;
using Bundler.Framework.Files;
using Bundler.Framework.Utilities;

namespace Bundler.Framework
{
    internal class CssBundle : BundleBase, ICssBundler
    {
        private static Dictionary<string, string> renderedCssFiles = new Dictionary<string, string>();
        private List<string> cssFiles = new List<string>();

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

        public static string ProcessCssInput(List<InputFile> arguments, string outputFile, string gzippedOutputFile, string compressorType)
        {
            List<string> files = GetFiles(arguments);
            string compressedCss = CompressCss(files, compressorType);
            WriteFiles(compressedCss, outputFile);
            WriteGZippedFile(compressedCss, null);
            return compressedCss;
        }

        public static string CompressCss(List<string> files, string compressorType)
        {
            ICssCompressor compressor = CssCompressorRegistry.Get(compressorType);
            return CompressCss(files, compressor).ToString();
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
    }
}