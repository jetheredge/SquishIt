using NUnit.Framework;
using SquishIt.Framework.Css;
using SquishIt.Framework.Files;
using SquishIt.Framework.Utilities;
using SquishIt.Tests.Helpers;
using SquishIt.Tests.Stubs;

namespace SquishIt.Tests 
{
    [TestFixture]
    public class StylePreprocessorPipelineTests 
    {
        private CssBundleFactory cssBundleFactory;
        private IHasher hasher;

        [SetUp]
        public void Setup () 
        {
            cssBundleFactory = new CssBundleFactory ();
            var retryableFileOpener = new RetryableFileOpener ();
            hasher = new Hasher (retryableFileOpener);
        }

        [Test]
        public void Css_Style_Then_Global () 
        {
            var stylePreprocessor = new StubStylePreprocessor ();
            var globalPreprocessor = new StubGlobalPreprocessor ();

            using (new StylePreprocessorScope<StubStylePreprocessor>(stylePreprocessor))
            using (new GlobalPreprocessorScope<StubGlobalPreprocessor> (globalPreprocessor)) 
            {
                CSSBundle cssBundle = cssBundleFactory
                    .WithHasher (hasher)
                    .WithDebuggingEnabled (false)
                    .WithContents ("start")
                    .Create ();

                string tag = cssBundle
                    .Add ("~/css/test.global.style")
                    .Render ("~/css/output.css");

                string contents =
                    cssBundleFactory.FileWriterFactory.Files[TestUtilities.PrepareRelativePath (@"css\output.css")];

                Assert.AreEqual ("<link rel=\"stylesheet\" type=\"text/css\" href=\"css/output.css?r=6816A2FDB0EE7941EE20E53D23779FC7\" />", tag);
                Assert.AreEqual ("globey", contents);

                Assert.IsTrue (stylePreprocessor.WasCalled);
                Assert.IsTrue (globalPreprocessor.WasCalled);
            }
        }

        [Test]
        public void Css_Global_Then_Style () 
        {
            var stylePreprocessor = new StubStylePreprocessor ();
            var globalPreprocessor = new StubGlobalPreprocessor ();

            using (new StylePreprocessorScope<StubStylePreprocessor> (stylePreprocessor))
            using (new GlobalPreprocessorScope<StubGlobalPreprocessor> (globalPreprocessor)) 
            {
                CSSBundle cssBundle = cssBundleFactory
                    .WithHasher (hasher)
                    .WithDebuggingEnabled (false)
                    .WithContents ("start")
                    .Create ();

                string tag = cssBundle
                    .Add ("~/css/test.style.global")
                    .Render ("~/css/output.css");

                string contents =
                    cssBundleFactory.FileWriterFactory.Files[TestUtilities.PrepareRelativePath (@"css\output.css")];

                Assert.AreEqual ("<link rel=\"stylesheet\" type=\"text/css\" href=\"css/output.css?r=0B5C0EC6F2D8CEA452236626242443B7\" />", tag);
                Assert.AreEqual ("styley", contents);

                Assert.IsTrue (stylePreprocessor.WasCalled);
                Assert.IsTrue (globalPreprocessor.WasCalled);
            }
        }
    }
}
