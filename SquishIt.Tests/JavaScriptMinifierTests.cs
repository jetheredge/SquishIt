using NUnit.Framework;
using SquishIt.Framework.JavaScript.Minifiers;

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
            var javaScriptMinifier = MinifierRegistry.Get(NullMinifier.Identifier);
            string minifiedJavaScript = javaScriptMinifier.CompressContent(javaScript);
            Assert.AreEqual(javaScript, minifiedJavaScript);
        }

        [Test]
        public void JsMinMinifierTest()
        {
            var javaScriptMinifier = MinifierRegistry.Get(JsMinMinifier.Identifier);
            string minifiedJavaScript = javaScriptMinifier.CompressContent(javaScript);
            Assert.AreEqual("\nfunction product(a,b)\n{return a*b;}\nfunction sum(a,b){return a+b;}", minifiedJavaScript);
        }

        [Test]
        public void ClosureMinifierTest()
        {
            var javaScriptMinifier = MinifierRegistry.Get(ClosureMinifier.Identifier);
            string minifiedJavaScript = javaScriptMinifier.CompressContent(javaScript);
            Assert.AreEqual("function product(a,b){return a*b}function sum(a,b){return a+b};\r\n", minifiedJavaScript);
        }

        [Test]
        public void YuiMinifierTest()
        {
            var javaScriptMinifier = MinifierRegistry.Get(YuiMinifier.Identifier);
            string minifiedJavaScript = javaScriptMinifier.CompressContent(javaScript);
            Assert.AreEqual("function product(d,c){return d*c}function sum(d,c){return d+c};", minifiedJavaScript);
        }
    }
}