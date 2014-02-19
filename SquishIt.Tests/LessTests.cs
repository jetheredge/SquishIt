using System;
using NUnit.Framework;
using SquishIt.Framework.CSS;
using SquishIt.Framework.Files;
using SquishIt.Framework.Utilities;
using SquishIt.Less;
using SquishIt.Tests.Helpers;
using SquishIt.Tests.Stubs;
using SquishIt.Framework;
using System.Threading;

namespace SquishIt.Tests 
{
    [TestFixture]
    public class LessTests
    {
        string cssLess = TestUtilities.NormalizeLineEndings (@"@brand_color: #4D926F;

                                    #header {
                                        color: @brand_color;
                                    }
 
                                    h2 {
                                        color: @brand_color;
                                    }");

        CSSBundleFactory cssBundleFactory;
        IHasher hasher;
        private IPathTranslator translator = Configuration.Instance.DefaultPathTranslator();

        [SetUp]
        public void Setup () {
            cssBundleFactory = new CSSBundleFactory ();
            var retryableFileOpener = new RetryableFileOpener ();
            hasher = new Hasher (retryableFileOpener);
        }

        [Test]
        public void CanBundleCssWithLess () {
            using (new StylePreprocessorScope<LessPreprocessor> ()) {
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
        public void CanBundleCssWithNestedLess()
        {
            string importCss =
                @"
                @import 'other.less';
                #header {
                    color: #4D926F;
                }";

            CSSBundle cssBundle = cssBundleFactory
                .WithDebuggingEnabled(false)
                .WithContents(importCss)
                .Create()
                .WithPreprocessor(new LessPreprocessor());

            TestUtilities.CreateFile("other.less", "#footer{color:#ffffff}");
            cssBundle
                .Add("~/css/test.less")
                .Render("~/css/output_test.css");

            TestUtilities.DeleteFile("other.less");

            Assert.AreEqual("#footer{color:#fff}#header{color:#4d926f}", cssBundleFactory.FileWriterFactory.Files[TestUtilities.PrepareRelativePath(@"css\output_test.css")]);
            Assert.Contains(translator.ResolveAppRelativePathToFileSystem("css/other.less"), cssBundle.bundleState.DependentFiles);
        }

        [Test]
        public void CanBundleNestedLessInDifferentDirectoriesMultiThreaded()
        {
            // This test is dependant on a race condition so may not cause a 
            // failure every time even when a bug is present. However on a
            // multicore machine it seems to have a high failure rate.

            string importCssA =
                @"
                @import 'other.less';
                .cssA {
                    color: #AAAAAA;
                }";

            string importCssB =
                @"
                @import 'other.less';
                .cssB {
                    color: #BBBBBB;
                }";

            cssBundleFactory.FileReaderFactory.SetContentsForFile(TestUtilities.PrepareRelativePath(@"css_A\test.less"), importCssA);
            cssBundleFactory.FileReaderFactory.SetContentsForFile(TestUtilities.PrepareRelativePath(@"css_B\test.less"), importCssB);

            TestUtilities.CreateFile("css_A/test.less", importCssA);
            TestUtilities.CreateFile("css_B/test.less", importCssB);

            TestUtilities.CreateFile("css_A/other.less", "#cssA{color:#ffffff}");
            TestUtilities.CreateFile("css_B/other.less", "#cssB{color:#000000}");

            var dirWrapper = new DirectoryWrapper();

            CSSBundle cssBundleA = cssBundleFactory
                .WithCurrentDirectoryWrapper(dirWrapper)
                .WithDebuggingEnabled(false)
                //.WithContents(importCssA)
                .Create()
                .WithPreprocessor(new LessPreprocessor());

            CSSBundle cssBundleB = cssBundleFactory
                .WithCurrentDirectoryWrapper(dirWrapper)
                .WithDebuggingEnabled(false)
                //.WithContents(importCssB)
                .Create()
                .WithPreprocessor(new LessPreprocessor());

            // Trigger the parsing of two different .less files in different
            // directories at the same time. Both import a file called other.less
            // which should be found in their own directory, but issues with 
            // changing the current directory at the wrong time could cause
            // them to pick up the imported file from the incorrect location.
            var taskA = new Thread(() =>
            {
                var sa = cssBundleA
                    .Add("css_A/test.less")
                    .Render("css_A/output_test.css");
            });

            var taskB = new Thread(() =>
            {
                var sb = cssBundleB
                    .Add("css_B/test.less")
                    .Render("css_B/output_test.css");
            });

            taskA.Start();
            taskB.Start();

            taskA.Join();
            taskB.Join();

            TestUtilities.DeleteFile("css_A/test.less");
            TestUtilities.DeleteFile("css_B/test.less");
            TestUtilities.DeleteFile("css_A/other.less");
            TestUtilities.DeleteFile("css_B/other.less");

            Assert.AreEqual("#cssA{color:#fff}.cssA{color:#aaa}", cssBundleFactory.FileWriterFactory.Files[TestUtilities.PrepareRelativePath(@"css_A\output_test.css")]);
            Assert.AreEqual("#cssB{color:#000}.cssB{color:#bbb}", cssBundleFactory.FileWriterFactory.Files[TestUtilities.PrepareRelativePath(@"css_B\output_test.css")]);
            Assert.Contains(translator.ResolveAppRelativePathToFileSystem("css_A/other.less"), cssBundleA.bundleState.DependentFiles);
            Assert.Contains(translator.ResolveAppRelativePathToFileSystem("css_B/other.less"), cssBundleB.bundleState.DependentFiles);
        }
 

        [Test]
        public void CanBundleCssWithArbitraryLess ()
        {
            var tag = cssBundleFactory
                .WithHasher(hasher)
                .WithDebuggingEnabled(false)
                .Create()
                .WithPreprocessor(new LessPreprocessor())
                .AddString(cssLess, ".less")
                .Render("~/css/output.css");

            var contents =
                cssBundleFactory.FileWriterFactory.Files[TestUtilities.PrepareRelativePath(@"css\output.css")];

            Assert.AreEqual("#header{color:#4d926f}h2{color:#4d926f}", contents);

            Assert.AreEqual ("<link rel=\"stylesheet\" type=\"text/css\" href=\"css/output.css?r=15D3D9555DEFACE69D6AB9E7FD972638\" />", tag);
        }

        [Test]
        public void CanBundleCssInDebugWithArbitraryLess()
        {
            var tag = cssBundleFactory
                .WithHasher(hasher)
                .WithDebuggingEnabled(true)
                .Create()
                .WithPreprocessor(new LessPreprocessor())
                .AddString(cssLess, ".less")
                .Render("~/css/output.css");

            var expected = TestUtilities.NormalizeLineEndings(@"<style type=""text/css"">#header {
  color: #4d926f;
}
h2 {
  color: #4d926f;
}
</style>") + Environment.NewLine; //account for stringbuilder

            Assert.AreEqual(expected, tag);
        }

        [Test]
        public void CanBundleCssWithLessAndPathRewrites () {
            using (new StylePreprocessorScope<LessPreprocessor> ()) {
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

                cssBundle
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
            using (new StylePreprocessorScope<LessPreprocessor> ()) {
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
