using NUnit.Framework;
using SquishIt.Framework.Css;
using SquishIt.Framework.Files;
using SquishIt.Framework.Utilities;
using SquishIt.Preprocessors;
using SquishIt.Tests.Helpers;
using SquishIt.Tests.Stubs;

namespace SquishIt.Tests 
{
    [TestFixture]
    public class LessTests
    {
        private string cssLess = TestUtilities.NormalizeLineEndings (@"@brand_color: #4D926F;

                                    #header {
                                        color: @brand_color;
                                    }
 
                                    h2 {
                                        color: @brand_color;
                                    }");

        private CssBundleFactory cssBundleFactory;
        private IHasher hasher;

        [SetUp]
        public void Setup () {
            cssBundleFactory = new CssBundleFactory ();
            var retryableFileOpener = new RetryableFileOpener ();
            hasher = new Hasher (retryableFileOpener);
        }

        [Test]
        public void CanBundleCssWithLess () {
            using (new PreprocessorScope<LessPreprocessor> ()) {
                CSSBundle cssBundle = cssBundleFactory
                    .WithHasher (hasher)
                    .WithDebuggingEnabled (false)
                    .WithContents (cssLess)
                    .Create ();

                string tag = cssBundle
                    .Add ("~/css/test.less")
                    .Render ("~/css/output.css");

                string contents =
                    cssBundleFactory.FileWriterFactory.Files[TestUtilities.PrepareRelativePath (@"css\output.css")];

                Assert.AreEqual (
                    "<link rel=\"stylesheet\" type=\"text/css\" href=\"css/output.css?r=15D3D9555DEFACE69D6AB9E7FD972638\" />",
                    tag);
                Assert.AreEqual ("#header{color:#4d926f}h2{color:#4d926f}", contents);
            }
        }

        [Test]
        public void CanBundleCssWithLessAndPathRewrites () {
            using (new PreprocessorScope<LessPreprocessor> ()) {
                string css =
                    @"@brand_color: #4D926F;
                        #header {
                            color: @brand_color;
                            background-image: url(../image/mygif.gif);
                        }
                    ";

                CSSBundle cssBundle = cssBundleFactory
                    .WithDebuggingEnabled (false)
                    .WithContents (css)
                    .Create ();

                string tag = cssBundle
                    .Add ("~/css/something/test.less")
                    .Render ("~/css/output_less_with_rewrites.css");

                string contents =
                    cssBundleFactory.FileWriterFactory.Files[
                        TestUtilities.PrepareRelativePath (@"css\output_less_with_rewrites.css")];

                Assert.AreEqual ("#header{color:#4d926f;background-image:url(image/mygif.gif)}", contents);
            }
        }

        [Test]
        public void CanBundleCssWithLessWithLessDotCssFileExtension () {
            using (new PreprocessorScope<LessPreprocessor> ()) {
                CSSBundle cssBundle = cssBundleFactory
                    .WithHasher (hasher)
                    .WithDebuggingEnabled (false)
                    .WithContents (cssLess)
                    .Create ();

                string tag = cssBundle
                    .Add ("~/css/test.less.css")
                    .Render ("~/css/output_less_dot_css.css");

                string contents =
                    cssBundleFactory.FileWriterFactory.Files[
                        TestUtilities.PrepareRelativePath (@"css\output_less_dot_css.css")];

                Assert.AreEqual (
                    "<link rel=\"stylesheet\" type=\"text/css\" href=\"css/output_less_dot_css.css?r=15D3D9555DEFACE69D6AB9E7FD972638\" />",
                    tag);
                Assert.AreEqual ("#header{color:#4d926f}h2{color:#4d926f}", contents);
            }
        }
    }
}
