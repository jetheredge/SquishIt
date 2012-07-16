using NUnit.Framework;
using SquishIt.Framework;
using SquishIt.Framework.Files;
using SquishIt.Framework.JavaScript;
using SquishIt.Framework.Utilities;
using SquishIt.Tests.Helpers;
using SquishIt.Tests.Stubs;

namespace SquishIt.Tests
{
    [TestFixture]
    public class ScriptPreprocessorPipelineTests
    {
        JavaScriptBundleFactory javaScriptBundleFactory;
        IHasher hasher;

        [SetUp]
        public void Setup()
        {
            javaScriptBundleFactory = new JavaScriptBundleFactory();
            var retryableFileOpener = new RetryableFileOpener();
            hasher = new Hasher(retryableFileOpener);
        }

        [Test]
        public void Js_Style_Then_Global()
        {
            var scriptPreprocessor = new StubScriptPreprocessor();
            var globalPreprocessor = new StubGlobalPreprocessor();

            using(new ScriptPreprocessorScope<StubScriptPreprocessor>(scriptPreprocessor))
            using(new GlobalPreprocessorScope<StubGlobalPreprocessor>(globalPreprocessor))
            {
                JavaScriptBundle javaScriptBundle = javaScriptBundleFactory
                    .WithHasher(hasher)
                    .WithDebuggingEnabled(false)
                    .WithContents("start")
                    .Create();

                string tag = javaScriptBundle
                    .Add("~/js/test.global.script")
                    .Render("~/js/output.js");

                string contents =
                    javaScriptBundleFactory.FileWriterFactory.Files[TestUtilities.PrepareRelativePath(@"js\output.js")];

                Assert.AreEqual("<script type=\"text/javascript\" src=\"js/output.js?r=4AC018380A2609F7B20456FA1200CBD7\"></script>", tag);
                Assert.AreEqual("globey;", contents);

                Assert.AreEqual("start", scriptPreprocessor.CalledWith);
                Assert.AreEqual("scripty", globalPreprocessor.CalledWith);
            }
        }

        [Test]
        public void Js_Global_Then_Style()
        {
            var scriptPreprocessor = new StubScriptPreprocessor();
            var globalPreprocessor = new StubGlobalPreprocessor();

            using(new ScriptPreprocessorScope<StubScriptPreprocessor>(scriptPreprocessor))
            using(new GlobalPreprocessorScope<StubGlobalPreprocessor>(globalPreprocessor))
            {
                JavaScriptBundle javaScriptBundle = javaScriptBundleFactory
                    .WithHasher(hasher)
                    .WithDebuggingEnabled(false)
                    .WithContents("start")
                    .Create();

                string tag = javaScriptBundle
                    .Add("~/js/test.script.global")
                    .Render("~/js/output.js");

                string contents =
                    javaScriptBundleFactory.FileWriterFactory.Files[TestUtilities.PrepareRelativePath(@"js\output.js")];

                Assert.AreEqual("<script type=\"text/javascript\" src=\"js/output.js?r=1CAFA2D8FF256D13A3BBBFB4770AC743\"></script>", tag);
                Assert.AreEqual("scripty;", contents);

                Assert.AreEqual("globey", scriptPreprocessor.CalledWith);
                Assert.AreEqual("start", globalPreprocessor.CalledWith);
            }
        }

        [Test]
        public void Js_Skips_Extensions_With_No_Preprocessors()
        {
            var scriptPreprocessor = new StubScriptPreprocessor();
            var globalPreprocessor = new StubGlobalPreprocessor();

            using(new ScriptPreprocessorScope<StubScriptPreprocessor>(scriptPreprocessor))
            using(new GlobalPreprocessorScope<StubGlobalPreprocessor>(globalPreprocessor))
            {
                JavaScriptBundle javaScriptBundle = javaScriptBundleFactory
                    .WithHasher(hasher)
                    .WithDebuggingEnabled(false)
                    .WithContents("start")
                    .Create();

                string tag = javaScriptBundle
                    .Add("~/js/test.script.fake.global.bogus")
                    .Render("~/js/output.js");

                string contents =
                    javaScriptBundleFactory.FileWriterFactory.Files[TestUtilities.PrepareRelativePath(@"js\output.js")];

                Assert.AreEqual("<script type=\"text/javascript\" src=\"js/output.js?r=1CAFA2D8FF256D13A3BBBFB4770AC743\"></script>", tag);
                Assert.AreEqual("scripty;", contents);

                Assert.AreEqual("globey", scriptPreprocessor.CalledWith);
                Assert.AreEqual("start", globalPreprocessor.CalledWith);
            }
        }

        [Test]
        public void WithPreprocessor_Uses_Instance_Preprocessors()
        {
            var scriptPreprocessor = new StubScriptPreprocessor();
            var globalPreprocessor = new StubGlobalPreprocessor();

            JavaScriptBundle javaScriptBundle = javaScriptBundleFactory
                .WithHasher(hasher)
                .WithDebuggingEnabled(false)
                .WithContents("start")
                .Create();

            string tag = javaScriptBundle
                .WithPreprocessor(scriptPreprocessor)
                .WithPreprocessor(globalPreprocessor)
                .Add("~/js/test.script.fake.global.bogus")
                .Render("~/js/output.js");

            string contents =
                javaScriptBundleFactory.FileWriterFactory.Files[TestUtilities.PrepareRelativePath(@"js\output.js")];

            Assert.AreEqual("<script type=\"text/javascript\" src=\"js/output.js?r=1CAFA2D8FF256D13A3BBBFB4770AC743\"></script>", tag);
            Assert.AreEqual("scripty;", contents);

            Assert.AreEqual("globey", scriptPreprocessor.CalledWith);
            Assert.AreEqual("start", globalPreprocessor.CalledWith);

            Assert.IsEmpty(Bundle.Preprocessors);
        }
    }
}
