using NUnit.Framework;
using SquishIt.Framework.Css;
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
        private JavaScriptBundleFactory javaScriptBundleFactory;
        private IHasher hasher;

        [SetUp]
        public void Setup () 
        {
            javaScriptBundleFactory = new JavaScriptBundleFactory();
            var retryableFileOpener = new RetryableFileOpener ();
            hasher = new Hasher (retryableFileOpener);
        }

        [Test]
        public void Css_Style_Then_Global () 
        {
            var scriptPreprocessor = new StubScriptPreprocessor();
            var globalPreprocessor = new StubGlobalPreprocessor ();

            using (new ScriptPreprocessorScope<StubScriptPreprocessor>(scriptPreprocessor))
            using (new GlobalPreprocessorScope<StubGlobalPreprocessor> (globalPreprocessor)) 
            {
                JavaScriptBundle javaScriptBundle = javaScriptBundleFactory
                    .WithHasher (hasher)
                    .WithDebuggingEnabled (false)
                    .WithContents ("start")
                    .Create ();

                string tag = javaScriptBundle
                    .Add ("~/css/test.global.script")
                    .Render ("~/css/output.css");

                string contents =
                    javaScriptBundleFactory.FileWriterFactory.Files[TestUtilities.PrepareRelativePath (@"css\output.css")];

                Assert.AreEqual ("<script type=\"text/javascript\" src=\"css/output.css?r=6816A2FDB0EE7941EE20E53D23779FC7\"></script>", tag);
                Assert.AreEqual ("globey", contents);

                Assert.IsTrue (scriptPreprocessor.WasCalled);
                Assert.IsTrue (globalPreprocessor.WasCalled);
            }
        }

        [Test]
        public void Css_Global_Then_Style () 
        {
            var scriptPreprocessor = new StubScriptPreprocessor ();
            var globalPreprocessor = new StubGlobalPreprocessor ();

            using (new ScriptPreprocessorScope<StubScriptPreprocessor> (scriptPreprocessor))
            using (new GlobalPreprocessorScope<StubGlobalPreprocessor> (globalPreprocessor)) 
            {
                JavaScriptBundle javaScriptBundle = javaScriptBundleFactory
                    .WithHasher (hasher)
                    .WithDebuggingEnabled (false)
                    .WithContents ("start")
                    .Create ();

                string tag = javaScriptBundle
                    .Add ("~/css/test.script.global")
                    .Render ("~/css/output.css");

                string contents =
                    javaScriptBundleFactory.FileWriterFactory.Files[TestUtilities.PrepareRelativePath (@"css\output.css")];

                Assert.AreEqual ("<script type=\"text/javascript\" src=\"css/output.css?r=A87E7F7EA72FD18FC3E75FACD850E2EB\"></script>", tag);
                Assert.AreEqual ("scripty", contents);

                Assert.IsTrue (scriptPreprocessor.WasCalled);
                Assert.IsTrue (globalPreprocessor.WasCalled);
            }
        }
    }
}
