using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using dotless.Core;
using SquishIt.Framework.Base;
using SquishIt.Framework.Minifiers;
using SquishIt.Framework.Minifiers.CSS;
using SquishIt.Framework.Resolvers;
using SquishIt.Framework.Files;
using SquishIt.Framework.Utilities;

namespace SquishIt.Framework.Css
{
    public class CSSBundle : BundleBase<CSSBundle>
    {
        private const string MEDIA_ALL = "all";
        private static Regex IMPORT_PATTERN = new Regex(@"@import +url\(([""']){0,1}(.*?)\1{0,1}\);", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private const string CSS_TEMPLATE = "<link rel=\"stylesheet\" type=\"text/css\" {0}href=\"{1}\" />";
        private const string CACHE_PREFIX = "css";

        private bool ShouldImport { get; set; }
        private bool ShouldAppendHashForAssets { get; set; }

        protected override string Template
        {
            get { return CSS_TEMPLATE; }
        }

    	protected override string CachePrefix
        {
            get { return CACHE_PREFIX; }
        }

    	protected override IMinifier<CSSBundle> DefaultMinifier
    	{
    		get { return new MsCompressor(); }
    	}

    	public CSSBundle()
            : base(new FileWriterFactory(new RetryableFileOpener(), 5), new FileReaderFactory(new RetryableFileOpener(), 5), new DebugStatusReader(), new CurrentDirectoryWrapper(), new Hasher(new RetryableFileOpener()), new BundleCache())
        {
        }

    	public CSSBundle(IDebugStatusReader debugStatusReader)
            : base(new FileWriterFactory(new RetryableFileOpener(), 5), new FileReaderFactory(new RetryableFileOpener(), 5), debugStatusReader, new CurrentDirectoryWrapper(), new Hasher(new RetryableFileOpener()), new BundleCache())
        {
        }

    	public CSSBundle(IDebugStatusReader debugStatusReader, IFileWriterFactory fileWriterFactory, IFileReaderFactory fileReaderFactory, ICurrentDirectoryWrapper currentDirectoryWrapper, IHasher hasher, IBundleCache bundleCache)
            : base(fileWriterFactory, fileReaderFactory, debugStatusReader, currentDirectoryWrapper, hasher, bundleCache)
        {
        }

    	private string ProcessLess(string file)
        {
            lock (typeof(CSSBundle))
            {
                try
                {
                    currentDirectoryWrapper.SetCurrentDirectory(Path.GetDirectoryName(file));
                    var content = ReadFile(file);
                    var engineFactory = new EngineFactory();
                    var engine = engineFactory.GetEngine();
                    return engine.TransformToCss(content, file);
                }
                finally
                {
                    currentDirectoryWrapper.Revert();
                }
            }
        }

    	private string ProcessImport(string css)
        {
            return IMPORT_PATTERN.Replace(css, new MatchEvaluator(ApplyFileContentsToMatchedImport));
        }

    	private string ApplyFileContentsToMatchedImport(Match match)
        {
            var file = FileSystem.ResolveAppRelativePathToFileSystem(match.Groups[2].Value);
            DependentFiles.Add(file);
            return ReadFile(file);
        }

    	public CSSBundle ProcessImports()
        {
            ShouldImport = true;
            return this;
        }

    	public CSSBundle AppendHashForAssets()
        {
            ShouldAppendHashForAssets = true;
            return this;
        }

    	protected override string BeforeMinify(string outputFile, List<string> filePaths)
        {
            var outputCss = new StringBuilder();
            foreach (string file in filePaths)
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

                if (ShouldImport)
                {
                    css = ProcessImport(css);
                }

                ICssAssetsFileHasher fileHasher = null;

                if (ShouldAppendHashForAssets)
                {
                    var fileResolver = new FileResolver();
                    fileHasher = new CssAssetsFileHasher(HashKeyName, fileResolver, hasher);
                }

                css = CSSPathRewriter.RewriteCssPaths(outputFile, file, css, fileHasher);
                outputCss.Append(css + "\n");
            }

            return outputCss.ToString();
        }

        internal override Dictionary<string, GroupBundle> BeforeRenderDebug()
        {
            var modifiedGroupBundles = new Dictionary<string, GroupBundle>(GroupBundles);

            foreach (var groupBundleKVP in modifiedGroupBundles)
            {
                var groupBundle = groupBundleKVP.Value;
                var assets = groupBundle.Assets;

                foreach (var asset in groupBundle.Assets)
                {
                    var localPath = asset.LocalPath;
                    if (localPath.ToLower().EndsWith(".less") || localPath.ToLower().EndsWith(".less.css"))
                    {
                        string outputFile = FileSystem.ResolveAppRelativePathToFileSystem(localPath);
                        string css = ProcessLess(outputFile);
                        outputFile += ".debug.css";
                        using (var fileWriter = fileWriterFactory.GetFileWriter(outputFile))
                        {
                            fileWriter.Write(css);
                        }

                        asset.LocalPath = localPath + ".debug.css";
                    }
                }
            }

            return modifiedGroupBundles;
        }
    }
}