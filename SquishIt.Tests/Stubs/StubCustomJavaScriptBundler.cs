using System.Collections.Generic;
using SquishIt.Framework;
using SquishIt.Framework.Files;
using SquishIt.Framework.JavaScript;
using SquishIt.Framework.Utilities;

namespace SquishIt.Tests.Stubs
{
    public class StubCustomJavaScriptBundler : JavaScriptBundle
    {
        public StubCustomJavaScriptBundler(IDebugStatusReader debugStatusReader, IFileWriterFactory fileWriterFactory, IFileReaderFactory fileReaderFactory, ICurrentDirectoryWrapper currentDirectoryWrapper, IHasher hasher, IBundleCache bundleCache)
            : base(debugStatusReader, fileWriterFactory, fileReaderFactory, currentDirectoryWrapper, hasher, bundleCache)
        { }

        protected override string BeforeMinify(string outputFile, List<string> files, IEnumerable<string> arbitraryContent)
        {
            return base.BeforeMinify(outputFile, files, arbitraryContent);
        }
    }

    public class StubCustomNoHashJavaScriptBundler : JavaScriptBundle
    {
        public StubCustomNoHashJavaScriptBundler(IDebugStatusReader debugStatusReader, IFileWriterFactory fileWriterFactory, IFileReaderFactory fileReaderFactory, ICurrentDirectoryWrapper currentDirectoryWrapper, IHasher hasher, IBundleCache bundleCache)
            : base(debugStatusReader, fileWriterFactory, fileReaderFactory, currentDirectoryWrapper, hasher, bundleCache)
        { }

        protected override string BeforeMinify(string outputFile, List<string> files, IEnumerable<string> arbitraryContent)
        {
            // Set the hash key to empty to keep it from being appended in Render.
            HashKeyNamed(string.Empty);

            return base.BeforeMinify(outputFile, files, arbitraryContent);
        }
    }
}
