using Bundler.Framework.Css.Compressors;
using NUnit.Framework;

namespace Bundler.Tests
{
    [TestFixture]
    public class CssCompressorsTests
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

        [Test]
        public void NullCompressorTest()
        {
            var cssCompressor = CssCompressorRegistry.Get(NullCompressor.Identifier);
            var uncompressedCss = cssCompressor.CompressContent(css);
            Assert.AreEqual(css, uncompressedCss);
        }
        
        [Test]
        public void YuiCompressorTest()
        {
            var cssCompressor = CssCompressorRegistry.Get(YuiCompressor.Identifier);
            var compressedCss = cssCompressor.CompressContent(css);
            Assert.AreEqual("li{margin-bottom:.1em;margin-left:0;margin-top:.1em;}th{font-weight:normal;vertical-align:bottom;}.FloatRight{float:right;}.FloatLeft{float:left;}", compressedCss);
        }
    }
}