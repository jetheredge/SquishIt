using NUnit.Framework;
using SquishIt.Framework.JavaScript;
using SquishIt.Framework.Minifiers;
using SquishIt.Framework.Minifiers.JavaScript;

namespace SquishIt.Tests
{
    [TestFixture]
    public class JavaScriptMinifierTests
    {
        private string javaScript = @"
                                        function product(a, b)
                                        {
                                            return a * b;
                                        }

                                        function sum(a, b){
                                            return a + b;
                                        }";

        [Test]
        public void NullMinifierTest()
        {
            var javaScriptMinifier = MinifierFactory.Get<JavaScriptBundle, NullMinifier>();
            string minifiedJavaScript = javaScriptMinifier.Minify(javaScript);
            Assert.AreEqual(javaScript, minifiedJavaScript);
        }

        [Test]
        public void JsMinMinifierTest()
        {
            var javaScriptMinifier = MinifierFactory.Get<JavaScriptBundle, JsMinMinifier>();
            string minifiedJavaScript = javaScriptMinifier.Minify(javaScript);
            Assert.AreEqual("\nfunction product(a,b)\n{return a*b;}\nfunction sum(a,b){return a+b;}", minifiedJavaScript);
        }

        /*[Test]
        public void ClosureMinifierTest()
        {
            var javaScriptMinifier = MinifierRegistry.Get(ClosureMinifier.Identifier);
            string minifiedJavaScript = javaScriptMinifier.CompressContent(javaScript);
            Assert.AreEqual("function product(a,b){return a*b}function sum(a,b){return a+b};\r\n", minifiedJavaScript);
        }*/

        [Test]
        public void YuiMinifierTest()
        {
            var javaScriptMinifier = MinifierFactory.Get<JavaScriptBundle, YuiMinifier>();
            string minifiedJavaScript = javaScriptMinifier.Minify(javaScript);
            Assert.AreEqual("function product(c,d){return c*d}function sum(c,d){return c+d};", minifiedJavaScript);
        }

        [Test]
        public void MsMinifierTest()
        {
            var javaScriptMinifier = MinifierFactory.Get<JavaScriptBundle, MsMinifier>();
            string minifiedJavaScript = javaScriptMinifier.Minify(javaScript);
            Assert.AreEqual("function product(a,b){return a*b}function sum(a,b){return a+b}", minifiedJavaScript);
        }
    }
}