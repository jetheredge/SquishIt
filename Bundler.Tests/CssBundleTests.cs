using System.IO;
using Bundler.Framework;
using Bundler.Framework.Tests.Mocks;
using NUnit.Framework;

namespace Bundler.Tests
{
    [TestFixture]
    public class CssBundleTests
    {
        private string css = @" li {
                                    margin-bottom:0.1em;
                                    margin-left:0;
                                    margin-top:0.1em;
                                }

                                th {
                                    font-weight:normal;
                                    vertical-align:bottom;
                                }

                                .FloatRight {
                                    float:right;
                                }
                                
                                .FloatLeft {
                                    float:left;
                                }";


        private string cssLess =
                                    @"@brand_color: #4D926F;

                                    #header {
                                        color: @brand_color;
                                    }
 
                                    h2 {
                                        color: @brand_color;
                                    }";

        [Test]
        public void CanBundleCss()
        {
            string tempInputFile = Path.GetTempFileName();
            string tempOutputFile = Path.GetTempFileName();

            try
            {
                using (var sw = new StreamWriter(tempInputFile))
                {
                    sw.Write(css);
                }

                string tag = Bundle.Css()
                    .Add(tempInputFile)
                    .Render(tempOutputFile);

                string output;
                using (var sr = new StreamReader(tempOutputFile))
                {
                    output = sr.ReadToEnd();
                }

                Assert.AreEqual("li{margin-bottom:.1em;margin-left:0;margin-top:.1em;}th{font-weight:normal;vertical-align:bottom;}.FloatRight{float:right;}.FloatLeft{float:left;}", output);
                Assert.IsTrue(tag.StartsWith("<link rel=\"stylesheet\" type=\"text/css\"  href=\""));
                Assert.IsTrue(tag.EndsWith("\" />"));
            }
            finally
            {
                File.Delete(tempInputFile);
                File.Delete(tempOutputFile);
            }
        }

        [Test]
        public void CanBundleCssWithLess()
        {
            string tempInputFile = Path.GetTempPath() + Path.GetRandomFileName() + ".less";            
            string tempOutputFile = Path.GetTempFileName();

            try
            {
                using (var sw = new StreamWriter(tempInputFile))
                {
                    sw.Write(cssLess);
                }

                Bundle.Css()
                    .Add(tempInputFile)
                    .Render(tempOutputFile);

                string output;
                using (var sr = new StreamReader(tempOutputFile))
                {
                    output = sr.ReadToEnd();
                }

                Assert.AreEqual("#header,h2{color:#4d926f;}", output);
            }
            finally
            {
                File.Delete(tempInputFile);
                File.Delete(tempOutputFile);
            }
        }

        [Test]
        public void CanCreateNamedBundle()
        {
            string tempInputFile = Path.GetTempFileName();
            string tempOutputFile = Path.GetTempFileName();

            try
            {
                using (var sw = new StreamWriter(tempInputFile))
                {
                    sw.Write(css);
                }

                Bundle.Css()
                    .Add(tempInputFile)
                    .AsNamed("Test", tempOutputFile);

                string tag = Bundle.Css()
                                .RenderNamed("Test");

                string output;
                using (var sr = new StreamReader(tempOutputFile))
                {
                    output = sr.ReadToEnd();
                }

                Assert.AreEqual("li{margin-bottom:.1em;margin-left:0;margin-top:.1em;}th{font-weight:normal;vertical-align:bottom;}.FloatRight{float:right;}.FloatLeft{float:left;}", output);
                Assert.IsTrue(tag.StartsWith("<link rel=\"stylesheet\" type=\"text/css\"  href=\""));
                Assert.IsTrue(tag.EndsWith("\" />"));
            }
            finally
            {
                File.Delete(tempInputFile);
                File.Delete(tempOutputFile);
            }
        }

        [Test]
        public void CanRenderDebugTags()
        {
            string tempInputFile = Path.GetTempFileName();
            string tempOutputFile = Path.GetTempFileName();

            try
            {
                using (var sw = new StreamWriter(tempInputFile))
                {
                    sw.Write(css);
                }

                ICssBundle cssBundle = new CssBundle(new MockDebugStatusReader());
                string tag = cssBundle.Add(tempInputFile)
                        .Render(tempOutputFile);

                string output;
                using (var sr = new StreamReader(tempOutputFile))
                {
                    output = sr.ReadToEnd();
                }

                Assert.AreEqual("", output);
                Assert.IsTrue(tag.StartsWith("<link rel=\"stylesheet\" type=\"text/css\"  href=\""));
                Assert.IsTrue(tag.EndsWith("\" />"));
            }
            finally
            {
                File.Delete(tempInputFile);
                File.Delete(tempOutputFile);
            }
        }
    }
}