using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using dotless.Core;
using SquishIt.Framework.Css.Compressors;
using SquishIt.Framework.Files;
using SquishIt.Framework.JavaScript;
using SquishIt.Framework.Utilities;

namespace SquishIt.Framework.Css
{
    internal class CssBundle : BundleBase, ICssBundle, ICssBundleBuilder
    {
        private static BundleCache bundleCache = new BundleCache();
        private static Dictionary<string, string> debugCssFiles = new Dictionary<string, string>();
        private static Dictionary<string, NamedState> namedState = new Dictionary<string, NamedState>();
        private List<string> cssFiles = new List<string>();
        private List<string> remoteCssFiles = new List<string>();
        private List<string> embeddedResourceCssFiles = new List<string>();
        private List<string> dependentFiles = new List<string>();
        private string mediaTag = "";
        private CssCompressors cssCompressor = CssCompressors.MsCompressor;
        private bool renderOnlyIfOutputFileMissing = false;
        private bool processImports = false;
        private const string CssTemplate = "<link rel=\"stylesheet\" type=\"text/css\" {0} href=\"{1}\" />";
        private static readonly Regex importPattern = new Regex(@"@import +url\(([""']){0,1}(.*?)\1{0,1}\);", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        
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

        ICssBundleBuilder ICssBundle.AddRemote(string localPath, string remotePath)
        {
            if (debugStatusReader.IsDebuggingEnabled())
            {
                cssFiles.Add(localPath);
            }
            else
            {
                remoteCssFiles.Add(remotePath);
            }
            return this;
        }

        ICssBundleBuilder ICssBundleBuilder.AddRemote(string javaScriptPath, string cdnUri)
        {
            if (debugStatusReader.IsDebuggingEnabled())
            {
                cssFiles.Add(javaScriptPath);
            }
            else
            {
                remoteCssFiles.Add(cdnUri);
            }
            return this;
        }

        void AddEmbeddedResource(string localPath, string embeddedResourcePath)
        {
            if (debugStatusReader.IsDebuggingEnabled())
            {
                cssFiles.Add(localPath);
            }
            else
            {
                embeddedResourceCssFiles.Add(embeddedResourcePath);
            }
        }

        ICssBundleBuilder ICssBundle.AddEmbeddedResource(string localPath, string embeddedResourcePath)
        {
            AddEmbeddedResource(localPath, embeddedResourcePath);
            return this;
        }

        ICssBundleBuilder ICssBundleBuilder.AddEmbeddedResource(string localPath, string embeddedResourcePath)
        {
            AddEmbeddedResource(localPath, embeddedResourcePath);
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
            namedState[name] = new NamedState(debugStatusReader.IsDebuggingEnabled(), renderTo);
            Render(renderTo, name);
        }

        public ICssBundleBuilder RenderOnlyIfOutputFileMissing()
        {
            renderOnlyIfOutputFileMissing = true;
            return this;
        }

        public ICssBundleBuilder ProcessImports()
        {
            processImports = true;
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
            NamedState state = namedState[name];
            if (state.Debug)
            {
                return debugCssFiles[name];
            }
            return RenderRelease(name, state.RenderTo);
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

            return RenderRelease(key, renderTo);
        }

        private string RenderRelease(string key, string renderTo)
        {
            if (!bundleCache.ContainsKey(key))
            {
                lock (bundleCache)
                {
                    if (!bundleCache.ContainsKey(key))
                    {
                        string compressedCss;
                        string hash= null;
                        bool hashInFileName = false;

                        dependentFiles.Clear();

                        string outputFile = ResolveAppRelativePathToFileSystem(renderTo);
                        List<string> files = GetFiles(GetFilePaths(cssFiles));
                        files.AddRange(GetFiles(GetEmbeddedResourcePaths(embeddedResourceCssFiles)));
                        dependentFiles.AddRange(files);
                        string identifier = MapCompressorToIdentifier(cssCompressor);

                        if (renderTo.Contains("#"))
                        {
                            hashInFileName = true;
                            compressedCss = CompressCss(outputFile, files, identifier);
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
                            compressedCss = CompressCss(outputFile, files, identifier);
                            WriteCssToFiles(compressedCss, outputFile, null);
                        }
                        
                        if (hash == null)
                        {
                            hash = Hasher.Create(compressedCss);
                        }

                        string renderedCssTag;
                        if (hashInFileName)
                        {
                            renderedCssTag = FillTemplate(mediaTag, ExpandAppRelativePath(renderTo));
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
                        renderedCssTag = String.Concat(GetFilesForRemote(), renderedCssTag);
                        bundleCache.AddToCache(key, renderedCssTag, dependentFiles);
                    }
                }
            }
            return bundleCache.GetContent(key);
        }

        private string MapCompressorToIdentifier(CssCompressors compressors)
        {
            switch (compressors)
            {
                case CssCompressors.NullCompressor:
                    return NullCompressor.Identifier;
                case CssCompressors.YuiCompressor:
                    return YuiCompressor.Identifier;
                case CssCompressors.MsCompressor:
                    return MsCompressor.Identifier;
                default:
                    return MsCompressor.Identifier;
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
            bundleCache.ClearTestingCache();
            debugCssFiles.Clear();
            namedState.Clear();
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
                
                if (processImports)
                {
                    css = ProcessImport(css);
                }
                css = CssPathRewriter.RewriteCssPaths(outputFilePath, file, css);
                outputCss.Append(compressor.CompressContent(css));
            }
            return outputCss;
        }

        private string ProcessLess(string file)
        {
            var content = ReadFile(file);
            var engineFactory = new EngineFactory();
            var engine = engineFactory.GetEngine();
            return engine.TransformToCss(content, file);
        }

        private string ProcessImport(string css)
        {
            return importPattern.Replace(css, new MatchEvaluator(ApplyFileContentsToMatchedImport));
        }

        private string ApplyFileContentsToMatchedImport(Match match)
        {
            var file = ResolveAppRelativePathToFileSystem(match.Groups[2].Value);
            dependentFiles.Add(file);
            return ReadFile(file);
        }

        private string GetFilesForRemote()
        {
            var renderedCssFilesForCdn = new StringBuilder();
            foreach (var uri in remoteCssFiles)
            {
                renderedCssFilesForCdn.Append(FillTemplate(mediaTag, uri));
            }
            return renderedCssFilesForCdn.ToString();
        }

        private string FillTemplate(string mediaTag, string path)
        {
            return String.Format(CssTemplate, mediaTag, path);
        }
    }
}