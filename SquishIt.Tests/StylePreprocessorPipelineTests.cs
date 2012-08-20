using NUnit.Framework;
using SquishIt.Framework;
using SquishIt.Framework.CSS;
using SquishIt.Framework.Files;
using SquishIt.Framework.Utilities;
using SquishIt.Tests.Helpers;
using SquishIt.Tests.Stubs;

namespace SquishIt.Tests
{
    [TestFixture]
    public class StylePreprocessorPipelineTests
    {
        CSSBundleFactory cssBundleFactory;
        IHasher hasher;

        [SetUp]
        public void Setup()
        {
            cssBundleFactory = new CSSBundleFactory();
            var retryableFileOpener = new RetryableFileOpener();
            hasher = new Hasher(retryableFileOpener);
        }

        [Test]
        public void Css_Style_Then_Global()
        {
            var stylePreprocessor = new StubStylePreprocessor();
            var globalPreprocessor = new StubGlobalPreprocessor();

            using(new StylePreprocessorScope<StubStylePreprocessor>(stylePreprocessor))
            using(new GlobalPreprocessorScope<StubGlobalPreprocessor>(globalPreprocessor))
            {
                CSSBundle cssBundle = cssBundleFactory
                    .WithHasher(hasher)
                    .WithDebuggingEnabled(false)
                    .WithContents("start")
                    .Create();

                string tag = cssBundle
                    .Add("~/css/test.global.style")
                    .Render("~/css/output.css");

                string contents =
                    cssBundleFactory.FileWriterFactory.Files[TestUtilities.PrepareRelativePath(@"css\output.css")];

                Assert.AreEqual("<link rel=\"stylesheet\" type=\"text/css\" href=\"css/output.css?r=6816A2FDB0EE7941EE20E53D23779FC7\" />", tag);
                Assert.AreEqual("globey", contents);

                Assert.AreEqual("start", stylePreprocessor.CalledWith);
                Assert.AreEqual("styley", globalPreprocessor.CalledWith);
            }
        }

        [Test]
        public void Css_Global_Then_Style()
        {
            var stylePreprocessor = new StubStylePreprocessor();
            var globalPreprocessor = new StubGlobalPreprocessor();

            using(new StylePreprocessorScope<StubStylePreprocessor>(stylePreprocessor))
            using(new GlobalPreprocessorScope<StubGlobalPreprocessor>(globalPreprocessor))
            {
                CSSBundle cssBundle = cssBundleFactory
                    .WithHasher(hasher)
                    .WithDebuggingEnabled(false)
                    .WithContents("start")
                    .Create();

                string tag = cssBundle
                    .Add("~/css/test.style.global")
                    .Render("~/css/output.css");

                string contents =
                    cssBundleFactory.FileWriterFactory.Files[TestUtilities.PrepareRelativePath(@"css\output.css")];

                Assert.AreEqual("<link rel=\"stylesheet\" type=\"text/css\" href=\"css/output.css?r=0B5C0EC6F2D8CEA452236626242443B7\" />", tag);
                Assert.AreEqual("styley", contents);

                Assert.AreEqual("globey", stylePreprocessor.CalledWith);
                Assert.AreEqual("start", globalPreprocessor.CalledWith);
            }
        }

        [Test]
        public void Css_Skips_Extensions_With_No_Preprocessors()
        {
            var stylePreprocessor = new StubStylePreprocessor();
            var globalPreprocessor = new StubGlobalPreprocessor();

            using(new StylePreprocessorScope<StubStylePreprocessor>(stylePreprocessor))
            using(new GlobalPreprocessorScope<StubGlobalPreprocessor>(globalPreprocessor))
            {
                CSSBundle cssBundle = cssBundleFactory
                    .WithHasher(hasher)
                    .WithDebuggingEnabled(false)
                    .WithContents("start")
                    .Create();

                string tag = cssBundle
                    .Add("~/css/test.style.fake.global.bogus")
                    .Render("~/css/output.css");

                string contents =
                    cssBundleFactory.FileWriterFactory.Files[TestUtilities.PrepareRelativePath(@"css\output.css")];

                Assert.AreEqual("<link rel=\"stylesheet\" type=\"text/css\" href=\"css/output.css?r=0B5C0EC6F2D8CEA452236626242443B7\" />", tag);
                Assert.AreEqual("styley", contents);

                Assert.AreEqual("globey", stylePreprocessor.CalledWith);
                Assert.AreEqual("start", globalPreprocessor.CalledWith);
            }
        }

        [Test]
        public void WithPreprocessor_Uses_Instance_Preprocessors()
        {
            var stylePreprocessor = new StubStylePreprocessor();
            var globalPreprocessor = new StubGlobalPreprocessor();

            CSSBundle cssBundle = cssBundleFactory
                .WithHasher(hasher)
                .WithDebuggingEnabled(false)
                .WithContents("start")
                .Create();

            string tag = cssBundle
                .Add("~/css/test.style.global")
                .WithPreprocessor(stylePreprocessor)
                .WithPreprocessor(globalPreprocessor)
                .Render("~/css/output.css");

            string contents =
                cssBundleFactory.FileWriterFactory.Files[TestUtilities.PrepareRelativePath(@"css\output.css")];

            Assert.AreEqual("<link rel=\"stylesheet\" type=\"text/css\" href=\"css/output.css?r=0B5C0EC6F2D8CEA452236626242443B7\" />", tag);
            Assert.AreEqual("styley", contents);

            Assert.AreEqual("globey", stylePreprocessor.CalledWith);
            Assert.AreEqual("start", globalPreprocessor.CalledWith);

            Assert.IsEmpty(Bundle.Preprocessors);
        }
    }
}
