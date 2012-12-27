using NUnit.Framework;
using SquishIt.Framework.CSS;
using SquishIt.Framework.Minifiers;
using SquishIt.Framework.Minifiers.CSS;

namespace SquishIt.Tests
{
    [TestFixture]
    public class CSSMinifierTests
    {
        string css = @" li {
                                    margin-bottom:0.1em;
                                    margin-left:0;
                                    margin-top:0.1em;
                                }
                                /* comment */
                                /*
                                    multiline
                                    comment
                                */
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
            var cssCompressor = MinifierFactory.Get<CSSBundle, NullMinifier>();
            var uncompressedCss = cssCompressor.Minify(css);
            Assert.AreEqual(css + "\n", uncompressedCss);
        }

        [Test]
        public void YuiCompressorTest()
        {
            var cssCompressor = MinifierFactory.Get<CSSBundle, YuiMinifier>();
            var compressedCss = cssCompressor.Minify(css);
            Assert.AreEqual("li{margin-bottom:.1em;margin-left:0;margin-top:.1em}th{font-weight:normal;vertical-align:bottom}.FloatRight{float:right}.FloatLeft{float:left}", compressedCss);
        }

        [Test]
        public void MsCompressorTest()
        {
            var cssCompressor = MinifierFactory.Get<CSSBundle, MsMinifier>();
            var compressedCss = cssCompressor.Minify(css);
            Assert.AreEqual("li{margin-bottom:.1em;margin-left:0;margin-top:.1em}th{font-weight:normal;vertical-align:bottom}.FloatRight{float:right}.FloatLeft{float:left}", compressedCss);
        }
    }
}