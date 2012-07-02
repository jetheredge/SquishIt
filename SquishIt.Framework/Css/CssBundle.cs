using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using dotless.Core;
using SquishIt.Framework.Base;
using SquishIt.Framework.Minifiers;
using SquishIt.Framework.Resolvers;
using SquishIt.Framework.Files;
using SquishIt.Framework.Utilities;

namespace SquishIt.Framework.Css
{
    public class CSSBundle : BundleBase<CSSBundle>
    {
        private readonly static Regex IMPORT_PATTERN = new Regex(@"@import +url\(([""']){0,1}(.*?)\1{0,1}\);", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private const string CSS_TEMPLATE = "<link rel=\"stylesheet\" type=\"text/css\" {0}href=\"{1}\" />";
        private const string CACHE_PREFIX = "css";
        private const string TAG_FORMAT = "<style type=\"text/css\">{0}</style>";

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

        protected override string defaultExtension { get { return ".CSS"; } }

        protected override IMinifier<CSSBundle> DefaultMinifier
        {
            get { return Configuration.Instance.DefaultCssMinifier(); }
        }

        private HashSet<string> _allowedExtensions = new HashSet<string> { ".CSS", ".LESS" };

        protected override HashSet<string> allowedExtensions
        {
            get { return _allowedExtensions; }
        }

        protected override string tagFormat
        {
            get { return typeless ? TAG_FORMAT.Replace(" type=\"text/css\"", "") : TAG_FORMAT; }
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
            lock(typeof(CSSBundle))
            {
                try
                {
                    var dir = Path.GetDirectoryName(file);
                    currentDirectoryWrapper.SetCurrentDirectory(dir);
                    var content = ReadFile(file);
                    var engineFactory = new EngineFactory();
                    var engine = engineFactory.GetEngine();
                    var css = engine.TransformToCss(content, file);

                    var appPath = FileSystem.ResolveFileSystemPathToAppRelative(dir);
                    var importPaths = engine.GetImports();
                    foreach(var importPath in importPaths)
                    {
                        var import = FileSystem.ResolveAppRelativePathToFileSystem(Path.Combine(appPath, importPath));
                        DependentFiles.Add(import);
                    }
                    currentDirectoryWrapper.Revert();
                    return css;
                }
                catch
                {
                    currentDirectoryWrapper.Revert();
                    throw;
                }
            }
        }

        private string ProcessImport(string file, string outputFile, string css)
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
                DependentFiles.Add(import);
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
            assets.SelectMany(a => a.IsArbitrary ? new[] { a.Content }.AsEnumerable() :
                GetFilesForSingleAsset(a).Select(f => ProcessCssFile(f, outputFile)))
                .ToList()
                .Distinct()
                .Aggregate(sb, (b, s) =>
                {
                    b.Append(s + "\n");
                    return b;
                });
        }

        string ProcessCssFile(string file, string outputFile, bool asImport = false)
        {
            string css = null;
            if(file.ToLower().EndsWith(".less") || file.ToLower().EndsWith(".less.css"))
            {
                css = ProcessLess(file);
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
                fileHasher = new CssAssetsFileHasher(HashKeyName, fileResolver, hasher);
            }

            return CSSPathRewriter.RewriteCssPaths(outputFile, file, css, fileHasher, asImport);
        }

        internal override string PreprocessForDebugging(string filename)
        {
            if(filename.ToLower().EndsWith(".less") || filename.ToLower().EndsWith(".less.css"))
            {
                string css = ProcessLess(filename);
                filename += debugExtension;
                using(var fileWriter = fileWriterFactory.GetFileWriter(filename))
                {
                    fileWriter.Write(css);
                }
            }
            return filename;
        }
    }
}