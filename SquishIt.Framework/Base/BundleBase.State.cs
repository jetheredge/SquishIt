using System;
using System.Collections.Generic;
using SquishIt.Framework.Files;
using SquishIt.Framework.Minifiers;
using SquishIt.Framework.Renderers;
using SquishIt.Framework.Utilities;

namespace SquishIt.Framework.Base
{
    public abstract partial class BundleBase<T> where T : BundleBase<T>
	{
        private static readonly Dictionary<string, string> renderPathCache = new Dictionary<string, string>();

        protected string BaseOutputHref = Configuration.Instance.DefaultOutputBaseHref() ?? String.Empty;
        protected IFileWriterFactory fileWriterFactory;
        protected IFileReaderFactory fileReaderFactory;
        protected IDebugStatusReader debugStatusReader;
        protected ICurrentDirectoryWrapper currentDirectoryWrapper;
        protected IHasher hasher;
        protected abstract IMinifier<T> DefaultMinifier { get; }

        protected abstract string tagFormat { get; }
        protected bool typeless;
        protected abstract string Template { get; }
        protected abstract string CachePrefix { get; }

        protected HashSet<string> instanceAllowedExtensions = new HashSet<string>();
        protected IList<IPreprocessor> instancePreprocessors = new List<IPreprocessor>();
        protected abstract IEnumerable<string> allowedExtensions { get; }
        protected abstract IEnumerable<string> disallowedExtensions { get; }
        protected abstract string defaultExtension { get; }

        private IMinifier<T> minifier;
        protected IMinifier<T> Minifier
        {
            get
            {
                return minifier ?? DefaultMinifier;
            }
            set { minifier = value; }
        }

        protected string HashKeyName { get; set; }
        private bool ShouldRenderOnlyIfOutputFileIsMissing { get; set; }
        internal List<string> DependentFiles = new List<string>();
        internal BundleState bundleState = new BundleState();

        private static Dictionary<string, BundleState> bundleStateCache = new Dictionary<string, BundleState>();

        private IBundleCache bundleCache;
        private IRenderer releaseFileRenderer;

        protected BundleBase(IFileWriterFactory fileWriterFactory, IFileReaderFactory fileReaderFactory, IDebugStatusReader debugStatusReader, ICurrentDirectoryWrapper currentDirectoryWrapper, IHasher hasher, IBundleCache bundleCache)
        {
            this.fileWriterFactory = fileWriterFactory;
            this.fileReaderFactory = fileReaderFactory;
            this.debugStatusReader = debugStatusReader;
            this.currentDirectoryWrapper = currentDirectoryWrapper;
            this.hasher = hasher;
            ShouldRenderOnlyIfOutputFileIsMissing = false;
            HashKeyName = "r";
            this.bundleCache = bundleCache;
        }
	}
}
