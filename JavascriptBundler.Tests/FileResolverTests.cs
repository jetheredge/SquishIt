using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using NUnit.Framework;

namespace JavascriptBundler.Tests
{
    [TestFixture]
    public class FileResolverTests
    {
        [Test]
        public void CanResolveFromFile()
        {
            var file = "\testfile.js";
            using (var sw = new StreamWriter(file, false))
            {
                sw.WriteLine(@"alert('hello');");
            }

            var fileResolver = new FileResolver();
            var resolvedFile = fileResolver.ResolveFromFile(file);
            Assert.AreEqual(@"C:\testfile.js", resolvedFile);
        }
    }
}
