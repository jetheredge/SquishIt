using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using SquishIt.Framework.Base;
using SquishIt.Framework.Files;
using SquishIt.Framework.Minifiers;
using SquishIt.Framework.Utilities;

namespace SquishIt.Framework.JavaScript
{
    public class JavaScriptBundle : BundleBase<JavaScriptBundle>
    {
        private const string JS_TEMPLATE = "<script type=\"text/javascript\" {0}src=\"{1}\" defer></script>";
        private const string TAG_FORMAT = "<script type=\"text/javascript\">{0}</script>";

        private const string CACHE_PREFIX = "js";

        private bool deferred;

        protected override IMinifier<JavaScriptBundle> DefaultMinifier
        {
            get { return Configuration.Instance.DefaultJsMinifier(); }
        }

        private HashSet<string> _allowedExtensions = new HashSet<string> { ".JS", ".COFFEE" };

        protected override HashSet<string> allowedExtensions
        {
            get { return _allowedExtensions; }
        }

        protected override string tagFormat
        {
            get { return typeless ? TAG_FORMAT.Replace(" type=\"text/javascript\"", "") : TAG_FORMAT; }
        }

        public JavaScriptBundle()
            : base(new FileWriterFactory(new RetryableFileOpener(), 5), new FileReaderFactory(new RetryableFileOpener(), 5), new DebugStatusReader(), new CurrentDirectoryWrapper(), new Hasher(new RetryableFileOpener()), new BundleCache()) { }

        public JavaScriptBundle(IDebugStatusReader debugStatusReader)
            : base(new FileWriterFactory(new RetryableFileOpener(), 5), new FileReaderFactory(new RetryableFileOpener(), 5), debugStatusReader, new CurrentDirectoryWrapper(), new Hasher(new RetryableFileOpener()), new BundleCache()) { }

        public JavaScriptBundle(IDebugStatusReader debugStatusReader, IFileWriterFactory fileWriterFactory, IFileReaderFactory fileReaderFactory, ICurrentDirectoryWrapper currentDirectoryWrapper, IHasher hasher, IBundleCache bundleCache) :
            base(fileWriterFactory, fileReaderFactory, debugStatusReader, currentDirectoryWrapper, hasher, bundleCache) { }

        protected override string Template
        {
            get
            {
                var val = typeless ? JS_TEMPLATE.Replace("type=\"text/javascript\" ", "") : JS_TEMPLATE;
                return deferred ? val : val.Replace(" defer", "");
            }
        }

        protected override string CachePrefix
        {
            get { return CACHE_PREFIX; }
        }

        internal override string PreprocessForDebugging(string filename)
        {
            if(filename.ToLower().EndsWith(".coffee"))
            {
                string js = ProcessCoffee(filename);
                filename += ".debug.js";
                using(var fileWriter = fileWriterFactory.GetFileWriter(filename))
                {
                    fileWriter.Write(js);
                }
            }
            return filename;
        }

        protected override void AggregateContent(List<Asset> assets, StringBuilder sb, string outputFile)
        {
            assets.SelectMany(a => a.IsArbitrary ? new[] { a.Content }.AsEnumerable() :
                    GetFilesForSingleAsset(a).Select(ReadFile))
                .ToList()
                .Distinct()
                .Aggregate(sb, (b, s) =>
                {
                    b.Append(s + "\n");
                    return b;
                });
        }
        private string ProcessCoffee(string file)
        {
            lock(typeof(JavaScriptBundle))
            {
                try
                {
                    currentDirectoryWrapper.SetCurrentDirectory(Path.GetDirectoryName(file));
                    var content = ReadFile(file);
                    var compiler = new Coffee.CoffeescriptCompiler();
                    currentDirectoryWrapper.Revert();
                    return compiler.Compile(content);
                }
                catch
                {
                    currentDirectoryWrapper.Revert();
                    throw;
                }
            }
        }

        public JavaScriptBundle WithDeferredLoad()
        {
            deferred = true;
            return this;
        }
    }
}