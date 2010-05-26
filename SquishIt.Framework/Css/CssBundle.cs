using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using dotless.Core;
using SquishIt.Framework.Css.Compressors;
using SquishIt.Framework.Files;
using SquishIt.Framework.Utilities;

namespace SquishIt.Framework.Css
{
    internal class CssBundle : BundleBase, ICssBundle, ICssBundleBuilder
    {
        private static Dictionary<string, string> renderedCssFiles = new Dictionary<string, string>();
        private static Dictionary<string, string> debugCssFiles = new Dictionary<string, string>();
        private List<string> cssFiles = new List<string>();
        private string mediaTag = "";
        private CssCompressors cssCompressor = CssCompressors.YuiCompressor;
        private bool renderOnlyIfOutputFileMissing = false;
        private const string CssTemplate = "<link rel=\"stylesheet\" type=\"text/css\" {0} href=\"{1}\" />";

        public CssBundle()
            : base(new FileWriterFactory(), new FileReaderFactory(), new DebugStatusReader())
        {
        }

        public CssBundle(IDebugStatusReader debugStatusReader, IFileWriterFactory fileWriterFactory, IFileReaderFactory fileReaderFactory)
            : base(fileWriterFactory, fileReaderFactory, debugStatusReader)
        {
        }

        ICssBundleBuilder ICssBundleBuilder.Add(string cssScriptPath)
        {
            cssFiles.Add(cssScriptPath);
            return this;
        }

        ICssBundleBuilder ICssBundle.Add(string cssScriptPath)
        {
            cssFiles.Add(cssScriptPath);
            return this;
        }

        ICssBundleBuilder ICssBundleBuilder.WithMedia(string media)
        {
            mediaTag = "media=\"" + media + "\"";
            return this;
        }

        public ICssBundleBuilder WithCompressor(CssCompressors cssCompressor)
        {
            this.cssCompressor = cssCompressor;
            return this;
        }

        void ICssBundleBuilder.AsNamed(string name, string renderTo)
        {
            Render(renderTo, name);
        }

        public ICssBundleBuilder RenderOnlyIfOutputFileMissing()
        {
            renderOnlyIfOutputFileMissing = true;
            return this;
        }

        public ICssBundleBuilder ForceDebug()
        {
            debugStatusReader.ForceDebug();
            return this;
        }

        public ICssBundleBuilder ForceRelease()
        {
            debugStatusReader.ForceRelease();
            return this;
        }

        string ICssBundle.RenderNamed(string name)
        {
            if (debugStatusReader.IsDebuggingEnabled())
            {
                return debugCssFiles[name];
            }
            
            return renderedCssFiles[name];
        }

        string ICssBundleBuilder.Render(string renderTo)
        {
            return Render(renderTo, renderTo);
        }

        private string Render(string renderTo, string key)
        {
            if (debugStatusReader.IsDebuggingEnabled())
            {
                string result = RenderDebugCss();
                debugCssFiles[key] = result;
                return result;
            }

            if (!renderedCssFiles.ContainsKey(key))
            {
                lock (renderedCssFiles)
                {
                    if (!renderedCssFiles.ContainsKey(key))
                    {
                        string compressedCss;
                        string hash= null;
                        bool hashInFileName = false;

                        string outputFile = ResolveAppRelativePathToFileSystem(renderTo);

                        if (renderTo.Contains("#"))
                        {
                            hashInFileName = true;
                            compressedCss = CompressCss(outputFile, GetFilePaths(cssFiles), MapCompressorToIdentifier(cssCompressor));
                            hash = Hasher.Create(compressedCss);
                            renderTo = renderTo.Replace("#", hash);
                            outputFile = outputFile.Replace("#", hash);
                        }

                        if (renderOnlyIfOutputFileMissing && FileExists(outputFile))
                        {
                            compressedCss = ReadFile(outputFile);
                        }
                        else
                        {
                            compressedCss = CompressCss(outputFile, GetFilePaths(cssFiles), MapCompressorToIdentifier(cssCompressor));
                            WriteCssToFiles(compressedCss, outputFile, null);
                        }
                        
                        if (hash == null)
                        {
                            hash = Hasher.Create(compressedCss);    
                        }

                        string renderedCssTag;
                        if (hashInFileName)
                        {
                            renderedCssTag = String.Format(CssTemplate, mediaTag, ExpandAppRelativePath(renderTo));
                        }
                        else
                        {
                            string path = ExpandAppRelativePath(renderTo);
                            if (path.Contains("?"))
                            {
                                renderedCssTag = String.Format(CssTemplate, mediaTag, path + "&r=" + hash);
                            }
                            else
                            {
                                renderedCssTag = String.Format(CssTemplate, mediaTag, path + "?r=" + hash);
                            }
                        }
                        renderedCssFiles.Add(key, renderedCssTag);
                    }
                }
            }
            return renderedCssFiles[key];
        }

        private string MapCompressorToIdentifier(CssCompressors compressors)
        {
            switch (compressors)
            {
                case CssCompressors.NullCompressor:
                    return NullCompressor.Identifier;
                case CssCompressors.YuiCompressor:
                    return YuiCompressor.Identifier;
                default:
                    return YuiCompressor.Identifier;
            }
        }

        private string RenderDebugCss()
        {
            string modifiedCssTemplate = String.Format(CssTemplate, mediaTag, "{0}");

            var processedCssFiles = new List<string>();
            foreach (string file in cssFiles)
            {
                if (file.ToLower().EndsWith(".less") || file.ToLower().EndsWith(".less.css"))
                {
                    string outputFile = ResolveAppRelativePathToFileSystem(file);
                    string css = ProcessLess(outputFile);
                    outputFile = outputFile.Substring(0, outputFile.Length - 5);
                    using (var fileWriter = fileWriterFactory.GetFileWriter(outputFile))
                    {
                        fileWriter.Write(css);
                    }
                    processedCssFiles.Add(file.Substring(0, file.Length - 5));
                }
                else
                {
                    processedCssFiles.Add(file);
                }
            }

            return RenderFiles(modifiedCssTemplate, processedCssFiles);
        }

        public string CompressCss(string outputFilePath, List<InputFile> arguments, string compressorType)
        {
            List<string> files = GetFiles(arguments);
            return CompressCss(outputFilePath, files, compressorType);
        }

        public void WriteCssToFiles(string compressedCss, string outputFile, string gzippedOutputFile)
        {
            WriteFiles(compressedCss, outputFile);
            WriteGZippedFile(compressedCss, null);
        }

        public string CompressCss(string outputFilePath, List<string> files, string compressorType)
        {
            ICssCompressor compressor = CssCompressorRegistry.Get(compressorType);
            return CompressCss(outputFilePath, files, compressor).ToString();
        }

        public void ClearCache()
        {
            renderedCssFiles.Clear();
            debugCssFiles.Clear();
        }

        private StringBuilder CompressCss(string outputFilePath, List<string> files, ICssCompressor compressor)
        {
            var outputCss = new StringBuilder();
            foreach (string file in files)
            {
                string css;
                if (file.ToLower().EndsWith(".less") || file.ToLower().EndsWith(".less.css"))
                {
                    css = ProcessLess(file);
                }
                else
                {
                    css = ReadFile(file);
                }
                css = CssPathRewriter.RewriteCssPaths(outputFilePath, file, css);
                outputCss.Append(compressor.CompressContent(css));
            }
            return outputCss;
        }        

        private string ProcessLess(string file)
        {
            var content = ReadFile(file);
            var engine = new ExtensibleEngine();
            var md = new MinifierDecorator(engine);
            var lso = new LessSourceObject();
            lso.Cacheable = false;
            lso.Content = content;
            lso.Key = file;
            return md.TransformToCss(lso);
        }
    }
}