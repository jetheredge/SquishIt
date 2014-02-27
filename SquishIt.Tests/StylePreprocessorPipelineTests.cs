using System.Linq;
using NUnit.Framework;
using SquishIt.Framework;
using SquishIt.Framework.Base;
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
            hasher = new StubHasher("hash");
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

                Assert.AreEqual("<link rel=\"stylesheet\" type=\"text/css\" href=\"css/output.css?r=hash\" />", tag);
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

                Assert.AreEqual("<link rel=\"stylesheet\" type=\"text/css\" href=\"css/output.css?r=hash\" />", tag);
                Assert.AreEqual("styley", contents);

                Assert.AreEqual("globey", stylePreprocessor.CalledWith);
                Assert.AreEqual("start", globalPreprocessor.CalledWith);
            }
        }

        [Test]
        public void Css_Stops_At_First_Extension_With_No_Defined_Preprocessor()
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

                Assert.AreEqual("<link rel=\"stylesheet\" type=\"text/css\" href=\"css/output.css?r=hash\" />", tag);
                Assert.AreEqual("start", contents);

                Assert.Null(stylePreprocessor.CalledWith);
                Assert.Null(globalPreprocessor.CalledWith);
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

            Assert.AreEqual("<link rel=\"stylesheet\" type=\"text/css\" href=\"css/output.css?r=hash\" />", tag);
            Assert.AreEqual("styley", contents);

            Assert.AreEqual("globey", stylePreprocessor.CalledWith);
            Assert.AreEqual("start", globalPreprocessor.CalledWith);

            Assert.IsEmpty(Bundle.Preprocessors.Where(x => !(x is NullPreprocessor)));
        }
    }
}
