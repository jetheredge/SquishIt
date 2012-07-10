using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using SquishIt.Framework.Base;
using SquishIt.Framework.Minifiers;
using SquishIt.Framework.Resolvers;
using SquishIt.Framework.Files;
using SquishIt.Framework.Utilities;

namespace SquishIt.Framework.Css
{
    public class CSSBundle : BundleBase<CSSBundle>
    {
        readonly static Regex IMPORT_PATTERN = new Regex(@"@import +url\(([""']){0,1}(.*?)\1{0,1}\);", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        const string CSS_TEMPLATE = "<link rel=\"stylesheet\" type=\"text/css\" {0}href=\"{1}\" />";
        const string CACHE_PREFIX = "css";
        const string TAG_FORMAT = "<style type=\"text/css\">{0}</style>";

        bool ShouldImport { get; set; }
        bool ShouldAppendHashForAssets { get; set; }


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
            get { return Configuration.Instance.DefaultCssMinifier(); }
        }

        protected override IEnumerable<string> allowedExtensions
        {
            get { return bundleState.AllowedExtensions.Union(Bundle.AllowedGlobalExtensions.Union(Bundle.AllowedStyleExtensions)); }
        }

        protected override IEnumerable<string> disallowedExtensions
        {
            get { return Bundle.AllowedScriptExtensions; }
        }

        protected override string defaultExtension
        {
            get { return ".CSS"; }
        }

        protected override string tagFormat
        {
            get { return bundleState.Typeless ? TAG_FORMAT.Replace(" type=\"text/css\"", "") : TAG_FORMAT; }
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

        string ProcessImport(string file, string outputFile, string css)
        {
            var sourcePath = FileSystem.ResolveFileSystemPathToAppRelative(Path.GetDirectoryName(file)) + "/";

            return IMPORT_PATTERN.Replace(css, match =>
            {
                var importPath = match.Groups[2].Value;
                string import;
                if(importPath.StartsWith("/"))
                {
                    import = FileSystem.ResolveAppRelativePathToFileSystem(importPath);
                }
                else
                {
                    import = FileSystem.ResolveAppRelativePathToFileSystem(sourcePath + importPath);
                }
                bundleState.DependentFiles.Add(import);
                return ProcessCssFile(import, outputFile, true);
            });
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

        protected override void AggregateContent(List<Asset> assets, StringBuilder sb, string outputFile)
        {
            assets.SelectMany(a => a.IsArbitrary
                                       ? new[] { PreprocessArbitrary(a) }.AsEnumerable()
                                       : GetFilesForSingleAsset(a).Select(f => ProcessFile(f, outputFile, a.Minify)))
                .ToList()
                .Distinct()
                .Aggregate(sb, (b, s) =>
                                   {
                                       b.Append(s);
                                       return b;
                                   });
        }

        protected override string ProcessFile(string file, string outputFile, bool minify)
        {
            return MinifyIfNeeded(ProcessCssFile(file, outputFile), minify);
        }

        string ProcessCssFile(string file, string outputFile, bool asImport = false)
        {
            string css = null;

            var preprocessors = FindPreprocessors(file);

            if(preprocessors.NullSafeAny())
            {
                css = PreprocessFile(file, preprocessors);
            }
            else
            {
                css = ReadFile(file);
            }

            if(ShouldImport)
            {
                css = ProcessImport(file, outputFile, css);
            }

            ICssAssetsFileHasher fileHasher = null;

            if(ShouldAppendHashForAssets)
            {
                var fileResolver = new FileSystemResolver();
                fileHasher = new CssAssetsFileHasher(bundleState.HashKeyName, fileResolver, hasher);
            }

            return CSSPathRewriter.RewriteCssPaths(outputFile, file, css, fileHasher, asImport);
        }
    }
}