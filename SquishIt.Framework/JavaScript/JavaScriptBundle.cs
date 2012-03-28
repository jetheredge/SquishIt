using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using SquishIt.Framework.Base;
using SquishIt.Framework.Files;
using SquishIt.Framework.Minifiers;
using SquishIt.Framework.Minifiers.JavaScript;
using SquishIt.Framework.Utilities;

namespace SquishIt.Framework.JavaScript
{
    public class JavaScriptBundle : BundleBase<JavaScriptBundle>
    {
        private const string JS_TEMPLATE = "<script type=\"text/javascript\" {0}src=\"{1}\"></script>";
        private const string TAG_FORMAT = "<script type=\"text/javascript\">{0}</script>";

        private const string CACHE_PREFIX = "js";

        protected override IMinifier<JavaScriptBundle> DefaultMinifier
        {
            get { return Configuration.DefaultJsMinifier(); }
        }

        public static void RegisterPreprocessor<T>() where T : IPreprocessor
        {
            var instance = Activator.CreateInstance<T>();
            foreach (var ext in instance.Extensions) _allowedExtensions.Add(ext);
            Bundle.RegisterPreprocessor<T>(instance);
        }

        public static void ClearPreprocessors() {
            var remove = _allowedExtensions.Where(ax => ax != _defaultExtension).ToArray();
            _allowedExtensions.RemoveWhere(remove.Contains);
            Bundle.RemovePreprocessors(remove);
        }

        static string _defaultExtension = ".JS";
        static HashSet<string> _allowedExtensions = new HashSet<string> { _defaultExtension };

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
            get { return typeless ? JS_TEMPLATE.Replace("type=\"text/javascript\" ", "") : JS_TEMPLATE; }
        }

        protected override string CachePrefix
        {
            get { return CACHE_PREFIX; }
        }

        internal override void BeforeRenderDebug()
        {
            foreach (var asset in bundleState.Assets)
            {
                var localPath = asset.LocalPath;
                var preprocessor = FindPreprocessor(localPath);
                if (preprocessor != null)
                {
                    string outputFile = FileSystem.ResolveAppRelativePathToFileSystem(localPath);
                    string javascript = PreprocessJavascriptFile(outputFile, preprocessor);
                    outputFile += ".debug.js";
                    using (var fileWriter = fileWriterFactory.GetFileWriter(outputFile))
                    {
                        fileWriter.Write(javascript);
                    }

                    asset.LocalPath = localPath + ".debug.js";
                }
            }
        }

        protected override string BeforeMinify(string outputFile, List<string> files, IEnumerable<string> arbitraryContent)
        {
            var sb = new StringBuilder();

            files.Select(ProcessJavascriptFile)
                .Concat(arbitraryContent)
                .Aggregate(sb, (builder, val) => builder.Append(val + "\n"));

            return sb.ToString();
        }

        private string ProcessJavascriptFile(string file) {
            var preprocessor = FindPreprocessor(file);
            if (preprocessor != null) {
                return PreprocessJavascriptFile(file, preprocessor);
            }
            return ReadFile(file);
        }

        private string PreprocessJavascriptFile(string file, IPreprocessor preprocessor) {
            lock (typeof(JavaScriptBundle)) {
                try {
                    currentDirectoryWrapper.SetCurrentDirectory(Path.GetDirectoryName(file));
                    var content = ReadFile(file);
                    if (preprocessor == null) {
                        return content;
                    }
                    return preprocessor.Process(file, content);
                }
                finally {
                    currentDirectoryWrapper.Revert();
                }
            }
        }
    }
}