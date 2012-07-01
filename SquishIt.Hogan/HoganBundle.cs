using System.Collections.Generic;
using System.Linq;
using System.Text;
using SquishIt.Framework;
using SquishIt.Framework.Base;
using SquishIt.Framework.Files;
using SquishIt.Framework.Minifiers;
using SquishIt.Framework.Utilities;

//TODO: is this needed to enforce that minification is not performed?  If so a better way to expose would be nice (mimicking Bundle.XXX() methods)
namespace SquishIt.Hogan
{
    public class HoganBundle : BundleBase<HoganBundle>
    {
        public HoganBundle(IFileWriterFactory fileWriterFactory, IFileReaderFactory fileReaderFactory,
                           IDebugStatusReader debugStatusReader, ICurrentDirectoryWrapper currentDirectoryWrapper,
                           IHasher hasher, IBundleCache bundleCache)
            : base(fileWriterFactory, fileReaderFactory, debugStatusReader, currentDirectoryWrapper, hasher,
                   bundleCache)
        {
        }

        public HoganBundle() :
            base(new FileWriterFactory(new RetryableFileOpener(), 5),
                 new FileReaderFactory(new RetryableFileOpener(), 5),
                 new DebugStatusReader(), new CurrentDirectoryWrapper(),
                 new Hasher(new RetryableFileOpener()), new BundleCache())
        {
        }


        protected override IMinifier<HoganBundle> DefaultMinifier
        {
            get { return new HoganMinifier(); }
        }

        protected override IEnumerable<string> allowedExtensions
        {
            get { return new List<string>(new[] {".HTML"}); }
        }

        protected override IEnumerable<string> disallowedExtensions
        {
            get { return new List<string>();}
        }

        protected override string defaultExtension
        {
            get { return ".JS"; }
        }

        protected override string ProcessFile(string file, string outputFile)
        {
            var preprocessors = FindPreprocessors(file);
            if (preprocessors != null)
            {
                return PreprocessFile(file, preprocessors);
            }
            return ReadFile(file);
        }

        protected override void AggregateContent(List<Asset> assets, StringBuilder sb, string outputFile)
        {
            assets.SelectMany(a => GetFilesForSingleAsset(a).Select(f => ProcessFile(f, outputFile)))
                .ToList()
                .Distinct()
                .Aggregate(sb, (b, s) => b.Append(s + "\n"));
        }

        protected override string tagFormat
        {
            get { return "<script>{0}</script>"; }
        }

        protected override string Template
        {
            get { return "<script {0}src=\"{1}\"></script>"; }
        }

        protected override string CachePrefix
        {
            get { return "HtmlTemplate"; }
        }
    }

    public class HoganMinifier : IMinifier<HoganBundle>
    {
        public string Minify(string content)
        {
            return content;
        }
    }
}