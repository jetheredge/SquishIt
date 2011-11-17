using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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

        protected override string[] allowedExtensions
        {
            get { return new [] {".CSS"}; }
        }

        protected override string tagFormat
        {
            get { return "<style type=\"text/css\">{0}</style>"; }
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

        protected override string BeforeMinify(string outputFile, List<string> filePaths, IEnumerable<string> arbitraryContent)
        {
            var outputCss = new StringBuilder();

            filePaths.Select(file => ProcessCssFile (file, outputFile))
                .Concat(arbitraryContent)
                .Aggregate(outputCss, (builder, val) => builder.Append(val + "\n"));

            return outputCss.ToString();
        }

        private string PreProcessCssFile(string file, IPreprocessor preProcessor)
        {
            lock (typeof(CSSBundle))
            {
                try
                {
                    currentDirectoryWrapper.SetCurrentDirectory(Path.GetDirectoryName(file));
                    var content = ReadFile(file);
                    return preProcessor.Process(file, content);
                }
                finally
                {
                    currentDirectoryWrapper.Revert();
                }
            }
        }

        string ProcessCssFile(string file, string outputFile) {
            string css = null;

            var preProcessor = FindPreProcessor(file);

            if(preProcessor != null)
            {
                css = PreProcessCssFile(file, preProcessor);
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
                var fileResolver = new FileSystemResolver();
                fileHasher = new CssAssetsFileHasher(HashKeyName, fileResolver, hasher);
            }

            return CSSPathRewriter.RewriteCssPaths(outputFile, file, css, fileHasher);
        }

        private static IPreprocessor FindPreProcessor(string file)
        {
            return Bundle.CssPreprocessors.FirstOrDefault(p => p.ValidFor(file));
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
                    var preProcessor = FindPreProcessor(localPath);
                    if(preProcessor != null)
                    {
                        string outputFile = FileSystem.ResolveAppRelativePathToFileSystem(localPath);

                        string css = PreProcessCssFile(outputFile, preProcessor);
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