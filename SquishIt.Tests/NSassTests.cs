using System;
using NUnit.Framework;
using SquishIt.Framework.Files;
using SquishIt.Framework.Utilities;
using SquishIt.NSass;
using SquishIt.Tests.Helpers;
using SquishIt.Tests.Stubs;

namespace SquishIt.Tests
{
    [TestFixture(Category="IgnoreCI", Description="Assembly loading issues on build server.")]
    public class NSassTests
    {
        CSSBundleFactory cssBundleFactory;
        IHasher hasher;
        string scss = @"$blue: #3bbfce;
                    $margin: 16px;

                    .content-navigation {
                      border-color: $blue;
                      color:
                        darken($blue, 9%);
                    }

                    .border {
                      padding: $margin / 2;
                      margin: $margin / 2;
                      border-color: $blue;
                    }";

        string renderedCss = @".content-navigation{border-color:#3bbfce;color:#2ca2af}.border{padding:8px;margin:8px;border-color:#3bbfce}";
       

        [SetUp]
        public void Setup()
        {
            cssBundleFactory = new CSSBundleFactory();
            var retryableFileOpener = new RetryableFileOpener();
            hasher = new Hasher(retryableFileOpener);
        }

        [Test]
        public void CanBundleCssWithScss()
        {
            using (new StylePreprocessorScope<NSassPreprocessor>())
            {
                var cssBundle = cssBundleFactory
                    .WithHasher(hasher)
                    .WithDebuggingEnabled(false)
                    .WithContents(scss)
                    .Create();

                string tag = cssBundle
                    .Add("~/css/test.scss")
                    .Render("~/css/output.css");

                string contents =
                    cssBundleFactory.FileWriterFactory.Files[TestUtilities.PrepareRelativePath(@"css\output.css")];

                Assert.AreEqual(
                    @"<link rel=""stylesheet"" type=""text/css"" href=""css/output.css?r=5C851B7837C923C399A44B1F5BF9F14A"" />",
                    tag);
                Assert.AreEqual(renderedCss, contents);
            }
        }


        [Test]
        public void CanBundleCssWithArbitraryScss()
        {
            using (new StylePreprocessorScope<NSassPreprocessor>())
            {
                var cssBundle = cssBundleFactory
                    .WithHasher(hasher)
                    .WithDebuggingEnabled(false)
                    .Create();

                string tag = cssBundle
                    .AddString(scss, ".scss")
                    .Render("~/css/output.css");

                string contents = cssBundleFactory.FileWriterFactory.Files[TestUtilities.PrepareRelativePath(@"css\output.css")];

                Assert.AreEqual(@"<link rel=""stylesheet"" type=""text/css"" href=""css/output.css?r=5C851B7837C923C399A44B1F5BF9F14A"" />", tag);
                Assert.AreEqual(renderedCss, contents);
            }
        }

        [Test]
        public void CanBundleCssInDebugWithArbitraryScss()
        {
            using (new StylePreprocessorScope<NSassPreprocessor>())
            {
                var cssBundle = cssBundleFactory
                    .WithHasher(hasher)
                    .WithDebuggingEnabled(true)
                    .Create();

                var tag = cssBundle
                    .AddString(scss, ".scss")
                    .Render("~/css/output.css");

                var expected = TestUtilities.NormalizeLineEndings(
@"<style type=""text/css"">/* line 4, source string */
.content-navigation {
  border-color: #3bbfce;
  color: #2ca2af; }

/* line 10, source string */
.border {
  padding: 8px;
  margin: 8px;
  border-color: #3bbfce; }
</style>") + Environment.NewLine;//account for stringbuilder

                Assert.AreEqual(expected, tag);
            }
        }

        [Test]
        public void CanUseNesting()
        {
            using (new StylePreprocessorScope<NSassPreprocessor>())
            {
                var original =
                    @"table.hl {
                      margin: 2em 0;
                      td.ln {
                        text-align: right;
                      }
                    }

                    li {
                      font: {
                        family: serif;
                        weight: bold;
                        size: 1.2em;
                      }
                    }";

                var expected =
                    @"table.hl{margin:2em 0}table.hl td.ln{text-align:right}li{font-family:serif;font-weight:bold;font-size:1.2em}";

                var cssBundle = cssBundleFactory
                    .WithHasher(hasher)
                    .WithDebuggingEnabled(false)
                    .WithContents(original)
                    .Create();

                cssBundle
                    .Add("~/css/test.scss")
                    .Render("~/css/output.css");

                string contents =
                    cssBundleFactory.FileWriterFactory.Files[TestUtilities.PrepareRelativePath(@"css\output.css")];
                Assert.AreEqual(expected, contents);
            }
        }

        [Test]
        public void CanUseMixins()
        {
            using (new StylePreprocessorScope<NSassPreprocessor>())
            {
                var original =
                    @"@mixin table-base {
                      th {
                        text-align: center;
                        font-weight: bold;
                      }
                      td, th {padding: 2px}
                    }

                    @mixin left($dist) {
                      float: left;
                      margin-left: $dist;
                    }

                    #data {
                      @include left(10px);
                      @include table-base;
                    }";

                var expected =
                    @"#data{float:left;margin-left:10px}#data th{text-align:center;font-weight:bold}#data td,#data th{padding:2px}";

                var cssBundle = cssBundleFactory
                    .WithHasher(hasher)
                    .WithDebuggingEnabled(false)
                    .WithContents(original)
                    .Create();

                cssBundle
                    .Add("~/css/test.scss")
                    .Render("~/css/output.css");

                string contents =
                    cssBundleFactory.FileWriterFactory.Files[TestUtilities.PrepareRelativePath(@"css\output.css")];
                Assert.AreEqual(expected, contents);
            }
        }

        [Test]
        public void CanUseSelectorInheritance()
        {
            using (new StylePreprocessorScope<NSassPreprocessor>())
            {
                var original =
                    @".error {
                      margin-right: 1px;
                    }
                    .error.intrusion {
                      margin-left: 1px;
                    }
                    .badError {
                      @extend .error;
                      margin-top: 1px;
                    }";

                var expected = @".error,.badError{margin-right:1px}.error.intrusion{margin-left:1px}.badError{margin-top:1px}";

                var cssBundle = cssBundleFactory
                    .WithHasher(hasher)
                    .WithDebuggingEnabled(false)
                    .WithContents(original)
                    .Create();

                cssBundle
                    .Add("~/css/test.scss")
                    .Render("~/css/output.css");

                var contents = cssBundleFactory.FileWriterFactory.Files[TestUtilities.PrepareRelativePath(@"css\output.css")];

                Assert.AreEqual(expected, contents);
            }
        }
    }
}
