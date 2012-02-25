using System.Collections.Generic;
using SquishIt.Framework.JavaScript;

namespace SquishItAspNetTest.Bundlers
{
    public class NoHashJavaScriptBundle : JavaScriptBundle
    {
        public NoHashJavaScriptBundle()
            : base()
        { }

        protected override string BeforeMinify(string outputFile, List<string> files, IEnumerable<string> arbitraryContent)
        {
            // Set the hash key to empty so the render doesn't append it.
            HashKeyNamed(string.Empty);

            return base.BeforeMinify(outputFile, files, arbitraryContent);
        }
    }
}